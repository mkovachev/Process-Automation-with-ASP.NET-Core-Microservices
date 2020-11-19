namespace CarRentalSystem.Infrastructure
{
    using System;
    using System.Net;
    using System.Net.Http.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using Services.Identity;

    using static InfrastructureConstants;

    public static class HttpClientBuilderExtensions
    {
        public static Random jitterer = new Random();
        public static void WithConfiguration(
            this IHttpClientBuilder httpClientBuilder,
            string baseAddress)
            => httpClientBuilder
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.BaseAddress = new Uri(baseAddress);

                    var requestServices = serviceProvider
                        .GetService<IHttpContextAccessor>()
                        ?.HttpContext
                        .RequestServices;

                    var currentToken = requestServices
                        ?.GetService<ICurrentTokenService>()
                        ?.Get();

                    if (currentToken == null)
                    {
                        return;
                    }

                    var authorizationHeader = new AuthenticationHeaderValue(AuthorizationHeaderValuePrefix, currentToken);
                    client.DefaultRequestHeaders.Authorization = authorizationHeader;
                })
                .AddTransientHttpErrorPolicy(policy => policy
                    .OrResult(result => result.StatusCode == HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(5, retry => TimeSpan.FromSeconds(Math.Pow(2, retry))
                                                  + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
        //            .AddTransientHttpErrorPolicy(policy => policy
        //                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
    }
}
