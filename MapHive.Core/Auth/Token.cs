using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core
{
    public partial class Auth
    {
        /// <summary>
        /// Merges user id with a token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string MergeIdWithToken(Guid userId, string token)
        {
            return $"{userId:N}.{token}";
        }

        /// <summary>
        /// Extracts token from a merged id + tokken string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ExtractTokenFromMergedToken(string input)
        {
            return input.Substring(input.IndexOf(".", StringComparison.Ordinal));
        }

        /// <summary>
        /// Extracts id from a merged id + tokken string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Guid? ExtractIdFromMergedToken(string input)
        {
            Guid.TryParseExact(input.Substring(0, input.IndexOf(".", StringComparison.Ordinal)), "N", out var guid);
            return guid;
        }
    }
}
