using Microsoft.Extensions.Configuration;
using Sample.Integration.AzureFunctionApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Configs
{
    public class AppSettings : IAppSettings
    {
        public static IConfiguration Configs { get; set; }
        public string AppSecretD365 { get; set; }
        public string D365BasePath { get; set; }
        public string AppKeyD365 { get; set; }
        public string D365AppSecret { get; set; }
        public string D365AppId { get; set; }
        public string DataverseConnectionString { get; set; }
        public string BlobUrl { get; set; }
        public string TenantID { get; set; }
    }
}
