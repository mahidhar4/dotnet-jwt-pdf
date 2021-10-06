


using Microsoft.EntityFrameworkCore;
using dotnet.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace dotnet
{
    public class DbMemContext : DbContext
    {
        public DbMemContext(DbContextOptions<DbMemContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

    }
    public class AddRequiredHeaderParameter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Authorization Header",
                Required = false
            });
            //operation.
        }
    }
}