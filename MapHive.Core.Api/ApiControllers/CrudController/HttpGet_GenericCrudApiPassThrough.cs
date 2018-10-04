using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class CrudController<T, TDbCtx>
    {
        /// <summary>
        /// Performs a standard GetAsync (sort, filter,start,limit) pass through against a maphive core api
        /// </summary>
        /// <param name="route"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> CoreApiGetPassThroughAsync(
            string route,
            string sort = null, string filter = null, int start = 0, int limit = 25
        )
        {
            var apiResponse = await CoreApiCall(
                route,
                Method.GET,
                new Dictionary<string, object>
                {
                    {nameof(sort), sort},
                    {nameof(filter), filter},
                    {nameof(start), start},
                    {nameof(limit), limit}
                }
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard GetAsync (sort, filter,start,limit) against a maphive core api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> CoreApiGetAsync<TOut>(
            string route,
            string sort = null, string filter = null, int start = 0, int limit = 25
        )
        {
            var apiResponse = await CoreApiCall<TOut>(
                route,
                Method.GET,
                new Dictionary<string, object>
                {
                    {nameof(sort), sort},
                    {nameof(filter), filter},
                    {nameof(start), start},
                    {nameof(limit), limit}
                }
            );

            return apiResponse.Output;
        }

        /// <summary>
        /// Performs a standard GetAsync (sort, filter,start,limit) pass through against a rest api
        /// </summary>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> RestApiGetPassThroughAsync(
            string url,
            string route,
            string sort = null, string filter = null, int start = 0, int limit = 25
        )
        {
            var apiResponse = await RestApiCall(
                url,
                route,
                Method.GET,
                new Dictionary<string, object>
                {
                    {nameof(sort), sort},
                    {nameof(filter), filter},
                    {nameof(start), start},
                    {nameof(limit), limit}
                }
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard GetAsync (sort, filter,start,limit) against a rest api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> RestApiGetAsync<TOut>(
            string url,
            string route,
            string sort = null, string filter = null, int start = 0, int limit = 25
        )
        {
            var apiResponse = await RestApiCall<TOut>(
                url,
                route,
                Method.GET,
                new Dictionary<string, object>
                {
                    {nameof(sort), sort},
                    {nameof(filter), filter},
                    {nameof(start), start},
                    {nameof(limit), limit}
                }
            );

            return apiResponse.Output;
        }

        /// <summary>
        /// Performs a standard GetAsync (uuid) pass through against a maphive core api
        /// </summary>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> CoreApiGetPassThroughAsync(
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await CoreApiCall(
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.GET
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard GetAsync (uuid) against a maphive core api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> CoreApiGetAsync<TOut>(
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await CoreApiCall<TOut>(
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.GET
            );

            return apiResponse.Output;
        }

        /// <summary>
        /// Performs a standard GetAsync (uuid) pass through against a rest api
        /// </summary>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> RestApiGetPassThroughAsync(
            string url,
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await RestApiCall(
                url,
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.GET
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard GetAsync (uuid) against a rest api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> RestApiGetAsync<TOut>(
            string url,
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await RestApiCall<TOut>(
                url,
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.GET
            );

            return apiResponse.Output;
        }

    }
}
