﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils;
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
        /// Performs a standard PutAsync (obj, uuid) pass through against a maphive core api
        /// </summary>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> CoreApiPutPassThroughAsync(
            string route,
            object obj,
            Guid? uuid = null
        )
        {
            var apiResponse = await CoreApiCall(
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.PUT,
                queryParams: null,
                data: obj
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard PutAsync (obj, uuid) against a maphive core api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> CoreApiPutAsync<TOut>(
            string route,
            object obj,
            Guid? uuid = null
        )
        {
            return (await CoreApiPutWithRawOutputAsync<TOut>(route, obj, uuid)).Output;
        }

        /// <summary>
        /// Performs a standard PutAsync (obj, uuid) against a maphive core api; automatically deserializes the output; returns a full ApiCallOutput object
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<RestApi.ApiCallOutput<TOut>> CoreApiPutWithRawOutputAsync<TOut>(
            string route,
            object obj,
            Guid? uuid = null
        )
        {
            var apiResponse = await CoreApiCall<TOut>(
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.PUT,
                queryParams: null,
                data: obj
            );

            return apiResponse;
        }

        /// <summary>
        /// Performs a standard PutAsync (obj, uuid) through against a rest api
        /// </summary>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<IActionResult> RestApiPutPassThroughAsync(
            string url,
            string route,
            object obj,
            Guid? uuid = null
        )
        {
            var apiResponse = await RestApiCall(
                url,
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.PUT,
                queryParams: null,
                data: obj
            );

            return ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Performs a standard PutAsync (obj, uuid) against a rest api; automatically deserializes the output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> RestApiPutAsync<TOut>(
            string url,
            string route,
            object obj,
            Guid? uuid = null
        )
        {
            var apiResponse = await RestApiCall<TOut>(
                url,
                uuid.HasValue ? $"{route}/{uuid}" : route,
                Method.PUT,
                queryParams: null,
                data: obj
            );

            return apiResponse.Output;
        }

    }
}
