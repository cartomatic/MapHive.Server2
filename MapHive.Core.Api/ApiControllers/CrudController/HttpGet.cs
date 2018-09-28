using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Cartomatic.Utils.Filtering;
using Cartomatic.Utils.Reflection;
using Cartomatic.Utils.Sorting;
using MapHive.Core.Api.Extensions;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class CrudController<T, TDbCtx>
    {
        /// <summary>
        /// Default get all action
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> GetAsync(string sort = null, string filter = null, int start = 0, int limit = 25, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db))
                return NotAllowed();

            return await ReadAsync<T,T>(db ?? _dbCtx, sort, filter, start, limit);
        }

        /// <summary>
        /// Default get all action
        /// </summary>
        /// <typeparam name="TDto">Defualt get by id action with automated DTO operation output</typeparam>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> GetAsync<TDto>(string sort = null, string filter = null, int start = 0, int limit = 25, DbContext db = null) where TDto : class
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db))
                return NotAllowed();

            return await ReadAsync<T,TDto>(db ?? _dbCtx, sort, filter, start, limit);
        }

        /// <summary>
        /// Default get all action
        /// </summary>
        /// <typeparam name="TExtended">Type to use as a base for reading - used for customising the read src, usually for extended views</typeparam>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> GetExtendedAsync<TExtended>(string sort = null, string filter = null, int start = 0, int limit = 25, DbContext db = null)
        where TExtended : Base
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db))
                return NotAllowed();

            return await ReadAsync<TExtended, TExtended>(db ?? _dbCtx, sort, filter, start, limit);
        }

        /// <summary>
        /// Gets alist of objects; performs automated conversion of output into specified DTO
        /// </summary>
        /// <typeparam name="TRead">Type to read</typeparam>
        /// <typeparam name="TDto">DTO Type to convert the output into</typeparam>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadAsync<TRead, TDto>(DbContext db, string sort = null, string filter = null,
            int start = 0, int limit = 25)
            where TRead: Base
            where TDto : class
        {
            //all stuff is instance based, so need to obtain one first
            var obj = (TRead)Activator.CreateInstance(typeof(TRead));

            try
            {
                var filters = filter.ExtJsJsonFiltersToReadFilters();

                //this is web service read call so read as no tracking - detached 
                var data = await obj.ReadAsync(db, sort.ExtJsJsonSortersToReadSorters(), filters, start, limit, detached: true);

                if (data.Any())
                {
                    //got the data, so can get the count too.
                    Context.AppendTotalHeader(await obj.ReadCountAsync(db, filters));

                    //Note: this could and should be done in a more elegant way. but had no smart ideas at a time.
                    //will come back to this at some stage...
                    //do dto hocus pocus if needed
                    if (typeof(TRead) != typeof(TDto))
                    {
                        //Note: IDTO is on the DTO and is implemented on instance obviously. so need one
                        var inst = Cartomatic.Utils.Dto.IDtoUtils.CrateIDtoInstance<TDto>();

                        return Ok(data.Select(d => inst.ToDto(d)).ToList());
                    }
                    else
                    {
                        return Ok(data);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }

        /// <summary>
        /// Defualt get by id action
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> GetAsync(Guid uuid, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db))
                return NotAllowed();

            return await ReadAsync<T>(db ?? _dbCtx, uuid);
        }

        /// <summary>
        /// Defualt get by id action with automated DTO operation output
        /// </summary>
        /// <typeparam name="TDto">DTO Type to convert the output into</typeparam>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> GetAsync<TDto>(Guid uuid, DbContext db = null) where TDto : class
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db))
                return NotAllowed();

            return await ReadAsync<TDto>(db ?? _dbCtx, uuid);
        }

        /// <summary>
        /// Gets an object by id
        /// </summary>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <param name="uuid"></param>
        /// <typeparam name="TDto">Type to convert to; must implement IDTO of DTO</typeparam>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadAsync<TDto>(DbContext db, Guid uuid) where TDto : class
        {
            //all stuff is instance based, so need to obtain one first
            T obj = (T)Activator.CreateInstance(typeof(T));

            try
            {
                //this is web service read call so read as no tracking - detached 
                var entity = await obj.ReadAsync(db, uuid, detached: true);
                if (entity != null)
                {
                    //Note: this could and should be done in a more elegant way. but had no smart ideas at a time. will come back to this at some stage...
                    //do dto hocus pocus if needed
                    if (typeof(T) != typeof(TDto))
                    {
                        //Note: IDTO is on the DTO and is implemented on instance obviously. so need one
                        var inst = Cartomatic.Utils.Dto.IDtoUtils.CrateIDtoInstance<TDto>();

                        return Ok(inst.ToDto(entity));
                    }
                    else
                    {
                        return Ok(entity);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }

        /// <summary>
        /// Reads links for given property
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="propertySpecifier"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadLinksAsync(Guid uuid, Expression<Func<T, IEnumerable<Base>>> propertySpecifier)
        {
            return await ReadLinksAsync(_dbCtx, uuid, propertySpecifier);
        }

        /// <summary>
        /// Reads links for given property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="uuid"></param>
        /// <param name="propertySpecifier"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadLinksAsync(DbContext db, Guid uuid, Expression<Func<T, IEnumerable<Base>>> propertySpecifier)
        {
            //first get an instance of T to call the appropriate methods on it
            var obj = (T)Activator.CreateInstance(typeof(T));

            //read the object from the db
            //this is web service read call so read as no tracking - detached 
            obj = await obj.ReadAsync(db, uuid, detached: true);

            if (obj == null)
            {
                return BadRequest();
            }

            //looks like the object has been retrieved, so can load the links for it now
            try
            {
                //read the links for a specific property
                //this is web service read call so read as no tracking - detached 
                await obj.MaterializeLinksAsDetachedAsync(db, propertySpecifier);

                //at this stage should have the links loaded, so can return the content of a property
                var mi = propertySpecifier.GetPropertyMemberInfoFromExpression();

                var property = obj.GetType().GetProperty(mi.Name);

                var data = property.GetValue(obj) as IEnumerable<Base>;

                if (data == null || !data.Any())
                {
                    return NotFound();
                }
                else
                {
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                //if something goes wrong just fail
                return InternalServerError();
            }
        }


        /// <summary>
        /// Reads parents of given type
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadParentsAsync<TParent>(Guid uuid)
            where TParent : Base
        {
            return await ReadParentsAsync<TParent>(_dbCtx, uuid);
        }

        /// <summary>
        /// Reads parents of given type
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="db"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadParentsAsync<TParent>(DbContext db, Guid uuid)
            where TParent : Base
        {
            //first get an instance of T to call the appropriate methods on it
            var obj = (T)Activator.CreateInstance(typeof(T));

            //read the object from the db
            //this is web service read call so read as no tracking - detached 
            obj = await obj.ReadAsync(db, uuid, detached: true);

            if (obj == null)
            {
                return BadRequest();
            }

            //looks like the object has been retrieved, so can load the links for it now
            try
            {
                //read the links for a specific property
                //this is web service read call so read as no tracking - detached 
                var data = await obj.GetParentsAsync<T, TParent>(db, detached: true);

                if (data == null || !data.Any())
                {
                    return NotFound();
                }
                else
                {
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                //if something goes wrong just fail
                return InternalServerError();
            }
        }


        /// <summary>
        /// Reads children of given type
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadChildrenAsync<TChild>(Guid uuid)
            where TChild : Base
        {

            return await ReadChildrenAsync<TChild>(_dbCtx, uuid);
        }

        /// <summary>
        /// Reads children of given type
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="db"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadChildrenAsync<TChild>(DbContext db, Guid uuid)
            where TChild : Base
        {
            //first get an instance of T to call the appropriate methods on it
            var obj = (T)Activator.CreateInstance(typeof(T));

            //read the object from the db
            //this is web service read call so read as no tracking - detached 
            obj = await obj.ReadAsync(db, uuid, detached: true);

            if (obj == null)
            {
                return BadRequest();
            }

            //looks like the object has been retrieved, so can load the links for it now
            try
            {
                //read the links for a specific property
                //this is web service read call so read as no tracking - detached 
                var data = await obj.GetChildrenAsync<T, TChild>(db, detached: true);

                if (data == null || !data.Any())
                {
                    return NotFound();
                }
                else
                {
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                //if something goes wrong just fail
                return InternalServerError();
            }
        }

        /// <summary>
        /// Reads first child of given type
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadFirstChildAsync<TChild>(Guid uuid)
            where TChild : Base
        {

            return await ReadFirstChildAsync<TChild>(_dbCtx, uuid);
        }

        /// <summary>
        /// Reads first child of given type
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="db"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ReadFirstChildAsync<TChild>(DbContext db, Guid uuid)
            where TChild : Base
        {
            //first get an instance of T to call the appropriate methods on it
            var obj = (T)Activator.CreateInstance(typeof(T));

            //read the object from the db
            //this is web service read call so read as no tracking - detached 
            obj = await obj.ReadAsync(db, uuid, detached: true);

            if (obj == null)
            {
                return BadRequest();
            }

            //looks like the object has been retrieved, so can load the links for it now
            try
            {
                //read the links for a specific property
                //this is web service read call so read as no tracking - detached 
                var data = await obj.GetFirstChildAsync<T, TChild>(db, detached: true);

                if (data == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                //if something goes wrong just fail
                return InternalServerError();
            }
        }
    }
}
