using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.DataModel
{
    public abstract class TranslationKeyBase : Base, ILocalization
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
        /// Whether or not a key is inherited; if so then in order to customize it user needs to tick Overwrite on and change it
        /// </summary>
        public bool? Inherited { get; set; }

        /// <summary>
        /// Whether or not this kwy should overwrite the inherited translations
        /// </summary>
        public bool? Overwrites { get; set; }

        /// <summary>
        /// Set of translations for a particular key
        /// </summary>
        public Translations Translations { get; set; }

        [JsonIgnore]
        public string TranslationsSerialized
        {
            get => Translations?.Serialized;
            set
            {
                if(Translations == null)
                    Translations = new Translations();

                Translations.Serialized = value;
            }
        }

        ITranslations ILocalization.Translations
        {
            get => Translations;

            set => Translations = (Translations)value;
        }
    }

    
}
