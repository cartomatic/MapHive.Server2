﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Cartomatic.Utils;
using Microsoft.Extensions.Configuration;

namespace MapHive.Core.IdentityServer.Cryptography
{
    public class Certificate
    {

        /// <summary>
        /// Gets a signing certificate
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 Get(CertificateConfig cfg = null)
        {
            X509Certificate2 cert = null;

            //read the cfg off the web.config if necessary
            if (cfg == null)
            {
                var appCfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                cfg = new CertificateConfig();
                appCfg.GetSection("CertificateConfig").Bind(cfg);
            }

            if(cfg == null)
                throw new InvalidOperationException("Certificate configuration object missing.");


            switch (cfg.StorageType)
            {
                case CertificateStorageType.File:
                    cert = ReadCertificateFromFile(cfg);
                    break;

                case CertificateStorageType.Embedded:
                    cert = GetEmbeddedCertificate(cfg);
                    break;

                case CertificateStorageType.Store:
                    cert = GetCertificateFromStore(cfg);
                    break;
            }

            return cert;
        }

        /// <summary>
        /// Reads certificate from Personal certificates
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        private static X509Certificate2 GetCertificateFromStore(CertificateConfig cfg)
        {
            X509Certificate2 cert = null;

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(X509FindType.FindBySubjectName, cfg.Subject, true); //true so only valid certs are removed
            if (certs.Count > 0)
            {
                cert = certs[0];
            }

            store.Close();

            return cert;
        }

        /// <summary>
        /// Reads a certificate from a file
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        private static X509Certificate2 ReadCertificateFromFile(CertificateConfig cfg)
        {
            X509Certificate2 cert = null;

            cert = !string.IsNullOrEmpty(cfg.Password) ? 
                  new X509Certificate2(cfg.FilePath.SolvePath(), cfg.Password, X509KeyStorageFlags.MachineKeySet) 
                : new X509Certificate2(cfg.FilePath.SolvePath(), string.Empty, X509KeyStorageFlags.MachineKeySet);

            return cert;
        }

        /// <summary>
        /// Reads a cretificate from an embedded resource
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        private static X509Certificate2 GetEmbeddedCertificate(CertificateConfig cfg)
        {
            X509Certificate2 cert = null;

            byte[] certData;
            var assembly = typeof(Certificate).Assembly;

            using (var stream = assembly.GetManifestResourceStream(cfg.NameSpace))
            {
                certData = ReadStream(stream);
            }

            cert = !string.IsNullOrEmpty(cfg.Password) ?
                  new X509Certificate2(certData, cfg.Password, X509KeyStorageFlags.MachineKeySet) 
                : new X509Certificate2(certData, string.Empty, X509KeyStorageFlags.MachineKeySet);

            return cert;
        }

        /// <summary>
        /// Reads stream into a byte arr
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}