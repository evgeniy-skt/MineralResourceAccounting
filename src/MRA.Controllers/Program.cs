using Microsoft.AspNetCore.Mvc;
using MineralResourceAccounting.Configuration;
using MRA.Common;
using MRA.DB;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddServices(builder);

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

app.MapGet("/getminerals", async ([FromServices] MySqlDataSource db, [FromServices] MineralRepository mineralRepository) =>
{
    var minerals = await mineralRepository.GetMinerals();
    return minerals;
});

app.MapGet("/getmineral/{id}", async ([FromServices] MySqlDataSource db, [FromServices] MineralRepository mineralRepository, long mineralId) =>
{
    var minerals = await mineralRepository.GetMineral(mineralId);
    return minerals;
});

app.MapPost("/createmineral", async ([FromServices] MySqlDataSource db, [FromServices] MineralRepository mineralRepository, [FromBody] MineralDto body) =>
{
    await mineralRepository.InsertAsync(body);
    return body;
});

app.MapPut("/updatemineral/{id}",
    async ([FromServices] MySqlDataSource db, [FromServices] MineralRepository mineralRepository, [FromBody] MineralDto updatedMineralDto, long mineralId) =>
    {
        await mineralRepository.UpdateAsync(updatedMineralDto, mineralId);
        return updatedMineralDto;
    });

app.Run();