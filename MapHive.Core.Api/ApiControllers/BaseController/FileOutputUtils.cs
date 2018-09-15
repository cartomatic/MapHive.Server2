using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Outputs a file as byte arr content response msg and deletes it afterwarads
        /// </summary>
        /// <param name="fPath"></param>
        /// <param name="outFileName"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> GetFile(string fPath, string outFileName = null)
        {
            //so warnings dissapear from async method when not using await
            await Task.Delay(0);

            var response = new HttpResponseMessage();
            try
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new ByteArrayContent(File.ReadAllBytes(fPath));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{(string.IsNullOrWhiteSpace(outFileName) ? Path.GetFileName(fPath) : outFileName)}"
                };

                //file has been read and will be output so can get rid of it now
                try
                {
                    File.Delete(fPath);
                }
                catch
                {
                    //ignore
                }
            }
            catch
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent("NO DATA");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            }

            return response;
        }
    }
}
