using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {
        const string _connectionString = "Server = localhost; Database = LinenAndBird; Trusted_Connection = True;";


        internal IEnumerable<Bird> GetAll()
        {
            //you must connect to database

            using var connection = new SqlConnection(_connectionString);
            //open the connection
            connection.Open();

            //tell sql what we wnt to do
            var command = connection.CreateCommand();
            command.CommandText = @"select *
                                    from Birds";

            //execute reader is for when we care about getting all the query results
            var reader = command.ExecuteReader();

            //store the birds
            var birds = new List<Bird>();

            //
            while(reader.Read())
            {
             var bird =  MapFromReader(reader);
                birds.Add(bird);
            }

            return birds;

        }

        internal void Remove(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"Delete From Birds Where Id = @id";

            command.Parameters.AddWithValue("id", id);

            command.ExecuteNonQuery();
        }

        internal Bird Update(Guid id, Bird bird)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"update Birds Set
                                     Color = @color, Name = @name, Type = @type, Size = @size
                                        output inserted.*
                                      Where id = @id";

            command.Parameters.AddWithValue("Type", bird.Type);
            command.Parameters.AddWithValue("Color", bird.Color);
            command.Parameters.AddWithValue("Size", bird.Size);
            command.Parameters.AddWithValue("Name", bird.Name);
            command.Parameters.AddWithValue("id", bird.Id);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {

                return MapFromReader(reader);
            }

            return null;
        }

        internal void Add(Bird newBird)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"insert into birds(Type,Color,Size,Name)
                                    output inserted.Id
                                    values(@Type, @Color, @Size, @Name)";

            command.Parameters.AddWithValue("Type", newBird.Type);
            command.Parameters.AddWithValue("Color", newBird.Color);
            command.Parameters.AddWithValue("Size", newBird.Size);
            command.Parameters.AddWithValue("Name", newBird.Name);

            //execute query but not care about results just number of rows
            //var numberOfRowsAffected = command.ExecuteNonQuery();

            var newId = (Guid)command.ExecuteScalar();

            newBird.Id = newId;
        }

        internal Bird GetById(Guid birdId)
        {
            using var connection = new SqlConnection(_connectionString);
            //open the connection
            connection.Open();

            //tell sql what we wnt to do
            var command = connection.CreateCommand();
            command.CommandText = @"select *
                                    from Birds
                                     where id = @id";

            command.Parameters.AddWithValue("id", birdId);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {

                return MapFromReader(reader);
            }

            return null;

            //return _birds.FirstOrDefault(bird => bird.Id == birdId);
        }

        Bird MapFromReader(SqlDataReader reader)
        {
            var bird = new Bird();
            bird.Id = reader.GetGuid(0);
            bird.Color = reader["Color"].ToString();
            bird.Size = reader["Size"].ToString();
            bird.Type = (BirdType)reader["Type"];
            bird.Name = reader["Name"].ToString();

            return bird;
        }
    }
}
