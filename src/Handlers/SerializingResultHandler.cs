namespace Linn.Common.Service.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Facade;
    using Linn.Common.Service.Serialization;

    using Microsoft.AspNetCore.Http;

    public abstract class SerializingResultHandler<T> : IHandler
    {
        private readonly string contentType;
        private readonly ISerializer serializer;

        protected SerializingResultHandler(string contentType, ISerializer serializer)
        {
            this.contentType = contentType;
            this.serializer = serializer;
        }

        public abstract Func<T, string> GenerateLocation { get; }

        public bool CanHandle(object model, string requestedContentType)
        {
            return model is IResult<T>
                   && requestedContentType.IndexOf(
                       this.contentType,
                       StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        public async Task Handle(
            HttpRequest req,
            HttpResponse res,
            object model,
            CancellationToken cancellationToken)
        {
            var result = (IResult<T>)model;

            var writer = new ResultResponseWriter<T>(
                this.contentType,
                this.serializer,
                this.GenerateLocation);

            await writer.WriteAsync(res, result, cancellationToken);
        }
    }
}
