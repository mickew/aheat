namespace AHeat.Web.API.Models;

public class ClimateDeviceReading
{
    public int ID { get; set; }
    public int ClimateDeviceID { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public DateTime Time { get; set; }
    public ClimateDevice? ClimateDevice { get; set; }
}
