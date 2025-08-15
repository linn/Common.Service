namespace Linn.Common.Service.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Facade;
    using Linn.Common.Service.Serialization;

    using Microsoft.AspNetCore.Http;

    public delegate Task ResponseAction(HttpResponse res, CancellationToken cancellationToken);

    public class ResultVisitor<T> : IResultVisitor<T, ResponseAction>
    {
        private readonly string contentType;
        private readonly ISerializer serialiser;
        private readonly Func<T, string> locationBuilder;

        public ResultVisitor(string contentType, ISerializer serialiser, Func<T, string> locationBuilder = null)
        {
            this.contentType = contentType;
            this.serialiser = serialiser;
            this.locationBuilder = locationBuilder;
        }

        public ResponseAction Visit(SuccessResult<T> result)
        {
            return async (res, cancellationToken) =>
            {
                res.StatusCode = 200;
                res.ContentType = this.contentType;
                await res.WriteAsync(this.serialiser.Serialize(result.Data), cancellationToken);
            };
        }

        public ResponseAction Visit(UnauthorisedResult<T> result)
        {
            return (res, cancellationToken) =>
            {
                res.StatusCode = 401;
                res.ContentType = this.contentType;

                return Task.CompletedTask;
            };
        }

        public ResponseAction Visit(NotFoundResult<T> result)
        {
            return (res, cancellationToken) =>
            {
                res.StatusCode = 404;
                res.ContentType = this.contentType;

                return Task.CompletedTask;
            };
        }

        public ResponseAction Visit(CreatedResult<T> result)
        {
            var location = this.locationBuilder == null
                ? null
                : this.locationBuilder(result.Data);

            return async (res, cancellationToken) =>
            {
                res.Headers["Location"] = location;
                res.StatusCode = 201;
                res.ContentType = this.contentType;
                await res.WriteAsync(this.serialiser.Serialize(result.Data), cancellationToken);
            };
        }

        public ResponseAction Visit(BadRequestResult<T> result)
        {
            return async (res, cancellationToken) =>
            {
                res.StatusCode = 400;
                res.ContentType = this.contentType;

                string payload;

                if (result.Message != null)
                {
                    payload = this.serialiser.Serialize(result.Message);
                }
                else if (result.ErrorData != null)
                {
                    payload = this.serialiser.Serialize(result.ErrorData);
                }
                else
                {
                    payload = string.Empty;
                }

                await res.WriteAsync(payload, cancellationToken);
            };
        }

        public ResponseAction Visit(ServerFailureResult<T> result)
        {
            return (res, cancellationToken) =>
            {
                res.StatusCode = 500;
                res.ContentType = this.contentType;

                return Task.CompletedTask;
            };
        }
    }
}
