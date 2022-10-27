using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ManageBastion2
{
    public static class SupportFunctions
    {
        public static string GetEnvironmentVariable(string name)
        {
            return                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        public static bool SetEnvironmentVariable(IConfigurationRoot config, string name, string value)
        {
            config[name] = value;
                          System.Environment.SetEnvironmentVariable(name, value);
            return (config[name] == value);
        }

        public static bool EnvironmentVariableExists( string name)
        {
            return !string.IsNullOrEmpty(name);
        }

       


    }
}
