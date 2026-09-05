using ParcelApi.Models;

namespace ParcelApi.Services;

public interface IParcelService
{
    IEnumerable<Parcel> GetAll(int page, int pageSize);
    Parcel? GetById(string id);
    IEnumerable<Parcel> GetComparables(string id);
    (IEnumerable<Parcel> Parcels, double MedianRatio, double Cod) GetWithinBoundingBox(double minLat, double minLon, double maxLat, double maxLon);
}
