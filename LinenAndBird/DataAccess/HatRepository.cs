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

        internal Hat GetByStyle(HatStyle style)
        {
            using var connection = new SqlConnection(_connectionString);
            //open the connection
            connection.Open();

            //tell sql what we wnt to do
            var command = connection.CreateCommand();
            command.CommandText = @"select *
                                    from Hats
                                     where style = @style";

            command.Parameters.AddWithValue("style", style);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {

                return MapFromReader(reader);
            }
            return null;
            //return _hats.Where(hat => hat.Style == style);
        }

        internal List<Hat> GetAll() //internal means it can be used anywhere within this project
        {
            using var connection = new SqlConnection(_connectionString);
            //open the connection
            connection.Open();

            //tell sql what we wnt to do
            var command = connection.CreateCommand();
            command.CommandText = @"select *
                                    from Hats";

            //execute reader is for when we care about getting all the query results
            var reader = command.ExecuteReader();

            //store the birds
            var hats = new List<Hat>();

            //
            while (reader.Read())
            {
                var hat = MapFromReader(reader);
                hats.Add(hat);
            }

            return hats;
        }

        internal void Add(Hat newHat)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"insert into hats(Color,Designer,Style)
                                    output inserted.Id
                                    values(@Color, @Designer, @Style)";

            command.Parameters.AddWithValue("Color", newHat.Color);
            command.Parameters.AddWithValue("Designer", newHat.Designer);
            command.Parameters.AddWithValue("Style", newHat.Style);

            //execute query but not care about results just number of rows
            //var numberOfRowsAffected = command.ExecuteNonQuery();

            var newId = (Guid)command.ExecuteScalar();

            newHat.Id = newId;
        }

        internal void Remove(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"Delete From Hats Where Id = @id";

            command.Parameters.AddWithValue("id", id);

            command.ExecuteNonQuery();
        }

        internal Hat Update(Guid id, Hat hat)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"update Hats Set
                                     Color = @color, Designer = @designer, Style = @style
                                        output inserted.*
                                      Where id = @id";

            command.Parameters.AddWithValue("Color", hat.Color);
            command.Parameters.AddWithValue("Designer", hat.Designer);
            command.Parameters.AddWithValue("Style", hat.Style);
            command.Parameters.AddWithValue("id", hat.Id);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {

                return MapFromReader(reader);
            }

            return null;
        }

        Hat MapFromReader(SqlDataReader reader)
        {
            var hat = new Hat();
            hat.Id = reader.GetGuid(0);
            hat.Color = reader["Color"].ToString();
            hat.Designer = reader["Designer"].ToString();
            hat.Style = (HatStyle)reader["Style"];
    
            return hat;
        }
    }

}
