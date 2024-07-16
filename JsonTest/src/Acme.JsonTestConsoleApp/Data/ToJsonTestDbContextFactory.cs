using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.JsonTestConsoleApp.Data
{
    public class ToJsonTestDbContextFactory : IDesignTimeDbContextFactory<ToJsonTestDbContext>
    {
        public ToJsonTestDbContext CreateDbContext(string[] args)
        {

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<ToJsonTestDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new ToJsonTestDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
