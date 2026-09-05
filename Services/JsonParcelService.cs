using System.Text.Json;
using ParcelApi.Models;

namespace ParcelApi.Services;

public class JsonParcelService : IParcelService
{
    private readonly List<Parcel> _parcels;

    public JsonParcelService()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "parcels.json");
        var json = File.ReadAllText(path);
        _parcels = JsonSerializer.Deserialize<List<Parcel>>(json) ?? [];
    }

    public IEnumerable<Parcel> GetAll(int page, int pageSize)
    {
        return _parcels.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public Parcel? GetById(string id)
    {
        return _parcels.FirstOrDefault(p => p.Id == id);
    }

    private static double Distance(Parcel a, Parcel b)
    {
        var dLat = a.Latitude - b.Latitude;
        var dLon = a.Longitude - b.Longitude;
        return Math.Sqrt(dLat * dLat + dLon * dLon);
    }

    public IEnumerable<Parcel> GetComparables(string id)
    {
        var target = _parcels.FirstOrDefault(p => p.Id == id);
        if (target is null) return [];

        return _parcels
            .Where(p => p.Id != target.Id)
            .Where(p => p.LastSalePrice is not null)
            .Where(p => p.Neighbourhood == target.Neighbourhood)
            .OrderBy(p => Distance(p, target))
            .Take(5);
    }

    private static double Median(List<double> sortedRatios)
    {
        var mid = sortedRatios.Count / 2;
        return sortedRatios.Count % 2 == 0
            ? (sortedRatios[mid - 1] + sortedRatios[mid]) / 2
            : sortedRatios[mid];
    }

    private static double Cod(List<double> ratios, double median)
    {
        var meanAbsoluteDeviation = ratios.Average(r => Math.Abs(r - median));
        return 100 * meanAbsoluteDeviation / median;
    }

    public (IEnumerable<Parcel> Parcels, double MedianRatio, double Cod) GetWithinBoundingBox(double minLat, double minLon, double maxLat, double maxLon)
    {
        var parcelsInBox = _parcels
            .Where(p => p.Latitude >= minLat && p.Latitude <= maxLat)
            .Where(p => p.Longitude >= minLon && p.Longitude <= maxLon)
            .ToList();

        var ratios = parcelsInBox
            .Where(p => p.LastSalePrice is not null)
            .Select(p => (double)(p.AssessedValue / p.LastSalePrice!.Value))
            .OrderBy(r => r)
            .ToList();

        if (ratios.Count == 0)
            return (parcelsInBox, 0, 0);

        var median = Median(ratios);
        var cod = Cod(ratios, median);
        return (parcelsInBox, median, cod);
    }
}
