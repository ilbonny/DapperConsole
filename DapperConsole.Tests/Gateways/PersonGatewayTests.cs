using System.Threading.Tasks;
using DapperConsole.Gateways;
using DapperConsole.Sql;
using NUnit.Framework;

namespace DapperConsole.Tests.Gateways
{
    [TestFixture]
    public class PersonGatewayTests
    {
        public PersonGateway _gateway;

        [SetUp]
        public void SetUp()
        {
            var config = new DbConfigurations
            {
                ConnectionString =
                    "Data Source=.\\sqlexpress;Initial Catalog=AdventureWorks;Integrated Security=True;Application Name=AdventureWorks"
            };
            _gateway = new PersonGateway(config);
        }

        [Test]
        public async Task Get()
        {
            var person = await _gateway.Get(1);
        }

        [Test]
        public async Task GetAll()
        {
            var persons = await _gateway.GetAll();
        }
    }
}
