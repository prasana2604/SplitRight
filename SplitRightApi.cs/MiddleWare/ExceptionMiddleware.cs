using SplitRightApi.cs.Exceptions;
using System.Net;
using System.Text.Json;

namespace SplitRightApi.cs.MiddleWare
{
    public class ExceptionMiddleware
    {
            private readonly RequestDelegate _next;

            public ExceptionMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);}
                }

        public static async Task HandleExceptionAsync(HttpContext context,Exception ex)
        {

            context.Response.ContentType = "application/json";
            var StatusCodes = ex switch {

                NotFoundException => HttpStatusCode.NotFound,
                UnAuthorizedException => HttpStatusCode.Forbidden,
                ValidationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)StatusCodes;

            var response = new
            {
                statusCode = (int)StatusCodes,
                Message = ex.Message,
                Error = StatusCodes.ToString(),
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);


        }

        
            }
        }
    

