using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// License options as set on an organization; pretty much a list of customised opts per linked object
    /// </summary>
    public partial class OrganizationLicenseOptions : List<OrganizationLicenseOption>
    {
        private static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// hide the base List Capacity property so EF does not force pushes it into db model!
        /// </summary>
        private new int Capacity
        {
            get => base.Capacity;
            set => base.Capacity = value;
        } 

        /// <summary>
        /// This property is used so EF can nicely read/write and persist the data without a messy setup
        /// The TypeConfiguration setup should look like this: Property(p => p.LinkData.Serialised).HasColumnName("some_column_name");
        /// </summary>
        /// <remarks>
        /// Idea ported from MapHive's LinkData / Translations
        /// </remarks>
        [JsonIgnore]
        public string Serialised
        {
            get
            {
                //remove the opts with inherited
                foreach (var orgLicOpts in this)
                {
                    orgLicOpts.LicenseOptions?.CleanInherited();
                }

                return JsonConvert.SerializeObject(this, Formatting.None, JsonSerializerSettings);
            }
            set
            {
                //deserialise...
                List<OrganizationLicenseOption> incomingStringData = null;
                try
                {
                    incomingStringData = JsonConvert.DeserializeObject<List<OrganizationLicenseOption>>(value, JsonSerializerSettings);
                }
                catch
                {
                    //ignore - silently fail
                }

                //do not modify self if this was invalid json...
                if (incomingStringData == null) return;

                //clear self, so can pump in the data
                this.Clear();

                foreach (var v in incomingStringData)
                {
                    this.Add(v);
                }
            }
        }

        /// <summary>
        /// Applies license options into this collection; makes sure that customised license options are maintained
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="licensedObjects">The object from which to transfer the default options</param>
        public void Apply<T>(IEnumerable<T> licensedObjects)
            where T: Base, ILicenseOptions, INamed
        {
            var t = (T)Activator.CreateInstance(typeof (T));

            //extract current license options of given licensed objects type
            var currentLicenseOpts = this.Where(lo => lo.LicensedObjectTypeUuid == t.TypeUuid).ToList();

            //and remove them from the collection
            foreach (var currentOpt in currentLicenseOpts)
            {
                Remove(currentOpt);
            }
            
            //now add the incoming opts, but make sure to override them with whatever has been saved there before
            foreach (var licensedObject in licensedObjects)
            {
                var newOpts = new OrganizationLicenseOption
                {
                    LicensedObjectTypeUuid = t.TypeUuid,
                    LicensedObjectUuid = licensedObject.Uuid,
                    LicensedObjectType = licensedObject.GetType().FullName,
                    LicensedObjectName = licensedObject.Name,
                    LicenseOptions = licensedObject.LicenseOptions.Clone()
                };

                //initially mark all the opts as inherited
                //overriding them with the org lvl changes happens later
                foreach (var lOpt in newOpts.LicenseOptions)
                {
                    lOpt.Value.Inherited = true;
                }

                //copy over the properties from the currently held licence options for an object to a new one.
                var currentOpts =
                    currentLicenseOpts.FirstOrDefault(
                        lo => lo.LicensedObjectTypeUuid == licensedObject.TypeUuid && lo.LicensedObjectUuid == licensedObject.Uuid);

                if (currentOpts?.LicenseOptions != null)
                {
                    //need to spin through the licensedObject.LicenseOptions.Keys not newOpts.LicenseOptions
                    //as the latter will be modified and would throw because of that!
                    foreach (var key in licensedObject.LicenseOptions.Keys)
                    {
                        //make sure though to only copy the properties that are actually specified on the incoming ILicenceOpts object; this should always be what is currently supported
                        if (currentOpts?.LicenseOptions.ContainsKey(key) == true)
                        {
                            //compare values - if the same, mark an opt as inherited.
                            //this does not edit the db content, but if a user decides to re-save a rec, an option will be removed as it is a default now
                            if (newOpts.LicenseOptions[key].Value == currentOpts.LicenseOptions[key].Value)
                                newOpts.LicenseOptions[key].Inherited = true;


                            newOpts.LicenseOptions[key] = currentOpts.LicenseOptions[key];
                        }
                    }
                }

                this.Add(newOpts);
            }
        }
    }
}
