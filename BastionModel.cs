using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageBastion2
{
    
 public static class BastionModel
    {
        public static bool Exists;
        public static string ProvisioningState;
        public static string SPN ;
        public static string SPNKEY;
        public static string TenantID ;
        public static string BastionName;
        public static string ResourceGroup;
        public static string ManagedResourceGroup;
        public static string PublicIP;
        public static string PublicIPId;
        public static string VNet;
        public static string VNetId;
        public static string Location;
        public static string SubscriptionID;
        public static string BastionSku;
        public static string VNetSetTrue;
        public static string FunctionName;
        public static string NewAppSettings;
        public static string OldSettings;

        public static string OnTimerSchedule;
        public static string OffTimerSchedule;

        public static string OnTimerScheduleDisabled;
        public static string OffTimerScheduleDisabled;


        public static string PublicIdSetTrue;

        public static string Response { get; set; }

        public static string Status { get; set; }
        


        public static string AccessToken { get; internal set; }
    }


}
