using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using MapHive.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MapHive.Core.IdentityServer.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(
                Task.Run(() => MainAsync(args))
            );
        }

        static async Task MainAsync(string[] args)
        {
            Console.Write("Testing obtaining client credentials...");

            // request the token from the Auth server
            var tokenClient = new TokenClient("http://localhost:5000/connect/token", "maphive-apis-client", "maphive-apis-client-test-secret");
            var clientCredentials = await tokenClient.RequestClientCredentialsAsync("maphive_apis");

            //note: client needs to support client credentials flow for the above
            Console.Write($"Done!" + Environment.NewLine);
            Console.WriteLine("output:");
            Console.WriteLine(JsonConvert.SerializeObject(clientCredentials.Json, Formatting.Indented));
            Console.WriteLine();


            var email = "queen@maphive.net";
            var pass = "test";

            Console.Write($"Configuring {nameof(UserManagerUtils)}... ");
            MapHive.Core.Identity.UserManagerUtils.Configure("MapHiveIdentity");
            var userManager  = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
            var signInManager = MapHive.Core.Identity.UserManagerUtils.GetSignInManager();
            Console.Write("Done!" + Environment.NewLine);
            Console.WriteLine();

            Console.Write($"Checking password validity via AspIdentity UserManager ({email} :: {pass})... ");
            var user = await userManager.FindByNameAsync(email);
            var passOk = await userManager.CheckPasswordAsync(user, pass);
            Console.Write($"Done!" + Environment.NewLine);
            Console.WriteLine($"Password valid: {passOk}");
            Console.WriteLine();


            Console.Write($"Checking sign in via AspIdentity SignInManager ({email} :: {pass})... ");
            var signIn = await signInManager.CheckPasswordSignInAsync(user, "test", false);
            Console.Write($"Done!" + Environment.NewLine);
            Console.WriteLine("output:");
            Console.WriteLine(JsonConvert.SerializeObject(signIn, Formatting.Indented));
            Console.WriteLine();


            Console.Write($"Checking auth service via {nameof(MapHive)}.{nameof(MapHive.Core)}.{nameof(MapHive.Core.Auth)} ({email} :: {pass})... ");
            var mhAuthTest = await MapHive.Core.Auth.LetMeInAsync(email, pass);
            Console.Write($"Done!" + Environment.NewLine);
            Console.WriteLine("output:");
            Console.WriteLine(JsonConvert.SerializeObject(mhAuthTest, Formatting.Indented));
            Console.WriteLine();

            Console.WriteLine("Finished!");

            Console.ReadLine();
        }
    }
}
