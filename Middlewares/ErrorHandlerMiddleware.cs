using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TimeOffTracker.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                await SendResult(context, ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        private async Task SendResult(HttpContext context, Exception ex, int statusCode)
        {
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new
            {
                ErrorMessage = ex.Message,
                StatusCode = statusCode
            });
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(result);
        }

        private void ShowError(Exception ex)
        {
            var type = ex.GetType();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {type}");
            Console.ResetColor();

            Console.WriteLine($"Message: {ex.Message}");
            var sf = new StackTrace(ex, true).GetFrame(0);
            Console.WriteLine("File: {0}", sf?.GetFileName());
            Console.WriteLine("Line Number: {0}", sf?.GetFileLineNumber());
            Console.WriteLine();
        }
    }
}