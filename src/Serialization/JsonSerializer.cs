namespace Linn.Common.Service.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings jsonSettings;

        public JsonSerializer()
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            this.jsonSettings = new JsonSerializerSettings { ContractResolver = contractResolver };
        }

        public string Serialize(object model)
        {
            return JsonConvert.SerializeObject(model, this.jsonSettings);
        }

        public T Deserialize<T>(string body)
        {
            return JsonConvert.DeserializeObject<T>(body, this.jsonSettings);
        }
    }
}
