using System.Data.Common;
using MySqlConnector;

namespace MineralResourceAccounting;

public class MineralRepository(MySqlDataSource database)
{
    public async Task<IReadOnlyList<MineralDto>> GetMinerals()
    {
        using var connection = await database.OpenConnectionAsync();
        using var command = connection.CreateCommand();
        command.CommandText = "select * from Minerals;";
        using var reader = await command.ExecuteReaderAsync();
        var minerals = ReadAllAsync(reader);

        return await minerals;
    }

    private async Task<IReadOnlyList<MineralDto>> ReadAllAsync(DbDataReader reader)
    {
        var minerals = new List<MineralDto>();
        using (reader)
        {
            while (await reader.ReadAsync())
            {
                var mineral = new MineralDto
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Type = reader.GetString(2),
                    Lat = reader.GetDecimal(3),
                    Lon = reader.GetDecimal(4),
                    AreaName = reader.GetString(5),
                    ValueM3 = reader.GetInt64(6)
                };
                minerals.Add(mineral);
            }
        }

        return minerals;
    }

    public async Task<long> InsertAsync(MineralDto mineralDto)
    {
        using var connection = await database.OpenConnectionAsync();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Minerals (Name, Type, Lat, Lon, AreaName, ValueM3) 
                                VALUES (@name, @type, @lat, @lon, @areaName, @volumeM3); 
                                select LAST_INSERT_ID();
                                ";
        AddParameters(command, mineralDto);

        var mineralObj = await command.ExecuteScalarAsync();
        var mineralId = long.Parse(mineralObj.ToString());

        Console.WriteLine($"Created Mineral: {mineralId}");

        return mineralId;
    }

    public async Task UpdateAsync(MineralDto mineralDto, Int64 mineralId)
    {
        using var connection = await database.OpenConnectionAsync();
        using var command = connection.CreateCommand();

        command.CommandText = @$"UPDATE Minerals
                                SET Name = @name, Type = @type, lat = @Lat, Lon = @lon, AreaName = @areaName, ValueM3 = @valueM3
                                WHERE Id = {mineralId}";
        AddParameters(command, mineralDto);

        await command.ExecuteScalarAsync();

        Console.WriteLine($"Updated Mineral: {mineralId}");
    }

    private void AddParameters(MySqlCommand command, MineralDto dto)
    {
        command.Parameters.AddWithValue("name", dto.Name);
        command.Parameters.AddWithValue("type", dto.Type);
        command.Parameters.AddWithValue("lat", dto.Lat.ToString().Replace(",", "."));
        command.Parameters.AddWithValue("lon", dto.Lat.ToString().Replace(",", "."));
        command.Parameters.AddWithValue("areaName", dto.AreaName);
        command.Parameters.AddWithValue("valueM3", dto.ValueM3);
    }
}