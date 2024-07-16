using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Acme.JsonTestConsoleApp.Data
{
    public class ToJsonTestDbMigrationService : ITransientDependency
    {
        public ILogger<ToJsonTestDbMigrationService> Logger { get; set; }

        private readonly IDataSeeder _dataSeeder;
        private readonly ToJsonTestEFCoreDbSchemaMigrator _dbSchemaMigrator;
        //private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentTenant _currentTenant;

        public ToJsonTestDbMigrationService(
            IDataSeeder dataSeeder,
            ToJsonTestEFCoreDbSchemaMigrator dbSchemaMigrator,
            //ITenantRepository tenantRepository,
            ICurrentTenant currentTenant)
        {
            _dataSeeder = dataSeeder;
            _dbSchemaMigrator = dbSchemaMigrator;
            //_tenantRepository = tenantRepository;
            _currentTenant = currentTenant;

            Logger = NullLogger<ToJsonTestDbMigrationService>.Instance;
        }

        public async Task MigrateAsync()
        {
            var initialMigrationAdded = AddInitialMigrationIfNotExist();

            if (initialMigrationAdded)
            {
                return;
            }

            Logger.LogInformation("Started database migrations...");

            await MigrateDatabaseSchemaAsync();

            Logger.LogInformation($"Successfully completed host database migrations.");

            var migratedDatabaseSchemas = new HashSet<string>();
            

            Logger.LogInformation("Successfully completed all database migrations.");
            Logger.LogInformation("You can safely end this process...");
        }

        private async Task MigrateDatabaseSchemaAsync()
        {
            Logger.LogInformation($"Migrating schema for  database...");
            await _dbSchemaMigrator.MigrateAsync();
        }

   

        private bool AddInitialMigrationIfNotExist()
        {
            try
            {
                if (!DbMigrationsProjectExists())
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                if (!MigrationsFolderExists())
                {
                    AddInitialMigration();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning("Couldn't determinate if any migrations exist : " + e.Message);
                return false;
            }
        }

        private bool DbMigrationsProjectExists()
        {
            return Directory.Exists(GetEntityFrameworkCoreProjectFolderPath());
        }

        private bool MigrationsFolderExists()
        {
            var dbMigrationsProjectFolder = GetEntityFrameworkCoreProjectFolderPath();

            return Directory.Exists(Path.Combine(dbMigrationsProjectFolder, "Migrations"));
        }

        private void AddInitialMigration()
        {
            Logger.LogInformation("Creating initial migration...");

            string argumentPrefix;
            string fileName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                argumentPrefix = "-c";
                fileName = "/bin/bash";
            }
            else
            {
                argumentPrefix = "/C";
                fileName = "cmd.exe";
            }

            var procStartInfo = new ProcessStartInfo(fileName,
                $"{argumentPrefix} \"abp create-migration-and-run-migrator \"{GetEntityFrameworkCoreProjectFolderPath()}\" --nolayers\""
            );

            try
            {
                Process.Start(procStartInfo);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't run ABP CLI...");
            }
        }

        private string GetEntityFrameworkCoreProjectFolderPath()
        {
            var slnDirectoryPath = GetSolutionDirectoryPath();

            if (slnDirectoryPath == null)
            {
                throw new Exception("Solution folder not found!");
            }

            return Path.Combine(slnDirectoryPath, "Mobest.ToJsonTest");
        }

        private string? GetSolutionDirectoryPath()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (currentDirectory != null && Directory.GetParent(currentDirectory.FullName) != null)
            {
                currentDirectory = Directory.GetParent(currentDirectory.FullName);

                if (currentDirectory != null && Directory.GetFiles(currentDirectory.FullName).FirstOrDefault(f => f.EndsWith(".sln")) != null)
                {
                    return currentDirectory.FullName;
                }
            }

            return null;
        }
    }

}
