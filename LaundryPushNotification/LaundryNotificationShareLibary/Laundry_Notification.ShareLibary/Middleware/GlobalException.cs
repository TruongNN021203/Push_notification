using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Http;
using Laundry_Notification.ShareLibary.Logs;
using Microsoft.AspNetCore.Mvc;

namespace Laundry_Notification.ShareLibary.Middleware
{
    public class GlobalException
    {

        private readonly RequestDelegate next;
        public GlobalException(RequestDelegate _next)
        {
            this.next = _next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare default variables
            string message = "Sorry, an internal server error occurred. Please try again later.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";


            try
            {
                await next(context);

                if (!context.Response.HasStarted)
                {
                    //check if exception is too many request //429 status code.
                    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                    {
                        title = "Warning";
                        message = "Too many requests";
                        statusCode = (int)StatusCodes.Status429TooManyRequests;
                        await ModifyHeader(context, title, message, statusCode);

                    }


                    //if response is forbidden //403

                    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                        title = "Access Denied";
                        message = "You are not allowed to access this resource.";
                        statusCode = (int)StatusCodes.Status403Forbidden;

                        await ModifyHeader(context, title, message, statusCode);
                    }
                }

            }
            catch (Exception ex)
            {
                //Log original exceptions/file, debugger, console
                LogException.LogExceptions(ex);

                //check if exception is timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "REquest timeout .... try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }


                // if none of the exceptions then do the default(at declare)
                // if exceptions is caught 
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();//clear old response
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
        }
        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //display scary-frr message to client
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var problem = new ProblemDetails
            {
                Title = title,
                Detail = message,
                Status = statusCode
            };

            var response = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(response);

            return;
        }
    }
}
