namespace Linn.Common.Service.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Facade;
    using Linn.Common.Service.Serialization;

    using Microsoft.AspNetCore.Http;

    internal sealed class ResultResponseWriter<T>
    {
        private readonly string contentType;
        private readonly ISerializer serialiser;
        private readonly Func<T, string> locationBuilder;

        public ResultResponseWriter(
            string contentType,
            ISerializer serialiser,
            Func<T, string> locationBuilder = null)
        {
            this.contentType = contentType;
            this.serialiser = serialiser;
            this.locationBuilder = locationBuilder;
        }

        public async Task WriteAsync(
            HttpResponse res,
            IResult<T> result,
            CancellationToken cancellationToken)
        {
            res.ContentType = this.contentType;

            switch (result)
            {
                case SuccessResult<T> r:
                    res.StatusCode = 200;
                    await res.WriteAsync(
                        this.serialiser.Serialize(r.Data),
                        cancellationToken);
                    break;

                case UnauthorisedResult<T> r:
                    res.StatusCode = 401;
                    await res.WriteAsync(
                        this.SerializeOptional(r.Message, r.Body),
                        cancellationToken);
                    break;

                case NotFoundResult<T> _:
                    res.StatusCode = 404;
                    break;

                case CreatedResult<T> r:
                    res.StatusCode = 201;
                   
                    res.Headers["Location"] =
                            this.locationBuilder(r.Data);

                    await res.WriteAsync(
                        this.serialiser.Serialize(r.Data ?? new object()),
                        cancellationToken);
                    break;

                case BadRequestResult<T> r:
                    res.StatusCode = 400;
                    await res.WriteAsync(
                        this.SerializeOptional(r.Message, r.ErrorData),
                        cancellationToken);
                    break;

                case ServerFailureResult<T> _:
                    res.StatusCode = 500;
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unhandled result type {result.GetType().Name}");
            }
        }

        private string SerializeOptional(object? a, object? b)
        {
            if (a != null)
            {
                return this.serialiser.Serialize(a);
            }

            if (b != null)
            {
                return this.serialiser.Serialize(b);
            }

            return string.Empty;
        }
    }
}
