using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sample.Integration.AzureFunctionApp.Models
{
    public class ServiceBusInput
    {
        [JsonProperty("EventName")]
        public string EventName { get; set; }
    }
}
