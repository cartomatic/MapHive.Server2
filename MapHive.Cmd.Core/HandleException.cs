using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DataModel.Validation;

namespace MapHive.Cmd.Core
{
    public partial class CommandHandler
    {
        protected virtual void HandleException(Exception ex, bool skipIntro = false)
        {

            if (!skipIntro)
            {
                ConsoleEx.WriteErr("Ooops, the following exception has been encountered: ");
            }

            //special treatment for validation exceptions; they may pop put when working with Base objects
            if (ex is ValidationFailedException)
            {
                var e = (ValidationFailedException)ex;

                foreach (var validationError in e.ValidationErrors)
                {

                    ConsoleEx.WriteLine(validationError.Message, ConsoleColor.DarkRed);

                    ConsoleEx.WriteLine($"\t{validationError.PropertyName}: {validationError.Message}", ConsoleColor.DarkYellow);
                }
            }
            else
            {
                var e = ex;

                ConsoleEx.WriteLine(e.Message, ConsoleColor.DarkRed);

                if (e.InnerException != null)
                {
                    ConsoleEx.WriteLine("Inner exceptions are: ", ConsoleColor.DarkYellow);
                }

                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    ConsoleEx.WriteLine(e.Message, ConsoleColor.DarkMagenta);

                }
            }

            Console.WriteLine();
        }
    }
}
