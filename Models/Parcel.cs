namespace ParcelApi.Models;

public class Parcel
{
    public required string Id { get; set; }
    public required string Address { get; set; }
    public required string Neighbourhood { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double LandAreaSqFt { get; set; }
    public double BuildingAreaSqFt { get; set; }
    public int YearBuilt { get; set; }
    public int Bedrooms { get; set; }
    public decimal AssessedValue { get; set; }
    public decimal? LastSalePrice { get; set; }
    public DateOnly? LastSaleDate { get; set; }
}
