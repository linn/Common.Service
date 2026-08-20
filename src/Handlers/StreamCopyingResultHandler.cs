namespace Linn.Common.Service.Handlers
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Facade;

    using Microsoft.AspNetCore.Http;

    public class StreamCopyingResultHandler : IHandler
    {
        public bool CanHandle(object model, string contentType)
        {
            return model is IResult<StreamResponse>;
        }

        public async Task Handle(
            HttpRequest req,
            HttpResponse res,
            object model,
            CancellationToken cancellationToken)
        {
            if (model is not IResult<StreamResponse> result)
            {
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }

            switch (result)
            {
                case SuccessResult<StreamResponse> success:
                    if (success.Data?.Stream == null)
                    {
                        res.StatusCode = StatusCodes.Status204NoContent;
                        return;
                    }

                    res.ContentType = success.Data.ContentType ?? "application/octet-stream";

                    if (!string.IsNullOrEmpty(success.Data.FileName))
                    {
                        var disposition = string.IsNullOrEmpty(success.Data.Disposition)
                            ? "inline"
                            : success.Data.Disposition;
                        res.Headers["Content-Disposition"] =
                            $"{disposition}; filename=\"{success.Data.FileName}\"";
                    }   

                    res.StatusCode = (int)HttpStatusCode.OK;

                    await using (success.Data.Stream)
                    {
                        if (success.Data.Stream.CanSeek)
                        {
                            success.Data.Stream.Position = 0;
                        }

                        await success.Data.Stream.CopyToAsync(res.Body, cancellationToken);
                    }
                    break;

                case BadRequestResult<StreamResponse> badRequest:
                    res.StatusCode = 400;
                    if (!string.IsNullOrEmpty(badRequest.Message))
                    {
                        await res.WriteAsync(badRequest.Message, cancellationToken);
                    }
                    break;

                case UnauthorisedResult<StreamResponse> unauthorised:
                    res.StatusCode = 401;
                    if (!string.IsNullOrEmpty(unauthorised.Message))
                    {
                        await res.WriteAsync(unauthorised.Message, cancellationToken);
                    }
                    break;

                case ForbiddenResult<StreamResponse> forbidden:
                    res.StatusCode = 403;
                    if (!string.IsNullOrEmpty(forbidden.Message))
                    {
                        await res.WriteAsync(forbidden.Message, cancellationToken);
                    }
                    break;

                case NotFoundResult<StreamResponse> _:
                    res.StatusCode = 404;
                    break;

                case ServerFailureResult<StreamResponse> _:
                    res.StatusCode = 500;
                    break;

                default:
                    res.StatusCode = 500;
                    await res.WriteAsync($"Unhandled result type: {result.GetType().Name}", cancellationToken);
                    break;
            }
        }
    }
}
