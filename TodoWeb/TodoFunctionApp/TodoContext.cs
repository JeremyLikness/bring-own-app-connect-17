using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TodoFunctionApp
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables();

            var config = builder.Build();

            var connection = config.GetConnectionString("TodoContext");
            optionsBuilder.UseSqlServer(connection);
        }

        public DbSet<TodoItem> TodoItems { get; set; }

    }
}
