namespace Linn.Common.Service.Handlers
{
    using System;

    using Linn.Common.Service.Serialization;

    public class CsvResultHandler<T> : ResultHandler<T>
    {
        public CsvResultHandler() : base("text/csv", new CsvSerializer())
        {
        }

        public CsvResultHandler(string contentType) : base(contentType, new CsvSerializer())
        {
        }

        public override Func<T, string> GenerateLocation => r => string.Empty;
    }
}
