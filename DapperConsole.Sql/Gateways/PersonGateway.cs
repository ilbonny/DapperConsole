using System.Collections.Generic;
using System.Threading.Tasks;
using DapperConsole.Models;
using Dapper;

namespace DapperConsole.Sql.Gateways
{
    public class PersonGateway : IRepository<Person>
    {
        private readonly DbConfigurations _configurations;

        public PersonGateway(DbConfigurations configurations)
        {
            _configurations = configurations;
        }

        public async Task<Person> Get(int id)
        {
            var itemData = await SqlDbConnectionHelper
                .Create(_configurations.ConnectionString)
                .ExecuteAsync(async conn => await conn.QuerySingleOrDefaultAsync<Person>(selectEntity, new { id }));

            return itemData;
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var itemData = await SqlDbConnectionHelper
                .Create(_configurations.ConnectionString)
                .ExecuteAsync(async conn => await conn.QueryAsync<Person>(selectEntities));

            return itemData;
        }

        public Task Insert(Person entity)
        {
            return SqlDbConnectionHelper
                .Create(_configurations.ConnectionString)
                .ExecuteAsync(conn =>
                    conn.ExecuteAsync(insertEntity, new
                    {
                        entity.FirstName,
                        entity.LastName
                    }));
        }

        public Task Update(Person entity)
        {
            return SqlDbConnectionHelper
                .Create(_configurations.ConnectionString)
                .ExecuteAsync(conn =>
                    conn.ExecuteAsync(updateEntity, new
                    {
                        entity.FirstName,
                        entity.LastName
                    }));
        }

        public Task Delete(Person entity)
        {
            return SqlDbConnectionHelper
                .Create(_configurations.ConnectionString)
                .ExecuteAsync(conn =>
                    conn.ExecuteAsync(deleteEntity, new { entity.Id }));
        }


        private const string selectEntity = @"
SELECT Id
      ,FirstName
      ,LastName
      ,ModifiedDate
FROM dbo.Persons 	
WHERE Id = @Id";

        private const string selectEntities = @"
SELECT TOP 1000 Id
      ,FirstName
      ,LastName
      ,ModifiedDate
FROM dbo.Persons";

        private const string insertEntity = @"
INSERT INTO dbo.Persons
           (FirstName
           ,LastName)
VALUES
           (@FirstName
           ,@LastName)
";

        private const string updateEntity = @"
UPDATE dbo.Persons
SET        (FirstName = @FirstName
           ,LastName = @LastName
           ,ModifiedDate = @ModifiedDate)
WHERE Id = @Id
";

        private const string deleteEntity = @"
DELETE FROM dbo.Persons
WHERE Id = @Id
";


    }
}
