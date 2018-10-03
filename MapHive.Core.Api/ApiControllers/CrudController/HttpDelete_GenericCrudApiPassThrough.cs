using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using RestSharp;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class CrudController<T, TDbCtx>
    {
        /// <summary>
        /// Performs a standard DeleteAsync (obj, uuid) pass through against a maphive core api
        /// </summary>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> CoreApiDeletePassThroughAsync(
            HttpRequestMessage request,
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await CoreApiCall(
                request,
                uuid.HasValue ? $"{route}/{uuid}" :  route,
                Method.DELETE
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard DeleteAsync (obj, uuid) against a maphive core api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> CoreApiDeleteAsync<TOut>(
            HttpRequestMessage request,
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await CoreApiCall<TOut>(
                request,
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.DELETE
            );

            return apiResponse.Output;
        }

        /// <summary>
        /// Performs a standard DeleteAsync (obj, uuid) through against a rest api
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> RestApiDeletePassThroughAsync(
            HttpRequestMessage request,
            string url,
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await RestApiCall(
                request,
                url,
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.DELETE
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard DeleteAsync (obj, uuid) against a rest api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> RestApiDeleteAsync<TOut>(
            HttpRequestMessage request,
            string url,
            string route,
            Guid? uuid = null
        )
        {
            var apiResponse = await RestApiCall<TOut>(
                request,
                url,
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.DELETE
            );

            return apiResponse.Output;
        }

    }
}
