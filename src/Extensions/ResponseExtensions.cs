namespace Linn.Common.Service.Extensions
{
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;

    public static class ResponseExtensions
    {
        public static Task Negotiate<T>(
            this HttpResponse response,
            T model,
            CancellationToken cancellationToken = default)
        {
            var negotiators = response.HttpContext.RequestServices.GetServices<IResponseNegotiator>().ToList();
            IResponseNegotiator negotiator = null;

            MediaTypeHeaderValue.TryParseList(response.HttpContext.Request.Headers["Accept"], out var accept);
            if (accept != null)
            {
                var ordered = accept.OrderByDescending(x => x.Quality ?? 1);

                foreach (var acceptHeader in ordered)
                {
                    negotiator = negotiators.FirstOrDefault(x => x.CanHandle(acceptHeader));
                    if (negotiator != null)
                    {
                        break;
                    }
                }
            }

            if (negotiator == null)
            {
                negotiator = negotiators.First(
                    x => x.CanHandle(new MediaTypeHeaderValue("application/json")));
            }

            return negotiator.Handle(
                response.HttpContext.Request, response, model, cancellationToken);
        }

        public static Task FromStream(
            this HttpResponse response,
            Stream source,
            string contentType,
            ContentDisposition contentDisposition = null)
        {
            var contentLength = source.Length;

            response.Headers["Accept-Ranges"] = "bytes";

            response.ContentType = contentType;

            if (contentDisposition != null)
            {
                response.Headers["Content-Disposition"] = contentDisposition.ToString();
            }

            if (RangeHeaderValue.TryParse(
                    response.HttpContext.Request.Headers["Range"].ToString(), out var rangeHeader))
            {
                var rangeStart = rangeHeader.Ranges.First().From;
                var rangeEnd = rangeHeader.Ranges.First().To ?? contentLength - 1;

                if (!rangeStart.HasValue || rangeEnd > contentLength - 1)
                {
                    response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
                    return Task.CompletedTask;
                }

                response.Headers["Content-Range"] = $"bytes {rangeStart}-{rangeEnd}/{contentLength}";
                response.StatusCode = (int)HttpStatusCode.PartialContent;
                if (!source.CanSeek)
                {
                    throw new InvalidOperationException(
                        "Sending Range Responses requires a seekable stream eg. FileStream or MemoryStream");
                }

                source.Seek(rangeStart.Value, SeekOrigin.Begin);
                return StreamCopyOperation.CopyToAsync(
                    source,
                    response.Body,
                    rangeEnd - rangeStart.Value + 1,
                    65536,
                    response.HttpContext.RequestAborted);
            }

            return StreamCopyOperation.CopyToAsync(
                source, response.Body, default, 65536, response.HttpContext.RequestAborted);
        }
    }
}
