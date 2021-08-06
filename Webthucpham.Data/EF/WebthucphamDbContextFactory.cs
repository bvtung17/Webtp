using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Webthucpham.Data.EF
{
    class WebthucphamDbContextFactory : IDesignTimeDbContextFactory<WebthucphamDbContext>
    {
        public WebthucphamDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder() 
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();
            var connectionString = configuration.GetConnectionString("WebthucphamDb");
            var optionsBuilder = new DbContextOptionsBuilder<WebthucphamDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
                
            return new WebthucphamDbContext(optionsBuilder.Options);
        }
    }
}
