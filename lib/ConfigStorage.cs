using System;
using System.Text;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Calculon.Types
{
    public class Config
    {
        public Config()
        {
            configuration = new ConfigurationBuilder()
                .AddIniFile("calculon.ini", optional: true, reloadOnChange: true)
                .Build();

        }
        private IConfiguration configuration;

        public bool AllowFilesystemWrites
        {
            get
            {
                string value = configuration["AllowFilesystemWrites"];
                if (value is null)
                {
                    return false;
                }
                try
                {
                    bool answer = bool.Parse(value);
                    return answer;
                } catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}