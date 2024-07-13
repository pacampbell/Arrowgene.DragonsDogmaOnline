using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using Arrowgene.Ddon.Database.Migrations;
using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Database.Sql;
using Arrowgene.Logging;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Arrowgene.Ddon.Database
{
    public static class DdonDatabaseBuilder
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(DdonDatabaseBuilder));
        private const string DefaultSchemaFile = "Script/schema_sqlite.sql";

        public static IDatabase Build(DatabaseSetting settings)
        {
            Enum.TryParse(settings.Type, true, out DatabaseType dbType);

            IDatabase database = dbType switch
            {
                DatabaseType.SQLite => BuildSqLite(settings.DatabaseFolder, settings.WipeOnStartup),
                DatabaseType.PostgreSQL => BuildPostgres(settings.DatabaseFolder, settings.Host, settings.User, settings.Password, settings.Database, settings.WipeOnStartup),
                DatabaseType.MariaDb => BuildMariaDB(settings.DatabaseFolder, settings.Host, settings.User, settings.Password, settings.Database, settings.WipeOnStartup),
                _ => throw new ArgumentOutOfRangeException($"Unknown database type '{settings.Type}' encountered!")
            };

            if (database == null)
            {
                Logger.Error("Database could not be created, exiting...");
                Environment.Exit(1);
            }
            else
            {
                Logger.Info($"Database of type '${dbType.ToString()}' has been created.");
                Logger.Info($"Database path: {settings.DatabaseFolder}");
            }

            return database;
        }

        public static DdonSqLiteDb BuildSqLite(string databaseFolder, bool wipeOnStartup)
        {
            string sqLitePath = Path.Combine(databaseFolder, $"db.v{DdonSqLiteDb.Version}.sqlite");
            DdonSqLiteDb db = new DdonSqLiteDb(sqLitePath, wipeOnStartup);
            
            if (!db.CreateDatabase())
            {
                return null;
            }

            return db;
        }

        public static DdonPostgresDb BuildPostgres(string databaseFolder, string host, string user, string password, string database, bool wipeOnStartup)
        {
            DdonPostgresDb db = new DdonPostgresDb(host, user, password, database, wipeOnStartup);
            if (!db.CreateDatabase())
            {
                return null;
            }

            return db;
        }

        public static DdonMariaDb BuildMariaDB(string databaseFolder, string host, string user, string password, string database, bool wipeOnStartup)
        {
            DdonMariaDb db = new DdonMariaDb(host, user, password, database, wipeOnStartup);
            if (!db.CreateDatabase())
            {
                return null;
            }

            return db;
        }

        /// <summary>
        /// Update the database
        /// </summary>
        public static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }

        /// <summary>
        /// Downgrade the database
        /// </summary>
        public static void DowngradeDatabase(IServiceProvider serviceProvider, long version)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(version);
        }
    }
}
