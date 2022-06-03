using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Blight.Entieties;
using Blight.Interfaces;

namespace Blight.Interfaces
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        int? GetUserId { get; }
        string GetUserIP { get; }
        string? GetUserRole { get; }
    }
}
