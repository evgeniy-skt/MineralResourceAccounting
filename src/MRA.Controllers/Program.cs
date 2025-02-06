using Microsoft.AspNetCore.Mvc;
using MRA.Common;
using MRA.DB;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MineralResourceAccounting", Version = "v1" });
});

builder.Services.AddMySqlDataSource(builder.Configuration.GetConnectionString("AppConnection")!);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MineralResourceAccounting v1");
    });
}

app.UseHttpsRedirection();

app.MapGet("/getminerals", async ([FromServices] MySqlDataSource db) =>
{
    var repository = new MineralRepository(db);
    var minerals = await repository.GetMinerals();
    return minerals;
});

app.MapGet("/getmineral/{id}", async ([FromServices] MySqlDataSource db, long mineralId) =>
{
    var repository = new MineralRepository(db);
    var minerals = await repository.GetMineral(mineralId);
    return minerals;
});

app.MapPost("/createmineral", async ([FromServices] MySqlDataSource db, [FromBody] MineralDto body) =>
{
    var repository = new MineralRepository(db);
    await repository.InsertAsync(body);
    return body;
});

app.MapPut("/updatemineral/{id}",
    async ([FromServices] MySqlDataSource db, [FromBody] MineralDto updatedMineralDto, long mineralId) =>
    {
        var repository = new MineralRepository(db);
        await repository.UpdateAsync(updatedMineralDto, mineralId);
        return updatedMineralDto;
    });

app.Run();