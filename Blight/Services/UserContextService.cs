using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Blight.Entieties;
using Blight.Interfaces;

namespace Blight.Services
{

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public int? GetUserId =>
            User is null ? null : (int?)int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

        public string? GetUserRole =>
            User is null ? null : User.FindFirst(c => c.Type == ClaimTypes.Role).Value;


        private string? DeterimineUserIP => _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        public string GetUserIP => DeterimineUserIP is null ? "IP not determined" : $"{DeterimineUserIP}";
    }
}
