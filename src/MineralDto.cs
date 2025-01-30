namespace MineralResourceAccounting;

public class MineralDto
{
    public Int64? Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lon { get; set; }
    public string? AreaName { get; set; }
    public Int64? ValueM3 { get; set; }
}