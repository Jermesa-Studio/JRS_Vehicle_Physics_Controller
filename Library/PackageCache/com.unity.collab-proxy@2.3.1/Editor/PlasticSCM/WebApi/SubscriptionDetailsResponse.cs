using System;
using System.Collections.Generic;

using Unity.Plastic.Newtonsoft.Json;

namespace Unity.PlasticSCM.Editor.WebApi
{
    public class SubscriptionDetailsResponse
    {
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("subscriptionType")]
        public string ProductType { get; set; }

        [JsonProperty("subscriptionSource")]
        public string OrderSource { get; set; }

        [JsonProperty("genesisOrgId")]
        public string GenesisOrgId { get; set; }

        [JsonProperty("plasticOrganizationName")]
        public string OrganizationName { get; set; }

        [JsonProperty("readonlyStatus")]
        public string ReadonlyStatus { get; set; }

        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonProperty("isOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("extraData")]
        public Dictionary<string, object> ExtraData { get; set; }
    }
}