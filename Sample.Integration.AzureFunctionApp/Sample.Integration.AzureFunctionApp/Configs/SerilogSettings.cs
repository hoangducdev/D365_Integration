using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Configs
{
    public class SerilogSettings
    {
        public static SerilogSettings Instance { get; set; }
        public ICollection<WriteTo> WriteTo { get; set; }
        public static IConfiguration Configs { get; set; }
    }

    public class WriteTo
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }

    public class Args
    {
        public string OutputTemplate { get; set; }
        public string ConnectionString { get; set; }
        public string StorageContainerName { get; set; }
        public string StorageFileName { get; set; }
        public string WriteInBatches { get; set; }
        public string BlobSizeLimitBytes { get; set; }
        public string HostnameOrAddress { get; set; }
        public string Port { get; set; }
        public string TransportType { get; set; }
    }
}
