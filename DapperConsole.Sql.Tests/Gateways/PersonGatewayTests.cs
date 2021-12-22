using System.Threading.Tasks;
using DapperConsole.Sql.Gateways;
using DapperConsole.Sql.Tests.Database;
using NUnit.Framework;
using Dapper;
using DapperConsole.Models;
using FluentAssertions;

namespace DapperConsole.Sql.Tests.Gateways
{
    [TestFixture]
    public class PersonGatewayTests : DatabaseHelper
    {
        public PersonGateway _gateway;

        private string InsertPerson = @"INSERT INTO dbo.Persons (FirstName, LastName) VALUES (@FirstName, @LastName)";
        private string SelectPerson = @"SELECT Id, FirstName, LastName, ModifiedDate FROM dbo.Persons";

        [SetUp]
        public void SetUp()
        {
            _gateway = new PersonGateway(Configurations);
        }

        [TearDown]
        public void TearDown()
        {
            SqlDbConnectionHelper.Create(ConnectionString)
                .Execute(conn =>
                    {
                        conn.Execute("DELETE [dbo].[Persons]");
                        conn.Execute("DBCC CHECKIDENT ('[dbo].[Persons]', RESEED, 0)");
                    }
                );
        }

        [Test]
        public async Task Get()
        {
            var expected = new Person
            {
                Id = 1,
                FirstName = "John",
                LastName = "Snow"
            };
            
            SqlDbConnectionHelper.Create(ConnectionString)
                .Execute(conn => conn.Execute(InsertPerson, new {  expected.FirstName, expected .LastName}));
            
            var person = await _gateway.Get(expected.Id);

            person.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.ModifiedDate));
        }

        [Test]
        public async Task GetAll()
        {
            var expected = new[]
            {
                new Person
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Snow"
                },
                new Person
                {
                    Id = 2,
                    FirstName = "James",
                    LastName = "Lannister"
                }
            };

            foreach (var person in expected)
                SqlDbConnectionHelper.Create(ConnectionString)
                    .Execute(conn => conn.Execute(InsertPerson, new { person.FirstName, person.LastName }));

            var persons = await _gateway.GetAll();
            persons.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.ModifiedDate));
        }

        [Test]
        public async Task Insert()
        {
            var expected = new Person
            {
                Id = 1,
                FirstName = "John",
                LastName = "Snow"
            };
           
            await _gateway.Insert(expected);

            var person = SqlDbConnectionHelper.Create(ConnectionString)
                .Execute(conn => conn.QuerySingleOrDefault<Person>(SelectPerson, new { Id =1 }));

            person.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.ModifiedDate));
        }

        [Test]
        public async Task Delete()
        {
            var expected = new Person
            {
                Id = 1,
                FirstName = "John",
                LastName = "Snow"
            };

            SqlDbConnectionHelper.Create(ConnectionString)
                .Execute(conn => conn.Execute(InsertPerson, new { expected.FirstName, expected.LastName }));

            await _gateway.Delete(expected);

            var person = SqlDbConnectionHelper.Create(ConnectionString)
                .Execute(conn => conn.QuerySingleOrDefault<Person>(SelectPerson, new { Id = 1 }));

            person.Should().BeNull();
        }
    }
}
