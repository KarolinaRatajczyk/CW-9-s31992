using CW9.Data;
using CW9.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default-db"));
});

builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();

app.MapControllers();

app.Run();