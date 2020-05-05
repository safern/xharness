﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Mono.Options;

namespace Microsoft.DotNet.XHarness.CLI.CommandArguments.Android
{
    internal class AndroidTestCommandArguments : TestCommandArguments
    {
        private string? _packageName;

        /// <summary>
        /// If specified, attempt to run instrumentation with this name instead of the default for the supplied APK.
        /// If a given package has multiple instrumentations, failing to specify this may cause execution failure.
        /// </summary>
        public string? InstrumentationName { get; set; }

        /// <summary>
        /// If specified, attempt to run instrumentation with this name instead of the default for the supplied APK
        /// </summary>
        public string PackageName
        {
            get => _packageName ?? throw new ArgumentException("Package name not specified");
            set => _packageName = value;
        }

        /// <summary>
        /// Folder to copy off for output of executing the specified APK
        /// </summary>
        public string? DeviceOutputFolder { get; set; }

        public Dictionary<string, string> InstrumentationArguments { get; } = new Dictionary<string, string>();

        protected override OptionSet GetTestCommandOptions() => new OptionSet
        {
            { "device-out-folder=|dev-out=", "If specified, copy this folder recursively off the device to the path specified by the output directory",
                v => DeviceOutputFolder = RootPath(v)
            },
            { "instrumentation:|i:", "If specified, attempt to run instrumentation with this name instead of the default for the supplied APK.",
                v => InstrumentationName = v
            },
            { "package-name=|p=", "Package name contained within the supplied APK",
                v => PackageName = v
            },
            { "arg=", "Argument to pass to the instrumentation, in form key=value", v =>
                {
                    var argPair = v.Split('=');

                    if (argPair.Length != 2)
                    {
                        throw new ArgumentException($"The --arg argument expects 'key=value' format. Invalid format found in '{v}'");
                    }

                    if (InstrumentationArguments.ContainsKey(argPair[0]))
                    {
                        throw new ArgumentException($"Duplicate arg name '{argPair[0]}' found");
                    }

                    InstrumentationArguments.Add(argPair[0].Trim(), argPair[1].Trim());
                }
            },
        };

        public override void Validate()
        {
            base.Validate();

            // Validate this field
            PackageName = PackageName;
            AppPackagePath = AppPackagePath;
        }
    }
}
