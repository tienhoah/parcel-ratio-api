using Microsoft.AspNetCore.Mvc;
using ParcelApi.Models;
using ParcelApi.Services;

namespace ParcelApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ParcelsController : ControllerBase
{
    private readonly IParcelService _parcelService;

    public ParcelsController(IParcelService parcelService)
    {
        _parcelService = parcelService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Parcel>> GetAll(int page = 1, int pageSize = 20)
    {
        return Ok(_parcelService.GetAll(page, pageSize));
    }

    [HttpGet("{id}")]
    public ActionResult<Parcel> GetById(string id)
    {
        var parcel = _parcelService.GetById(id);
        if (parcel is null) return NotFound();

        return Ok(parcel);
    }

    [HttpGet("{id}/comparables")]
    public ActionResult<IEnumerable<Parcel>> GetComparables(string id)
    {
        return Ok(_parcelService.GetComparables(id));
    }

    [HttpGet("within")]
    public ActionResult<object> GetWithinBoundingBox(string bbox)
    {
        var parts = bbox.Split(',').Select(double.Parse).ToArray();
        var (minLon, minLat, maxLon, maxLat) = (parts[0], parts[1], parts[2], parts[3]);

        var (parcels, medianRatio, cod) = _parcelService.GetWithinBoundingBox(minLat, minLon, maxLat, maxLon);

        return Ok(new { parcels, medianRatio, cod });
    }

}
