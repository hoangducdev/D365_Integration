using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Sample.Integration.AzureFunctionApp.Models
{
    public class ResultToken
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("ext_expires_in")]
        public string Ext_Expires_In { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
