using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Volo.Abp;

namespace EventHub;

public static class AssemblyInfoHelper
{
    private const string AssemblyInfoCacheKey = "AssemblyInfoCacheKey";
    private readonly static Dictionary<string, AssemblyInfo> AssemblyInfoCache = new();

    public static AssemblyInfo Get()
    {
        if (AssemblyInfoCache.TryGetValue(AssemblyInfoCacheKey, out var assemblyInfo))
        {
            return assemblyInfo;
        }

        var abpCorePackageVersion = GetAbpCorePackageVersion();
        var applicationAssembly = Assembly.GetEntryAssembly();
        var applicationVersion = applicationAssembly?.GetFileVersion();
        var modificationDate = applicationAssembly?.Location == null ? DateTime.Now : File.GetLastWriteTime(applicationAssembly.Location);

        assemblyInfo = new AssemblyInfo(abpCorePackageVersion, applicationVersion, modificationDate);

        AssemblyInfoCache[AssemblyInfoCacheKey] = assemblyInfo;

        return assemblyInfo;
    }

    private static string GetAbpCorePackageVersion()
    {
        var abpCoreAssembly = Assembly.GetAssembly(typeof(AbpApplicationBase));

        return abpCoreAssembly == null 
            ? string.Empty 
            : abpCoreAssembly.GetFileVersion();
    }

    public class AssemblyInfo
    {
        public string AbpCoreVersion { get; }

        public string Version { get; }

        public DateTime ModificationDate { get; }

        public AssemblyInfo(string abpCoreVersion, string version, DateTime modificationDate)
        {
            AbpCoreVersion = abpCoreVersion;
            Version = version;
            ModificationDate = modificationDate;
        }
    }
}