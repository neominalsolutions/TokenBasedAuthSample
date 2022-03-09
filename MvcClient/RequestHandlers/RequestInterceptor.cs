using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MvcClient.RequestHandlers
{
    public class RequestInterceptor: DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var accessToken =  await _httpContextAccessor.HttpContext.GetTokenAsync("AccessToken");

            if(accessToken != null)
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
