using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MapHive.Core.Api.Extensions;
using Serilog;
using Serilog.Events;


namespace MapHive.Core.Api
{

    public class Program
    {
        public static int Main(string[] args)
        {
            return MapHive.Core.Api.WebHostUtils.BuildAndRunWebHost<Startup>(args);
        }
    }
}
