using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {
        readonly string _connectionString;
        public BirdRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("LinenAndBird");
        }

        internal IEnumerable<Bird> GetAll()
        {

            using var db = new SqlConnection(_connectionString);

            var birds = db.Query<Bird>(@"select * from Birds");

            var accessoriesQuery = @"select * from BirdAccessories";

            var accessories = db.Query<BirdAccessory>(accessoriesQuery);

            foreach (var bird in birds)
            {
                bird.Accessories = accessories.Where(ac => ac.BirdId == bird.Id);
            }



            return birds;

        }

        internal void Remove(Guid id)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"Delete From Birds Where Id = @id";

            db.Execute(sql, new { id });

        }

        internal Bird Update(Guid id, Bird bird)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"update Birds Set
                                     Color = @color, Name = @name, Type = @type, Size = @size
                                        output inserted.*
                                      Where id = @id";

            bird.Id = id;
            var updatedBird = db.QuerySingleOrDefault<Bird>(sql, bird);
            return updatedBird;
        }

        internal void Add(Bird newBird)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"insert into birds(Type,Color,Size,Name)
                                    output inserted.Id
                                    values(@Type, @Color, @Size, @Name)";

            var id = db.ExecuteScalar<Guid>(sql, newBird);
            newBird.Id = id;

        }

        internal Bird GetById(Guid birdId)
        {
            using var db = new SqlConnection(_connectionString);

            var birdQuery = @"select *
                      from Birds
                      where id = @id";

            var bird = db.QueryFirstOrDefault<Bird>(birdQuery, new {id = birdId} );

            if (bird == null) return null;

            var accessoriesQuery = @"select * from BirdAccessories where birdId = @birdId";

            var accessories = db.Query<BirdAccessory>(accessoriesQuery, new { birdId });

            bird.Accessories = accessories;

            return bird;
        }
    }
}
