using DineView.Application.infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Test
{
    {
    [Collection("Sequential")]
    public class BookingContextTests
    {
        private DineContext GetDatabase(bool deleteDb = false)
        {
            var db = new DineContext(new DbContextOptionsBuilder()
                .UseSqlite("Data Source = FlightBooking.db")
                .UseLazyLoadingProxies()
                .Options);
            if (deleteDb)
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            return db;
        }

        [Fact]
        public void CreateDatabaseSuccessTest()
        {
            using var db = GetDatabase(deleteDb: true);
        }

        [Fact]
        public void SeedDatabaseTest()
        {
            using var db = GetDatabase(deleteDb: true);
            //db.Seed();
        }
    }
}
