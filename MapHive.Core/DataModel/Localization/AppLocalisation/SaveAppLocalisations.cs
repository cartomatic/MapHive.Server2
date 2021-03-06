﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public static partial class AppLocalization
    {
        public static async Task SaveLocalizationsAsync(DbContext dbCtx, IEnumerable<LocalizationClass> localizations,
            bool? overwrite, bool? upsert, IEnumerable<string> langsToImport)
        {

            var localisedDbCtx = (ILocalizedDbContext) dbCtx;
            var appNames = new List<string>();

            foreach (var incomingLc in localizations)
            {
                if (!appNames.Contains(incomingLc.ApplicationName))
                {
                    appNames.Add(incomingLc.ApplicationName);
                }

                var lc =
                        await localisedDbCtx.LocalizationClasses.FirstOrDefaultAsync(
                            x => x.ApplicationName == incomingLc.ApplicationName && x.ClassName == incomingLc.ClassName);

                if (overwrite == true)
                {
                    if (lc != null)
                    {
                        //this should nicely cleanup the translation keys too
                        await lc.DestroyAsync<LocalizationClass>(dbCtx, lc.Uuid);
                    }

                    lc = new LocalizationClass
                    {
                        ApplicationName = incomingLc.ApplicationName,
                        ClassName = incomingLc.ClassName,
                        InheritedClassName = incomingLc.InheritedClassName
                    };
                    await lc.CreateAsync(dbCtx);

                    foreach (var translationKey in incomingLc.TranslationKeys)
                    {
                        //filter out translations that are about to be saved
                        var translations = new Translations();
                        foreach (var lng in translationKey.Translations.Keys)
                        {
                            if (langsToImport == null || langsToImport.Contains(lng))
                            {
                                translations.Add(lng, translationKey.Translations[lng]);
                            }
                        }

                        var tk = new TranslationKey
                        {
                            LocalizationClassUuid = lc.Uuid,
                            Key = translationKey.Key,
                            Inherited = translationKey.Inherited,
                            Overwrites = translationKey.Overwrites,
                            Translations = translations ?? new Translations()
                        };
                        await tk.CreateAsync(dbCtx);
                    }
                }
                else
                {
                    //this is a non-overwrite, so it applies to both - localization and translation keys; whatever is in the db must be maintained
                    //only the missing bits and pieces are added

                    //take care of the translation key - if not there it must be created
                    if (lc == null)
                    {
                        lc = new LocalizationClass
                        {
                            ApplicationName = incomingLc.ApplicationName,
                            ClassName = incomingLc.ClassName,
                            InheritedClassName = incomingLc.InheritedClassName
                        };
                        await lc.CreateAsync(dbCtx);
                    }

                    //review the translations now
                    foreach (var translationKey in incomingLc.TranslationKeys)
                    {
                        //check if the translation key is already there
                        var tk =
                            await
                                localisedDbCtx.TranslationKeys.FirstOrDefaultAsync(
                                    x => x.LocalizationClassUuid == lc.Uuid && x.Key == translationKey.Key);

                        //create key if not there
                        if (tk == null)
                        {
                            tk = new TranslationKey
                            {
                                LocalizationClassUuid = lc.Uuid,
                                Key = translationKey.Key,
                                Translations = new Translations()
                            };
                            await tk.CreateAsync(dbCtx);
                        }

                        tk.Inherited = translationKey.Inherited;
                        tk.Overwrites = translationKey.Overwrites;

                        //filter out translations that are about to be saved
                        var translations = new Translations();
                        foreach (var lng in translationKey.Translations.Keys)
                        {
                            if (langsToImport == null || langsToImport.Contains(lng))
                            {
                                translations.Add(lng, translationKey.Translations[lng]);
                            }
                        }
                        
                        //check translations
                        foreach (var lng in translations.Keys)
                        {
                            if (upsert == true || !tk.Translations.ContainsKey(lng))
                            {
                                tk.Translations[lng] = translations[lng];
                            }
                        }
                        await tk.SilentUpdateAsync<TranslationKey>(dbCtx);
                    }
                }
            }

            //need to wipe out cache too...
            foreach (var appName in appNames)
            {
                InvalidateAppLocalizationsCache(appName);
            }
        }
    }
}
