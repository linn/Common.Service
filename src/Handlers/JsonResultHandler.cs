namespace Linn.Common.Service.Handlers
{
    using System;

    using Linn.Common.Service.Serialization;

    public class JsonResultHandler<T> : ResultHandler<T>
    {
        public JsonResultHandler() : base("application/json", new JsonSerializer())
        {
        }

        public JsonResultHandler(string contentType) : base(contentType, new JsonSerializer())
        {
        }

        public override Func<T, string> GenerateLocation => r => string.Empty;
    }
}
