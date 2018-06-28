using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.DataModel
{
    public partial class TranslationKey : Base, ILocalization
    {
        /// <summary>
        /// Identifier of a Localization class name a translation applies to;
        /// </summary>
        public Guid LocalizationClassUuid { get; set; }

        /// <summary>
        /// A key that identifies the translation string within a translations class
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Set of translations for a particular key
        /// </summary>
        public Translations Translations { get; set; }

        [JsonIgnore]
        public string TranslationsSerialized
        {
            get => Translations.Serialized;
            set => Translations.Serialized = value;
        }

        ITranslations ILocalization.Translations
        {
            get => Translations;

            set => Translations = (Translations)value;
        }
    }
    
}
