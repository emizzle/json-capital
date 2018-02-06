using System;
namespace JSONCapital.Common.Helpers
{
    public class EnvironmentHelper
    {
        public static string GetEnvironmentName()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                              Environment.GetEnvironmentVariable("Hosting:Environment") ??
                              Environment.GetEnvironmentVariable("ASPNET_ENV");
        }
    }
}
