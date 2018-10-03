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
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> CoreApiPostPassThroughAsync(
            HttpRequestMessage request,
            string route,
            object obj
        )
        {
            var apiResponse = await CoreApiCall(
                request,
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
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> CoreApiPostAsync<TOut>(
            HttpRequestMessage request,
            string route,
            object obj
        )
        {
            var apiResponse = await CoreApiCall<TOut>(
                request,
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
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> RestApiPostPassThroughAsync(
            HttpRequestMessage request,
            string url,
            string route,
            object obj
        )
        {
            var apiResponse = await RestApiCall(
                request,
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
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> RestApiPostAsync<TOut>(
            HttpRequestMessage request,
            string url,
            string route,
            object obj = null
        )
        {
            var apiResponse = await RestApiCall<TOut>(
                request,
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
