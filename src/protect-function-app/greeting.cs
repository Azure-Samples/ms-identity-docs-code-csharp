/*
This is an Azure Function that responds at GET /api/greeting.

Using the built-in authentication and authorization capabilities (sometimes
referred to as "Easy Auth") of Azure Functions, offloads part of the authentication
and authorization process by ensuring that every request to this Azure Function has
an access token. That access token has had its signature, issuer (iss), expiry
dates (exp, nbf), and audience (aud) validated. This means all that is left to
perform is any per-function authorization related to your application.
*/

using System;
using System.IO;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api
{
    public static class Greeting
    {
        private static readonly List<String> s_acceptedRoles = new() { "Greeting.Read" };

        /*
        Because Easy Auth has already validated the signature, the validation is not
        performed again, but instead the token is is being decoded only to get access
        to its contained scopes claim.
        */
        [FunctionName("greeting")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            ClaimsPrincipal principal,
            ILogger log)
        {
            // This API endpoint requires the "Greeting.Read" scope to be present, if it is
            // not, then reject the request with a 403.
            if (!principal.Claims.Select(c => c.Value).Intersect(s_acceptedRoles).Any())
            {
                return new ObjectResult("Forbidden") { StatusCode = 403};
            }

            // Authentication is complete, process request.
            return new OkObjectResult("Hello, world. You were able to access this because you provided a valid access token with the Greeting.Read scope as a claim.");
        }
    }
}
