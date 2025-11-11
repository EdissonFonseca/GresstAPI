using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Helper methods for working with NetTopologySuite geometry types
/// </summary>
public static class NetTopologySuiteExtensions
{
    /// <summary>
    /// Extract latitude from a Point geometry
    /// </summary>
    public static decimal? GetLatitude(this Geometry? geometry)
    {
        if (geometry == null) return null;
        var point = geometry as Point;
        return point != null ? (decimal?)point.Y : null;
    }

    /// <summary>
    /// Extract longitude from a Point geometry
    /// </summary>
    public static decimal? GetLongitude(this Geometry? geometry)
    {
        if (geometry == null) return null;
        var point = geometry as Point;
        return point != null ? (decimal?)point.X : null;
    }

    /// <summary>
    /// Create a Point geometry from latitude and longitude
    /// </summary>
    public static Geometry? CreatePoint(decimal? latitude, decimal? longitude)
    {
        if (!latitude.HasValue || !longitude.HasValue) return null;
        
        return new Point((double)longitude.Value, (double)latitude.Value) 
        { 
            SRID = 4326 // WGS 84 - Standard GPS coordinates
        };
    }
}

