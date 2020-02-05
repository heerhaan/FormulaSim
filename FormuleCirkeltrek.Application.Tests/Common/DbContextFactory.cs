using FormuleCirkeltrek.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace FormuleCirkeltrek.Application.Tests.Common
{
    public class DbContextFactory : IDisposable
    {
        readonly SqliteConnection _connection;
        readonly DbContextOptions<FormuleCirkeltrekDbContext> _options;

        public DbContextFactory()
        {
            // In-memory database only exists while the connection is open
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<FormuleCirkeltrekDbContext>()
                    .UseSqlite(_connection)
                    .Options;

            // Create the schema in the database
            using (var context = new FormuleCirkeltrekDbContext(_options))
            {
                context.Database.EnsureCreated();
            }
        }

        public FormuleCirkeltrekDbContext Create()
        {
            return new FormuleCirkeltrekDbContext(_options);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Create the schema in the database
                    using (var context = new FormuleCirkeltrekDbContext(_options))
                    {
                        context.Database.EnsureDeleted();
                    }

                    _connection.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
