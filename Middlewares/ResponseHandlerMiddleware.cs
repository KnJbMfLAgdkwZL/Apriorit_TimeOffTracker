using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TimeOffTracker.Middlewares
{
    public class ResponseHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var bodyStr = "";
            await using (var swapStream = new MemoryStream())
            {
                var originalResponseBody = context.Response.Body;
                context.Response.Body = swapStream;

                await _next(context);

                swapStream.Seek(0, SeekOrigin.Begin);
                bodyStr = new StreamReader(swapStream).ReadToEnd();
                swapStream.Seek(0, SeekOrigin.Begin);
                await swapStream.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;
            }

            Console.WriteLine("Response");
            var statusCode = context.Response.StatusCode;
            Console.WriteLine($"StatusCode: {statusCode}");

            foreach (var (key, value) in context.Response.Headers)
            {
                Console.WriteLine($"\t{key}: {value}");
            }

            if (bodyStr.Length > 0)
            {
                string str;
                if (bodyStr.Length > 1000)
                {
                    str = bodyStr[..1000] + "....";
                }
                else
                {
                    str = bodyStr;
                }

                Console.WriteLine($"Body: {str}");
            }

            Console.WriteLine();
        }
    }
}