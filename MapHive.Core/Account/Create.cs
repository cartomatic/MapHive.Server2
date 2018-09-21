using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core
{
    /// <summary>
    /// provides platform account related apis
    /// </summary>
    public partial class Account
    {
        /// <summary>
        /// Creates an org owner account - creates a user profile, an organization for a user, ties all the bits and pieces together
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<AccountCreateOutput> CreateAccountAsync(
            MapHiveDbContext dbCtx,
            AccountCreateInput input
        )
        {
            var user = new MapHive.Core.DataModel.MapHiveUser()
            {
                Email = input.AccountDetails.Email,
                Slug = input.AccountDetails.Slug,
                Forename = input.AccountDetails.Forename,
                Surname = input.AccountDetails.Surname,

                Company = input.AccountDetails.Company,
                Department = input.AccountDetails.Department,
                ContactPhone = input.AccountDetails.ContactPhone
            };

            //prepare the email template tokens known at this stage,
            var tokens = new Dictionary<string, object>
            {
                {"UserName", $"{user.GetFullUserName()} ({user.Email})"},
                {"Email", user.Email}
            };

            var accountCreateOutput = await MapHive.Core.DataModel.MapHiveUser.CreateUserAccountAsync(dbCtx, user,
                input.EmailAccount, input.EmailTemplate?.Prepare(tokens));
            user = accountCreateOutput.User;



            //now the org object
            var orgNameDesc = string.IsNullOrEmpty(input.AccountDetails.Company) ? input.AccountDetails.Email + "-org" : input.AccountDetails.Company;
            var newOrg = new Organization
            {
                DisplayName = orgNameDesc,
                Description = orgNameDesc,
                Slug = Utils.Slug.GetOrgSlug(orgNameDesc, user.Slug + "-org"),

                //push to extra billing info?
                BillingExtraInfo = new SerializableDictionaryOfString
                {
                    { nameof(input.AccountDetails.ContactPhone), input.AccountDetails.ContactPhone},
                    { nameof(input.AccountDetails.Email), input.AccountDetails.Email},
                    { "ContactPerson", $"{input.AccountDetails.Forename} {input.AccountDetails.Surname}"},
                    { nameof(input.AccountDetails.Street), input.AccountDetails.Street},
                    { nameof(input.AccountDetails.HouseNo), input.AccountDetails.HouseNo},
                    { nameof(input.AccountDetails.FlatNo), input.AccountDetails.FlatNo},
                    { nameof(input.AccountDetails.Postcode), input.AccountDetails.Postcode},
                    { nameof(input.AccountDetails.City), input.AccountDetails.City},
                    { nameof(input.AccountDetails.Country), input.AccountDetails.Country},
                    { nameof(input.AccountDetails.VatNumber), input.AccountDetails.VatNumber}
                },

                LicenseOptions = new OrganizationLicenseOptions()
            };

            //see what apps the client api wants to register
            var appIdentifiers = input.LicenseOptions.Keys.ToArray();
            //get them...
            var apps = await dbCtx.Applications.Where(a => appIdentifiers.Contains(a.ShortName)).ToListAsync();

            //always make sure to glue in the core apis and apps
            apps.AddRange(
                MapHive.Core.Defaults.Applications.GetDefaultOrgApps()
            );

            foreach (var appShortName in input.LicenseOptions.Keys)
            {
                var app = apps.FirstOrDefault(a => a.ShortName == appShortName);
                if(app == null)
                    continue;
                
                newOrg.LicenseOptions.Add(
                    new OrganizationLicenseOption
                    {
                        LicensedObjectTypeUuid = app.TypeUuid,
                        LicensedObjectUuid = app.Uuid,
                        LicenseOptions = input.LicenseOptions[appShortName]
                    }
                );
            }


            //create an org with owner and register the specified apps
            //make sure though to use the collection that contains the the default org apps!
            await newOrg.CreateAsync(dbCtx, user, apps);

            //wire up user with his 'parent org'
            //this is so it is clear a user has his own org
            user.UserOrgId = newOrg.Uuid;
            await user.UpdateAsync(dbCtx);

            return new AccountCreateOutput
            {
                EmailTemplate = input.EmailTemplate,
                VerificationKey = accountCreateOutput.VerificationKey,
                InitialPassword = accountCreateOutput.InitialPassword
            };
        }
    }
}
