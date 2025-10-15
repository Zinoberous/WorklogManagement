// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace WorklogManagement.Shared;

public static class Assembly
{
    public static string Version => FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly()!.Location).ProductVersion ?? "0.0.0";
}
