namespace Linn.Common.Service.Handlers
{
    using System;

    using Linn.Common.Service.Serialization;

    public class CsvSerializingResultHandler<T> : SerializingResultHandler<T>
    {
        public CsvSerializingResultHandler() : base("text/csv", new CsvSerializer())
        {
        }

        public CsvSerializingResultHandler(string contentType) : base(contentType, new CsvSerializer())
        {
        }

        public override Func<T, string> GenerateLocation => r => string.Empty;
    }
}
