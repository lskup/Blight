using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Blight.Interfaces;

namespace Blight.Services
{
    public class AdminPasswordService:IAdminPasswordService
    {
        Random random = new Random();
        IConfiguration _root;
        private string _filePath;

        public AdminPasswordService()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", false, true);
            IConfigurationRoot _root = builder.Build();
            _filePath = (string)_root.GetValue(typeof(string), "AdminPasswordPath");
        }

        public string GeneratePassword()
        {
            string password = string.Empty;

            for (int i = 0; i < 10; i++)
            {
                var randomInt = random.Next(33, 126);
                char randomChar = (char)randomInt;
                password += randomChar;
            }

            return password;
        }
        public void GenerateAndSavePasswordToDirectoryFromAppSettings()
        {

            var password = GeneratePassword();
            File.WriteAllText(_filePath, password);
        }
        public string ReadPasswordFromFile()
        {
            return File.ReadAllText(_filePath);
        }

    }
}
