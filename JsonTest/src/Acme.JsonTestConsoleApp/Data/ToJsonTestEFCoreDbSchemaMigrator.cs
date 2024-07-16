using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.JsonTestConsoleApp.Data
{
    public class ToJsonTestEFCoreDbSchemaMigrator : ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public ToJsonTestEFCoreDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            await _serviceProvider
                .GetRequiredService<ToJsonTestDbContext>()
                .Database
                .MigrateAsync();
        }
    }

}
