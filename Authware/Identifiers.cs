using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Authware;

/// <summary>
///     Handles the hardware ID aspect of API requests
/// </summary>
internal static class Identifiers
{
    /// <summary>
    ///     Generates a hardware ID of the current device and hashes it with SHA256 for extra security
    /// </summary>
    /// <returns>The hardware ID of the current device, hashed with SHA256 and encoded to a HEX string</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when the current OS is not a NT-based system (Windows NT+)</exception>
    internal static string GetIdentifier()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            if (Debugger.IsAttached)
                throw new PlatformNotSupportedException(
                    $"The default hardware ID system is not supported on {RuntimeInformation.OSDescription}, if you want to add support for the current platform, you can set-up your own hardware ID system through the AuthwareApplication constructor using a Func that returns a string.");
            else
                throw new PlatformNotSupportedException(
                    $"The default hardware ID system is not supported on {RuntimeInformation.OSDescription}.");

        var management = new ManagementClass("win32_processor");
        var managementObject = management.GetInstances();
        var cpuId = string.Empty;
        var sha256 = SHA256.Create();
        sha256.Initialize();
        foreach (var mo in managementObject)
        {
            var preHash = mo.Properties["processorID"].Value.ToString();
            cpuId = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(preHash)));
            break;
        }

        return cpuId;
    }
}