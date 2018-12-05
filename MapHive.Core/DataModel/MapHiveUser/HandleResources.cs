using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class MapHiveUser
    {
        private static string ProfilePictureResourceIdentifier = "profile_picture";

        /// <summary>
        /// Handles user resources
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected async Task HandleResources(DbContext dbCtx, Guid uuid)
        {
            if (dbCtx is MapHive.Core.DAL.MapHiveDbContext mhDbCtx)
            {
                //if picture empty - clean up a resource
                if (string.IsNullOrEmpty(ProfilePicture))
                {
                    await DestroyResource(dbCtx, uuid);
                }

                //if does not look like guid... (just test length for the time being)
                else if (ProfilePicture.Length != 36)
                {
                    var newRes = new Resource(ProfilePicture)
                    {
                        OwnerId = uuid,
                        OwnerTypeId = this.GetTypeIdentifier(),
                        Identifier = ProfilePictureResourceIdentifier
                    };

                    //if get here, the resource should really be an image
                    await newRes.CreateAsync(dbCtx);

                    ProfilePicture = newRes.Uuid.ToString();
                }
            }

        }

        /// <summary>
        /// Destroys a resource
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected async Task DestroyResource(DbContext dbCtx, Guid uuid)
        {
            if (dbCtx is MapHive.Core.DAL.MapHiveDbContext mhDbCtx)
            {
                mhDbCtx.Resources.RemoveRange(mhDbCtx.Resources.AsNoTracking()
                        .Where(r => r.OwnerId == uuid && r.Identifier == ProfilePictureResourceIdentifier));

                await dbCtx.SaveChangesAsync();
            }
        }
    }
}
