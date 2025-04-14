using Microsoft.EntityFrameworkCore;
using TastyGo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TastyGoDbContext>(options =>
{
    // Example for SQL Server; replace with your provider
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    // For SQLite: options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();




app.Run();

