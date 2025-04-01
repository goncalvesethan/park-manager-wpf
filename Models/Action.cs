namespace ParkManagerWPF.Models;

public class Action
{
    public int Id { get; set; }
    public required int DeviceId { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}