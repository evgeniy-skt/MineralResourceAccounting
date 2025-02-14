using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using MRA.Common;
using MySqlConnector;

namespace MRA.DB;

public class MineralRepository(MySqlDataSource database, ILogger logger)
{
    public async Task<IReadOnlyList<MineralDto>> GetMinerals()
    {
        await using var connection = await database.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "select * from Minerals;";
        await using var reader = await command.ExecuteReaderAsync();
        var minerals = ReadAllAsync(reader);
        logger.Log(LogLevel.Information, "Minerals retrieved");
        
        return await minerals;
    }
    
    public async Task<IReadOnlyList<MineralDto>> GetMineral(long mineralId)
    {
        await using var connection = await database.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"select * from Minerals where Id = {mineralId};";
        await using var reader = await command.ExecuteReaderAsync();
        var minerals = ReadAllAsync(reader);
        logger.Log(LogLevel.Information, $"Mineral {mineralId} retrieved");
        
        return await minerals;
    }

    private async Task<IReadOnlyList<MineralDto>> ReadAllAsync(DbDataReader reader)
    {
        var minerals = new List<MineralDto>();
        await using (reader)
        {
            while (await reader.ReadAsync())
            {
                var mineral = new MineralDto
                {
                    Id = reader.GetInt64("Id"),
                    Name = reader.GetString("Name"),
                    Type = reader.GetString("Type"),
                    Lat = reader.GetDecimal("Lat"),
                    Lon = reader.GetDecimal("Lon"),
                    AreaName = reader.GetString("AreaName"),
                    ValueM3 = reader.GetInt64("ValueM3"),
                };
                minerals.Add(mineral);
            }
        }

        return minerals;
    }

    public async Task<long> InsertAsync(MineralDto mineralDto)
    {
        await using var connection = await database.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Minerals (Name, Type, Lat, Lon, AreaName, ValueM3) 
                                VALUES (@name, @type, @lat, @lon, @areaName, @valueM3); 
                                select LAST_INSERT_ID();
                                ";
        AddParameters(command, mineralDto);
        var mineralObj = await command.ExecuteScalarAsync();
        var mineralId = long.Parse(mineralObj?.ToString() ?? throw new NoNullAllowedException());
        logger.Log(LogLevel.Information, $"Inserting mineral with id={mineralId} to database");

        return mineralId;
    }

    public async Task UpdateAsync(MineralDto mineralDto, Int64 mineralId)
    {
        await using var connection = await database.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = @$"UPDATE Minerals
                                SET Name = @name, Type = @type, lat = @Lat, Lon = @lon, AreaName = @areaName, ValueM3 = @valueM3
                                WHERE Id = {mineralId}";
        AddParameters(command, mineralDto);

        await command.ExecuteScalarAsync();
        logger.Log(LogLevel.Information, $"Mineral with id={mineralId} updated");
    }

    private void AddParameters(MySqlCommand command, MineralDto dto)
    {
        command.Parameters.AddWithValue("name", dto.Name);
        command.Parameters.AddWithValue("type", dto.Type);
        command.Parameters.AddWithValue("lat", dto.Lat.ToString()!.Replace(",", "."));
        command.Parameters.AddWithValue("lon", dto.Lat.ToString()!.Replace(",", "."));
        command.Parameters.AddWithValue("areaName", dto.AreaName);
        command.Parameters.AddWithValue("valueM3", dto.ValueM3);
    }
}