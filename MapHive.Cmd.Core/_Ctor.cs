using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Cmd.Core
{
    public partial class CommandHandler : Cartomatic.CmdPrompt.Core.DefaultCmdCommandHandler
    {
        public CommandHandler(string handlerInfo)
            : base(handlerInfo)
        {
            //register some xtra commands
            SetUpCommandMap(new Dictionary<string, string>
            {
                { "s", "setup" },
                { "conn", "dsc" },
                { "xuser", "destroyuser" },
                { "apps", "addapps" },
                { "u", "adduser" },
                { "ur", "adduserremote" },
                { "morg", "addmasterorg" }
            });


            //default db credentials
            SetDefaultDsc();
        }

        public CommandHandler()
            : this("MapHive2 CMD v1.0....")
        {
            Cartomatic.Utils.Identity.ImpersonateGhostUser();
        }
    }
}
