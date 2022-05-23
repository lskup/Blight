using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IAdminPasswordService
    {
        void GenerateAndSavePasswordToDirectoryFromAppSettings();
        string ReadPasswordFromFile();


    }
}
