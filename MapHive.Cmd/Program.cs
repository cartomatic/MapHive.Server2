using System;
using System.Threading.Tasks;
using MapHive.Cmd.Core;
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


            await cmdWatcher.Init(false);


            Console.ReadLine();
        }
    }
}
