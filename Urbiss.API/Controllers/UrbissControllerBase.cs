using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace Urbiss.API.Controllers
{
    public abstract class UrbissControllerBase<T> : ControllerBase where T : ControllerBase
    {
        private ILogger<T> _logger;

        protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();

        protected int CurrentUserId { get => Convert.ToInt32(HttpContext.User.FindFirstValue("uid")); }
    }
}
