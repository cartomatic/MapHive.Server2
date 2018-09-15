using System;
using System.Threading.Tasks;
using MapHive.Core.Cmd;
using Microsoft.Extensions.Configuration;

namespace MapHive.Cmd
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Task.WaitAll(
                Task.Run(() => MainAsync(args))
            );
        }

        static async Task MainAsync(string[] args)
        {
            var cmdWatcher = new Cartomatic.CmdPrompt.Core.CmdWatcher(new CommandHandler())
            {
                Prompt = "MapHive...Bzz...>",
                PromptColor = ConsoleColor.DarkBlue
            };

            //setup if needed
            MapHive.Core.Identity.UserManagerUtils.Configure("MapHiveIdentity");

            await cmdWatcher.Init(false);


            Console.ReadLine();
        }
    }
}
