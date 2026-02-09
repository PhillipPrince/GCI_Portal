using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ConnectionStringProvider
    {
        private readonly IConfiguration _configuration;
        private static readonly Security Security = new();


        public ConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> BuildConnectionString()
        {
            return await Security.ConfigureConn(
                _configuration["GCI_ConnectionStrings:DBConn"],
                _configuration["GCI_ConnectionStrings:CSV"],
                _configuration["GCI_ConnectionStrings:CDB"],
                _configuration["GCI_ConnectionStrings:CUI"],
                _configuration["GCI_ConnectionStrings:CPW"]
            );
        }
    }
}
