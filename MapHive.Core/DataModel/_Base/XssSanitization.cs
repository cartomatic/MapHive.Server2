using Ganss.XSS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MapHive.Core.DataModel
{
    public abstract partial class Base
    {
        /// <summary>
        /// Xss sanitizes the model - takes care of all the string, list of string and string arr properties
        /// </summary>
        public virtual void XssSanitize()
        {
            //in order to perform automated sanitization, grab all the public string properties with getters & setters
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var ignore = XssSanitizationIgnoredProperties ?? new string[0];
            var include = XssSanitizationIncludedProperties ?? new string[0];

            foreach (var prop in props)
            {
                if (ignore.Contains(prop.Name))
                    continue;

                //auto ignore props with 'Serialized' string in the name
                if(prop.Name.Contains("Serialized") && !include.Contains(prop.Name))
                    continue;

                //ignore properties that do not have a getter & setter - need to read, sanitize & write back after all
                if (prop.GetAccessors().Length != 2)
                    continue;

                try
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        XssSanitizeStringProperty(prop);
                    }
                    else if (prop.PropertyType == typeof(List<string>))
                    {
                        XssSanitizeListOfStringProperty(prop);
                    }
                    else if (prop.PropertyType == typeof(string[]))
                    {
                        XssSanitizeArrOfStringProperty(prop);
                    }
                }
                catch
                {
                    //ignore
                }
            }

            XssSanitizeCustom();
        }

        /// <summary>
        /// Returns names of properties that should be skipped when Xss sanitizing
        /// </summary>
        protected virtual string[] XssSanitizationIgnoredProperties => null;

        /// <summary>
        /// Properties to be included in sanitization - used to sanitize properties that contain 'Serialized' string (they are ignored by default)
        /// </summary>
        protected virtual string[] XssSanitizationIncludedProperties => null;

        /// <summary>
        /// Custom Xss sanitization extension hook
        /// </summary>
        protected virtual void XssSanitizeCustom() { }

        /// <summary>
        /// Xss sanitizes a string property
        /// </summary>
        /// <param name="pInfo"></param>
        protected void XssSanitizeStringProperty(PropertyInfo pInfo)
        {
            if (pInfo.PropertyType != typeof(string))
                return;

            var pValue = (string)pInfo.GetValue(this);
            if (!string.IsNullOrEmpty(pValue))
            {
                pInfo.SetValue(this, XssSanitizeString(pValue));
            }
        }

        /// <summary>
        /// Xss sanitizes string[]
        /// </summary>
        /// <param name="pInfo"></param>
        protected void XssSanitizeArrOfStringProperty(PropertyInfo pInfo)
        {
            if (pInfo.PropertyType != typeof(string[]))
                return;

            var arr = (string[])pInfo.GetValue(this);

            if (arr == null || arr.Length == 0)
                return;

            for (var i = 0; i < arr.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(arr[i]))
                    arr[i] = XssSanitizeString(arr[i]);
            }
        }

        /// <summary>
        /// Xss sanitizes List[string]
        /// </summary>
        /// <param name="pInfo"></param>
        protected void XssSanitizeListOfStringProperty(PropertyInfo pInfo)
        {
            if (pInfo.PropertyType != typeof(List<string>))
                return;

            var list = (List<string>)pInfo.GetValue(this);

            if (list == null || !list.Any())
                return;

            for (var i = 0; i < list.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(list[i]))
                    list[i] = XssSanitizeString(list[i]);
            }
        }

        /// <summary>
        /// xss sanitizes a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string XssSanitizeString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sanitizer = GetSanitizerStrictest();
            return sanitizer.Sanitize(str);
        }


        protected static HtmlSanitizer HtmlSanitizerStrictest = null;

        /// <summary>
        /// Returns a strict sanitizer - fully cleans html stuff
        /// </summary>
        /// <returns></returns>
        protected HtmlSanitizer GetSanitizerStrictest()
        {
            if (HtmlSanitizerStrictest == null)
            {
                HtmlSanitizerStrictest = new HtmlSanitizer();
                HtmlSanitizerStrictest.AllowedTags.Clear();
                HtmlSanitizerStrictest.AllowedAtRules.Clear();
                HtmlSanitizerStrictest.AllowedAttributes.Clear();
                HtmlSanitizerStrictest.AllowedCssClasses.Clear();
                HtmlSanitizerStrictest.AllowedCssProperties.Clear();
            }

            return HtmlSanitizerStrictest;
        }
    }
}
