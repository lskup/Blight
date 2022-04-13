using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Exceptions;

namespace Blight.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(NotFoundException notFoundException)
            {
                _logger.LogError(notFoundException, notFoundException.Message);

                context.Response.StatusCode = 404;
                context.Response.ContentType = "plain/text";
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (BadRequestException badRequestException)
            {
                _logger.LogError(badRequestException, badRequestException.Message);

                context.Response.StatusCode = 400;
                context.Response.ContentType = "plain/text";
                await context.Response.WriteAsync(badRequestException.Message);
            }


            catch (DataBaseException dataBaseException)
            {
                _logger.LogError(dataBaseException, dataBaseException.Message);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "plain/text";
                await context.Response.WriteAsync(dataBaseException.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "plain/text";
                await context.Response.WriteAsync("Sorry,not handled error");

                //context.Response.ContentType = "application/json";
                //await context.Response.WriteAsync(@"{""information"": ""something went wrong""}");
            }

        }
    }
}
