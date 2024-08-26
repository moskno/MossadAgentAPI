using MossadAgentAPI.Services;
using MossadAgentAPI.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MossadAgentContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<DistanceCalculate>();
builder.Services.AddScoped<MissionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseMiddleware<GlobalLoggingMW>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
