using Sample.Integration.AzureFunctionApp.Configs;

namespace Sample.Integration.AzureFunctionApp.Interfaces
{
    public interface IAppSettings
    {
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
