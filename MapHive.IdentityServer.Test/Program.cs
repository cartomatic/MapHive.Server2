using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using MapHive.Identity;

namespace MapHive.IdentityServer.Test
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

            // request the token from the Auth server
            var tokenClient = new TokenClient("http://localhost:5000/connect/token", "maphive-apis-client", "maphive-apis-client-test-secret");
            var response = await tokenClient.RequestClientCredentialsAsync("maphive_apis");
            //note: client needs to support client credentials flow for the above


            MapHive.Identity.UserManagerUtils.Configure("MapHiveIdentity");

            var userManager  = MapHive.Identity.UserManagerUtils.GetUserManager();
            var signInManager = MapHive.Identity.UserManagerUtils.GetSignInManager();

            var user = await userManager.FindByNameAsync("queen@maphive.net");

            var passOk = await userManager.CheckPasswordAsync(user, "test");

            var signIn = await signInManager.CheckPasswordSignInAsync(user, "test", false);

            var test = await MapHive.Core.Auth.LetMeInAsync("queen@maphive.net", "test");


            Console.ReadLine();
        }
    }
}
