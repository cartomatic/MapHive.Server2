using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.Api
{
    /// <summary>
    /// File upload configuration settings
    /// </summary>
    public class FileUploadConfiguration
    {
        /// <summary>
        /// upload path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Time after the files can be removed
        /// </summary>
        public int? FileRetentionInMinutes { get; set; }
    }
}
