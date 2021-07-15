using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace TimeOffTracker.Middlewares
{
    public class RequestHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            const string delimiter =
                "____________________________________________________________________________________________________";
            Console.WriteLine(delimiter);


            Console.WriteLine("Request");
            var method = context.Request.Method;
            var url = context.Request.GetDisplayUrl();
            var protocol = context.Request.Protocol;
            Console.WriteLine($"{method} {url} {protocol}");

            foreach (var (key, value) in context.Request.Headers)
            {
                Console.WriteLine($"\t{key}: {value}");
            }

            const int size = 1024;
            using (var reader
                = new StreamReader(context.Request.Body, Encoding.UTF8, true, size, true))
            {
                var bodyStr = await reader.ReadToEndAsync();
                if (bodyStr.Length is > 0)
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

                context.Request.Body.Position = 0;
            }

            Console.WriteLine();
            await _next(context);
        }
    }
}