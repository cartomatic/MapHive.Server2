using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Result
{

    /// <summary>
    /// Custom result for JS script
    /// </summary>
    public class JavaScriptResult : ContentResult
    {
        public JavaScriptResult(string scriptContent, string encoding = "charset=utf-8")
        {
            Content = scriptContent;
            StatusCode = (int) HttpStatusCode.OK;
            ContentType = $"text/javascript; {encoding}";
        }
    }

}
