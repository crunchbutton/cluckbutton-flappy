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
using System.Diagnostics;

namespace AudienceNetwork.Editor
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using AudienceNetwork.Editor;

    public static class XCodePostProcess
    {
        [PostProcessBuild(100)]
        public static void OnPostProcessBuild (BuildTarget target, string path)
        {
            // Unity renamed build target from iPhone to iOS in Unity 5, this keeps both versions happy
            if (target.ToString () == "iOS" || target.ToString () == "iPhone") {
                UpdatePlist (path);
            }

            if (target == BuildTarget.Android) {
                // The default Bundle Identifier for Unity does magical things that causes bad stuff to happen
                if (PlayerSettings.bundleIdentifier == "com.Company.ProductName") {
                    Debug.LogError ("The default Unity Bundle Identifier (com.Company.ProductName) will not work correctly.");
                }

                if (!ManifestMod.CheckManifest())
                {
                    // If something is wrong with the Android Manifest, try to regenerate it to fix it for the next build.
                    ManifestMod.GenerateManifest();
                }
            }
        }

        public static void UpdatePlist (string path)
        {
            const string FileName = "Info.plist";
            string fullPath = Path.Combine (path, FileName);

            var facebookParser = new PListParser (fullPath);
            facebookParser.UpdateFBSettings ();
            facebookParser.WriteToFile ();
        }
    }
}
