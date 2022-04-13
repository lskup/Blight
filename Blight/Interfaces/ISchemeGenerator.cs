using Blight.Entieties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface ISchemeGenerator
    {
        string GenerateJWT(User dto);
    }
}
