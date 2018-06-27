using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    
    public partial class Organization
    {
        public enum OrganizationRole
        {
            Owner,
            Admin,
            Member
        }

        /// <summary>
        /// org owner role identifier. used to mark an owner role
        /// </summary>
        public const string OrgRoleIdentifierOwner = "org_owner";

        /// <summary>
        /// default en owner role name
        /// </summary>
        public const string OrgRoleNameOwner = "Owner";


        /// <summary>
        /// org admin role identifier; used to mark admin roles
        /// </summary>
        public static string OrgRoleIdentifierAdmin = "org_admin";

        /// <summary>
        /// default en admin role name
        /// </summary>
        public static string OrgRoleNameAdmin = "Admin";

        /// <summary>
        /// org member role identifier; used to mark standard members of an org
        /// </summary>
        public const string OrgRoleIdentifierMember = "org_member";

        /// <summary>
        /// default en member role name
        /// </summary>
        public const string OrgRoleNameMember = "Member";


        /// <summary>
        /// Creates an org role
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Role> CreateRoleAsync(DbContext dbCtx, OrganizationRole role)
        {
            var roleName = string.Empty;
            var roleIdentifier = string.Empty;

            switch (role)
            {
                case OrganizationRole.Owner:
                    roleIdentifier = OrgRoleIdentifierOwner;
                    roleName = OrgRoleNameOwner;
                    break;

                    case OrganizationRole.Admin:
                    roleIdentifier = OrgRoleIdentifierAdmin;
                    roleName = OrgRoleNameAdmin;
                    break;

                default:
                    roleIdentifier = OrgRoleIdentifierMember;
                    roleName = OrgRoleNameMember;
                    break;
            }

            return await CreateRoleAsync(dbCtx, roleIdentifier, roleName);
        }

        /// <summary>
        /// Creates a role object for an organization and links to an org; does not save org changes!
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Role> CreateRoleAsync(DbContext dbCtx, string roleName)
        {
            return await CreateRoleAsync(dbCtx, null, roleName);
        }

        /// <summary>
        /// Creates a role object for an organization and links to an org; does not save org changes!
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="roleIdentifier"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Role> CreateRoleAsync(DbContext dbCtx, string roleIdentifier, string roleName)
        {
            var role = await new Role
            {
                Identifier = roleIdentifier,
                Name = roleName
            }.CreateAsync(dbCtx);

            this.AddLink(role);
            
            return role;
        }

        /// <summary>
        /// Gets organization's owner role
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Role> GetOrgOwnerRoleAsync(DbContext dbCtx)
        {
            return (await this.GetChildrenAsync<Organization, Role>(dbCtx)).FirstOrDefault(r=>r.Identifier == OrgRoleIdentifierOwner);
        }

        /// <summary>
        /// Gets organization's admin role
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Role> GetOrgAdminRoleAsync(DbContext dbCtx)
        {
            return (await this.GetChildrenAsync<Organization, Role>(dbCtx)).FirstOrDefault(r => r.Identifier == OrgRoleIdentifierAdmin);
        }

        /// <summary>
        /// Gets organization's member role
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Role> GetOrgMemberRoleAsync(DbContext dbCtx)
        {
            return (await this.GetChildrenAsync<Organization, Role>(dbCtx)).FirstOrDefault(r => r.Identifier == OrgRoleIdentifierMember);
        }

        /// <summary>
        /// Gets an organization role
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Role> GetOrgRoleAsync(DbContext dbCtx, OrganizationRole role)
        {
            switch (role)
            {
                case OrganizationRole.Admin:
                    return await GetOrgAdminRoleAsync(dbCtx);

                case OrganizationRole.Owner:
                    return await GetOrgOwnerRoleAsync(dbCtx);

                case OrganizationRole.Member:
                default:
                    return await GetOrgMemberRoleAsync(dbCtx);
            }
        }

        /// <summary>
        /// Gets OrgRoles
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Role>> GetOrgRolesAsync(DbContext dbCtx)
        {
            return (await this.GetChildrenAsync<Organization, Role>(dbCtx)).Where(r => r.Identifier == OrgRoleIdentifierOwner || r.Identifier == OrgRoleIdentifierAdmin || r.Identifier == OrgRoleIdentifierMember);
        }

        /// <summary>
        /// Determines if a user is an org member (is assigned to an org)
        /// </summary>
        /// <param name="dbctx"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> IsOrgMember(DbContext dbctx, MapHiveUser user)
        {
            return await this.HasChildLinkAsync(dbctx, user);
        }

        /// <summary>
        /// Checks if user is an organization owner (user has the org owner role assigned)
        /// </summary>
        /// <param name="dbctx"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> IsOrgOwner(DbContext dbctx, MapHiveUser user)
        {
            return await user.HasChildLinkAsync(dbctx, await GetOrgOwnerRoleAsync(dbctx));
        }

        /// <summary>
        /// Checks if a user is an organization admin (user has the org admin role assigned)
        /// </summary>
        /// <param name="dbctx"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> IsOrgAdmin(DbContext dbctx, MapHiveUser user)
        {
            return await user.HasChildLinkAsync(dbctx, await GetOrgAdminRoleAsync(dbctx));
        }

        /// <summary>
        /// Gets a type of OrgRole from role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public OrganizationRole? GetOrgRoleFromRole(Role role)
        {
            if (role.Identifier == OrgRoleIdentifierMember)
                return OrganizationRole.Member;
            if (role.Identifier == OrgRoleIdentifierAdmin)
                return OrganizationRole.Admin;
            if (role.Identifier == OrgRoleIdentifierOwner)
                return OrganizationRole.Owner;
            return null;
        }

        /// <summary>
        /// Gets a mapping between org roles and users
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Dictionary<OrganizationRole, IEnumerable<Link>>> GetOrgRoles2UsersMap(DbContext dbCtx)
        {
            //need to obtain a user role within an organization!
            //so need the orgroles first, and then user ids linked to them
            var roles = await GetOrgRolesAsync(dbCtx);

            var roles2users = new Dictionary<OrganizationRole, IEnumerable<Link>>();

            foreach (var role in roles)
            {
                var orgRole = GetOrgRoleFromRole(role);
                if (orgRole.HasValue)
                {
                    roles2users.Add(orgRole.Value, await role.GetParentLinksAsync<Role, MapHiveUser>(dbCtx));
                }
            }

            return roles2users;
        }


        /// <summary>
        /// Works out a user role within an organization
        /// </summary>
        /// <param name="roles2users"></param>
        /// <returns></returns>
        public OrganizationRole? GetUserOrgRole(Dictionary<OrganizationRole, IEnumerable<Link>> roles2users, Guid userId)
        {
            //Note: roles are linked to users not the other way round; it is a user that has a role

            //check owner firs
            if(roles2users[OrganizationRole.Owner].Any(l => l.ParentUuid == userId))
                return OrganizationRole.Owner;
            if (roles2users[OrganizationRole.Admin].Any(l => l.ParentUuid == userId))
                return OrganizationRole.Admin;
            if (roles2users[OrganizationRole.Member].Any(l => l.ParentUuid == userId))
                return OrganizationRole.Member;

            return null;
        }
    }
}
