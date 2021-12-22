using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Dapper;

namespace DapperConsole.Sql.Tests.Database
{
    [TestFixture]
    public class DatabaseHelper
    {
        protected DbConfigurations Configurations;
        
        public static readonly string DatabaseName = $"DapperConsoleTest{Guid.NewGuid():N}";
        public const string MasterDbConnString = @"Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
        public static readonly string ConnectionString = $@"Data Source=.\SQLEXPRESS;Initial Catalog={DatabaseName};Integrated Security=True";


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SqlDbConnectionHelper.Create(MasterDbConnString)
                .Execute(conn => conn.Execute($"CREATE DATABASE {DatabaseName};"));

            Configurations = new DbConfigurations
            {
                ConnectionString = ConnectionString
            };

            ExecuteMigrations();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            SqlDbConnectionHelper.Create(MasterDbConnString)
                .Execute(conn => conn.Execute($@"
                    ALTER DATABASE {DatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE {DatabaseName};"));
        }

        public void ExecuteMigrations()
        {
            var scriptsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Database\\Scripts");
            foreach (var file in Directory.GetFiles(scriptsPath))
            {
                var sql = File.ReadAllText(file);
                SqlDbConnectionHelper.Create(ConnectionString)
                    .Execute(conn => conn.Execute(sql));
            }
        }
    }
}
