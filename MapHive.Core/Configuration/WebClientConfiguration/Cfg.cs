﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.Configuration
{
    public partial class WebClientConfiguration
    {
        /// <summary>
        /// UrlPart / hash property names; used to pass some data between the apps
        /// </summary>
        public static Dictionary<string, string> AppHashProperties { get; set; } = new Dictionary<string, string>
            {
                { "app", "a" },
                { "route", "r" },
                { "accessToken", "at" },
                { "suppressAppToolbar", "suppress-app-toolbar" },
                { "hosted", "hosted" },
                { "suppressSplash", "suppress-splash" },
                { "auth", "auth" },
                { "verificationKey", "vk" }
            };

        /// <summary>
        /// Name of the web client configuration variable output to the client
        /// </summary>
        public static string MhCfgVariableName { get; set; } = "__mhcfg__";

        /// <summary>
        /// Identifier column for the data store tables
        /// </summary>
        public static string MhIdCol { get; set; } = MapHive.Core.DataModel.Map.DataStore.IdCol;

        /// <summary>
        /// Hash property delimiters
        /// </summary>
        public static string HashPropertyDelimiter { get; set; } = ";";

        /// <summary>
        /// Hash property value delimiter
        /// </summary>
        public static string HashPropertyValueDelimiter { get; set; } = ":";

        /// <summary>
        /// Name of the settings cookie
        /// </summary>
        public static string MhCookie { get; set; } = "mh";

        /// <summary>
        /// Cookie lifetime expressed in seconds
        /// </summary>
        public static int CookieValidSeconds { get; set; } = 60 * 60 * 24 * 365; //make it a year or so

        /// <summary>
        /// URL param used to identify language the app should localise itself for
        /// </summary>
        public static string LangParam { get; set; } = "lng";

        /// <summary>
        /// Header used to pass lang info along
        /// </summary>
        public static string HeaderLang { get; set; } = "MH-Lng";

        /// <summary>
        /// Source header used to send the full location of the request; handy when need to revide the url part stuff (after hash) as this is not sent to the server by a browser
        /// </summary>
        public static string HeaderSource { get; set; } = "MH-Src";

        /// <summary>
        /// header used when returning lists to indicate a full dataset count for given request
        /// </summary>
        public static string HeaderTotal { get; set; } = "MH-Total";

        /// <summary>
        /// Returns a hash used to activate a user account; Uses a '{InitialPassword}' token as the replacement token placeholder for the password
        /// </summary>
        public static string ActivateAccountLinkHash =>
            $"{{RedirectUrl}}#{AppHashProperties["auth"]}{HashPropertyValueDelimiter}activateaccount{HashPropertyDelimiter}{AppHashProperties["verificationKey"]}{HashPropertyValueDelimiter}{{VerificationKey}}";

        /// <summary>
        /// Returns a hash used to trigger a reset pass finalisation procedure; uses a '{VerificationKey}' token as the verification token placeholder
        /// </summary>
        public static string ResetPassLinkHash =>
            $"{{RedirectUrl}}#{AppHashProperties["auth"]}{HashPropertyValueDelimiter}resetpass{HashPropertyDelimiter}{AppHashProperties["verificationKey"]}{HashPropertyValueDelimiter}{{VerificationKey}}";

        /// <summary>
        /// gets a collection of mh header names
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMhHeaders()
        {
            return new List<string>
            {
                HeaderLang,
                HeaderSource
            };
        }
    }
}
