/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace AudienceNetwork.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;
    using AudienceNetwork.Editor;

    internal class PListParser
    {
        private const string NSAppTransportSecurityKey = "NSAppTransportSecurity";
        private const string NSExceptionDomainsKey = "NSExceptionDomains";
        private const string NSIncludesSubdomainsKey = "NSIncludesSubdomains";
        private const string NSExceptionRequiresForwardSecrecyKey = "NSExceptionRequiresForwardSecrecy";
        private const string CFBundleURLTypesKey = "CFBundleURLTypes";
        private const string CFBundleURLName = "CFBundleURLName";
        private static readonly PListDict FacebookNSExceptionDomainsEntry = new PListDict ()
        {
            { NSIncludesSubdomainsKey, true },
            { NSExceptionRequiresForwardSecrecyKey, false }
        };
        private static readonly PListDict FacebookNSExceptionDomainsKey = new PListDict ()
        {
            { "facebook.com", new PListDict(FacebookNSExceptionDomainsEntry) },
            { "fbcdn.net", new PListDict(FacebookNSExceptionDomainsEntry) },
            { "akamaihd.net", new PListDict(FacebookNSExceptionDomainsEntry) },
        };
        private static readonly PListDict FacebookNSAppTransportSecurity = new PListDict ()
        {
            { PListParser.NSExceptionDomainsKey, FacebookNSExceptionDomainsKey }
        };
        private string filePath;

        public PListParser (string fullPath)
        {
            this.filePath = fullPath;
            XmlReaderSettings settings = new XmlReaderSettings ();
            settings.ProhibitDtd = false;
            XmlReader plistReader = XmlReader.Create (this.filePath, settings);

            XDocument doc = XDocument.Load (plistReader);
            XElement plist = doc.Element ("plist");
            XElement dict = plist.Element ("dict");
            this.XMLDict = new PListDict (dict);
            plistReader.Close ();
        }

        public PListDict XMLDict { get; set; }

        public void UpdateFBSettings ()
        {
            // iOS 9+ Support
            WhilelistFacebookServersForNetworkRequests (this.XMLDict);
        }

        public void WriteToFile ()
        {
            // Corrected header of the plist
            string publicId = "-//Apple//DTD PLIST 1.0//EN";
            string stringId = "http://www.apple.com/DTDs/PropertyList-1.0.dtd";
            string internalSubset = null;
            XDeclaration declaration = new XDeclaration ("1.0", Encoding.UTF8.EncodingName, null);
            XDocumentType docType = new XDocumentType ("plist", publicId, stringId, internalSubset);

            this.XMLDict.Save (this.filePath, declaration, docType);
        }

        private static void WhilelistFacebookServersForNetworkRequests (PListDict plistDict)
        {
            if (!ContainsKeyWithValueType (plistDict, PListParser.NSAppTransportSecurityKey, typeof(PListDict))) {
                // We don't have a NSAppTransportSecurity entry. We can easily add one
                plistDict [PListParser.NSAppTransportSecurityKey] = PListParser.FacebookNSAppTransportSecurity;
                return;
            }

            var appTransportSecurityDict = (PListDict)plistDict [PListParser.NSAppTransportSecurityKey];
            if (!ContainsKeyWithValueType (appTransportSecurityDict, PListParser.NSExceptionDomainsKey, typeof(PListDict))) {
                appTransportSecurityDict [PListParser.NSExceptionDomainsKey] = PListParser.FacebookNSExceptionDomainsEntry;
                return;
            }

            var exceptionDomains = (PListDict)appTransportSecurityDict [PListParser.NSExceptionDomainsKey];
            foreach (var key in PListParser.FacebookNSExceptionDomainsEntry.Keys) {
                // Instead of just updating overwrite values to keep things up to date
                exceptionDomains [key] = FacebookNSExceptionDomainsEntry [key];
            }
        }

        private static bool ContainsKeyWithValueType (IDictionary<string, object> dictionary, string key, Type type)
        {
            if (dictionary.ContainsKey (key) &&
                type.IsAssignableFrom (dictionary [key].GetType ())) {
                return true;
            }

            return false;
        }
    }
}
