using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;

namespace servartur.Middleware;

public class FirebaseAuthMiddleware : IMiddleware
{
    public FirebaseAuthMiddleware()
    { }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            await next.Invoke(context);
        }
        catch (FirebaseAuthException) //thrown by VerifyIdTokenAsync
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}