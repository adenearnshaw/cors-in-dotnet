using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace WebApiCorsHandler.Api.Providers
{
    public class CustomCorsPolicy : ICorsPolicyProvider
    {
        private readonly IReadOnlyCollection<string> corsExceptions;


        public CustomCorsPolicy()
        {
            var settingCollection = (NameValueCollection)ConfigurationManager.GetSection("corsSection");
            corsExceptions = settingCollection?.AllKeys?.ToList() ?? new List<string>();
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var corsRequestContext = request.GetCorsRequestContext();
            var origin = corsRequestContext.Origin;
            var allowed = false;

#if !DEBUG
            var matches = corsExceptions.Where(corsEntry => origin.EndsWith(corsEntry));
            allowed = matches.Any();
#else
            allowed = true;
#endif

            if (allowed)
            {
                // Grant CORS request
                var policy = new CorsPolicy
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                };

                policy.Origins.Add(origin);
                policy.ExposedHeaders.Add("AccessToken");

                return Task.FromResult(policy);
            }

            // Reject CORS request
            return null;
        }
    }
}