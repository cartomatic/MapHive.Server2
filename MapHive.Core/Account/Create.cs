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
        public static async Task<AccountCreateOutput> CreateAccount(
            MapHiveDbContext dbCtx,
            AccountCreateInput input,
            UserManager<IdentityUser<Guid>> userManager
        )
        {
            var user = new MapHive.Core.DataModel.MapHiveUser()
            {
                Email = input.AccountDetails.Email,
                Forename = input.AccountDetails.Forename,
                Surname = input.AccountDetails.Surname,
                ContactPhone = input.AccountDetails.ContactPhone
            };

            //prepare the email template tokens known at this stage,
            var tokens = new Dictionary<string, object>
            {
                {"UserName", $"{user.GetFullUserName()} ({user.Email})"},
                {"Email", user.Email}
            };

            var accountCreateOutput = await MapHive.Core.DataModel.MapHiveUser.CreateUserAccountAsync(dbCtx, user, userManager, input.EmailAccount, input.EmailTemplate?.Prepare(tokens));
            user = accountCreateOutput.User;


            var appIdentifiers = input.LicenseOptions.Keys.ToArray();

            //get the apps

            //web router app
            var apps = await dbCtx.Applications.Where(a => appIdentifiers.Contains(a.ShortName)).ToListAsync();


            //now the org object
            var orgNameDesc = string.IsNullOrEmpty(input.AccountDetails.Company) ? input.AccountDetails.Email : input.AccountDetails.Company;
            var newOrg = new Organization
            {
                DisplayName = orgNameDesc,
                Description = orgNameDesc,

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
            await newOrg.Create(dbCtx, user, appIdentifiers);
            
            //TODO - with multiorg users, will need an org identifier for org users
            //TODO - org users are users that 'belong' to an org

            return new AccountCreateOutput
            {
                EmailTemplate = input.EmailTemplate,
                VerificationKey = accountCreateOutput.VerificationKey,
                InitialPassword = accountCreateOutput.InitialPassword
            };
        }
    }
}
