namespace Linn.Common.Service.Tests
{
    using System;
    using System.Net.Http;

    using Linn.Common.Service.Extensions;
    using Linn.Common.Service.Tests.Extensions;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;

    public static class TestClient
    {
        public static HttpClient With<T>(
            Action<IServiceCollection> serviceConfiguration, 
            params Func<RequestDelegate, RequestDelegate>[] middleWares) where T : IModule
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(
                        services =>
                            {
                                services.AddRouting();
                                services.Apply(serviceConfiguration);
                                services.AddSingleton<IResponseNegotiator, UniversalResponseNegotiator>();
                            })
                    .Configure(
                        app =>
                            {
                                app.UseRouting();
                                app.UseEndpoints(builder => { builder.MapEndpoints(); });
                            }));

            return server.CreateClient();
        }
    }
}
