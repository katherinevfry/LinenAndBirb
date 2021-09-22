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
    public class OrdersRepository
    {
        readonly string _connectionString;
        public OrdersRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("LinenAndBird");
        }

        internal void Add(Order order)
        {
            //create a connection

           using var db = new SqlConnection(_connectionString);


            var sql = @"INSERT INTO [dbo].[Orders]
                                   ([BirdId]
                                   ,[HatId]
                                   ,[Price])
                               Output inserted.Id
                               VALUES
                                   (@BirdId
                                   ,@HatId
                                   ,@Price)";

            var param = new
            {
                BirdId = order.Bird.Id,
                HatId = order.Hat.Id,
                Price = order.Price
            };

            var id = db.ExecuteScalar<Guid>(sql, param);

            order.Id = id;

        }

        internal IEnumerable<Order> GetAll()
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"select *
                        from Orders o
                        join Birds b
                        on b.Id = o.BirdId
                        join Hats h
                        on h.Id = o.HatId";

            //multi mapping
            //the last thing is what everything ultimately belongs to
            var results = db.Query<Order, Bird, Hat, Order>(sql, Map, splitOn: "id");

            return results;
        }

        internal Order Get(Guid id)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"select *
                        from Orders o
                        join Birds b
                        on b.Id = o.BirdId
                        join Hats h
                        on h.Id = o.HatId
                         where o.id = @id";

            var orders = db.Query<Order, Bird, Hat, Order>(sql, Map, new { id }, splitOn: "id");

            return orders.FirstOrDefault();
        }

        Order Map(Order order, Bird bird, Hat hat)
        {
            order.Bird = bird;
            order.Hat = hat;
            return order;
        }
    }
}
