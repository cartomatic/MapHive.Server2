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
        /// Performs a standard PostAsync (obj) pass through against a maphive core api
        /// </summary>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> CoreApiPostPassThroughAsync(
            string route,
            object obj
        )
        {
            var apiResponse = await CoreApiCall(
                route,
                Method.POST,
                queryParams: null,
                data: obj
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard PostAsync (obj) against a maphive core api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> CoreApiPostAsync<TOut>(
            string route,
            object obj
        )
        {
            var apiResponse = await CoreApiCall<TOut>(
                route,
                Method.POST,
                queryParams: null,
                data: obj
            );

            return apiResponse.Output;
        }

        /// <summary>
        /// Performs a standard PostAsync (obj) pass through against a rest api
        /// </summary>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> RestApiPostPassThroughAsync(
            string url,
            string route,
            object obj
        )
        {
            var apiResponse = await RestApiCall(
                url,
                route,
                Method.POST,
                queryParams: null,
                data: obj
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard PostAsync (obj) against a rest api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> RestApiPostAsync<TOut>(
            string url,
            string route,
            object obj = null
        )
        {
            var apiResponse = await RestApiCall<TOut>(
                url,
                route,
                Method.POST,
                queryParams: null,
                data: obj
            );

            return apiResponse.Output;
        }

    }
}
