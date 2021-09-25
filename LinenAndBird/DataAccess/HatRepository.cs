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
    public class HatRepository
    {
        readonly string _connectionString;
        public HatRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("LinenAndBird");
        }
        internal Hat GetById(Guid hatId)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"select *
                                    from Hats
                                     where id = @id";

            var hat = db.QueryFirstOrDefault<Hat>(sql, new { id = hatId });

            return hat;
        }

        internal IEnumerable<Hat> GetByStyle(HatStyle style)
        {
            using var db = new SqlConnection(_connectionString);

           var sql= @"select *
                                    from Hats
                                     where style = @style";

            var hat = db.Query<Hat>(sql, new { style });

            return hat;
        }

        internal IEnumerable<Hat> GetAll() //internal means it can be used anywhere within this project
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"select * from hats";

            var hats = db.Query<Hat>(sql);

            return hats;
           
        }

        internal void Add(Hat newHat)
        {
            using var db = new SqlConnection(_connectionString);

           var sql = @"insert into hats(Color,Designer,Style)
                                    output inserted.Id
                                    values(@Color, @Designer, @Style)";

            var id = db.ExecuteScalar<Guid>(sql, newHat);

            newHat.Id = id;

        }

        internal void Remove(Guid id)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"Delete From Hats Where Id = @id";

            db.Execute(sql, new { id });

        }

        internal Hat Update(Guid id, Hat hat)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"update Hats Set
                                     Color = @color, Designer = @designer, Style = @style
                                        output inserted.*
                                      Where id = @id";

            hat.Id = id;
            var updatedHat = db.QuerySingleOrDefault<Hat>(sql, hat);
            return updatedHat;


        }

    }

}
