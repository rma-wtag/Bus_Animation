using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Providers;

namespace MapsUiTest1.Services;

public sealed class BusPointProvider : MemoryProvider, IDynamic, IDisposable
{
    public event EventHandler? DataChanged;
    private (double Lon, double Lat) _currentCoordinates = (90.404872, 23.793442);
    private (double Lon, double Lat) _prevCoordinates = (90.404872, 23.793442);
    private int _currentIndex = 0;
    
    public BusPointProvider()
    {
    }
    
    public void UpdatePosition(double lon, double lat)
    {
        _prevCoordinates = _currentCoordinates;
        _currentCoordinates = (lon, lat);
        _currentIndex++;
        //Console.WriteLine($"[BusPointProvider] Updated position: {lon}, {lat}");
        OnDataChanged();
    }
    
    void IDynamic.DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new EventArgs());
    }

    public (double Lon, double Lat) GetCurrentPosition()
    {
        return _currentCoordinates;
    }
    public (double Lon, double Lat) GetPreviousPosition()
    {
        return _prevCoordinates;
    }

    public int GetCurrentIndex()
    {
        return  _currentIndex;
    }

    public override Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        var busFeature = new PointFeature(SphericalMercator.FromLonLat(_currentCoordinates.Lon, _currentCoordinates.Lat).ToMPoint());
        double rotation = CalculateRotation();
        busFeature["ID"] = "bus";
        busFeature["Rotation"] = rotation;
        return Task.FromResult((IEnumerable<IFeature>)[busFeature]);
    }
    
    public double CalculateRotation()
    {
        (double Lon, double Lat) from = _prevCoordinates;
        (double Lon, double Lat) to = _currentCoordinates;
        
        double lat1 = Math.PI * from.Lat / 180.0;
        double lat2 = Math.PI * to.Lat / 180.0;
        double deltaLon = Math.PI * (to.Lon - from.Lon) / 180.0;

        double y = Math.Sin(deltaLon) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) -
                   Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon);

        double bearing = Math.Atan2(y, x);
        bearing = bearing * 180.0 / Math.PI;
        return (bearing + 360.0) % 360.0; // Normalize
    }
    
    public void Dispose()
    {
    }
}