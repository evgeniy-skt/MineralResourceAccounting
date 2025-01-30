using Microsoft.AspNetCore.Mvc;
using MineralResourceAccounting;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMySqlDataSource(builder.Configuration.GetConnectionString("AppConnection")!);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/getminerals",  async ([FromServices] MySqlDataSource db) =>
{
    var repository = new MineralRepository(db);
    var minerals = await repository.GetMinerals();
    return minerals;
});

app.MapPost("/createmineral", async ([FromServices] MySqlDataSource db, [FromBody] MineralDto body) =>
{
    var repository = new MineralRepository(db);
    await repository.InsertAsync(body);
    return body;
} );

app.Run();