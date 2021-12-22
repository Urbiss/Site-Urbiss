using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Urbiss.API.Wrappers;
using Urbiss.Domain.Exceptions;

namespace Urbiss.API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                response.StatusCode = error switch
                {
                    ApiException => (int)HttpStatusCode.BadRequest,// custom application error
                    /*                    case Urbiss.API.Exceptions.ValidationException e:
                                            // custom application error
                                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                                            responseModel.Errors = e.Errors;
                                            break;*/
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,// not found error
                    _ => (int)HttpStatusCode.InternalServerError,// unhandled error
                };
                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }
        }
    }
}
