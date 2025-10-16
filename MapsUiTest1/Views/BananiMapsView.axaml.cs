using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.Tiling;
using Mapsui.Tiling.Layers;
using Mapsui.Widgets.InfoWidgets;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace MapsUiTest1.Views;

public partial class BananiMapsView : UserControl
{
    public BananiMapsView()
    {
        InitializeComponent();

        
        var googleMapUrl = new TileLayer(new HttpTileSource(
            new GlobalSphericalMercator(),
            "https://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}",
            name: "Google Maps"
        ));
        
        var map = new Mapsui.UI.Avalonia.MapControl();
        map.Map.Layers.Add(googleMapUrl);
        var lineStringLayer = CreateLineStringLayer(CreateLineStringStyle());
        map.Map.Layers.Add(lineStringLayer);
        map.Map.Layers.Add(CreatePointLayerWithText());
        map.Map.Layers.Add(CreatePointLayerWithImage());

        
        map.Map.Navigator.CenterOnAndZoomTo(lineStringLayer.Extent!.Centroid, 10);
        map.Map.Widgets.Add(new MapInfoWidget(map.Map, l => l.Name == "Points"));
        
        Content = map;
    }
    
    public ILayer CreateLineStringLayer(IStyle? style = null)
    {
        var lineString = (LineString)new WKTReader().Read(WktGr5);
        lineString = new LineString(lineString.Coordinates.Select(v => SphericalMercator.FromLonLat(v.Y, v.X).ToCoordinate()).ToArray());

        return new MemoryLayer
        {
            Features = new[] { new GeometryFeature { Geometry = lineString } },
            Name = "LineStringLayer",
            Style = style
        };
    }
    
    public IStyle CreateLineStringStyle()
    {
        return new VectorStyle
        {
            Outline = new Pen { Color = Color.Black, Width = 1 },
            Line = new Pen { Color = Color.DeepSkyBlue, Width = 4 }
        };
    }
    
    private MemoryLayer CreatePointLayerWithImage()
    {
        return new MemoryLayer
        {
            Name = "PointsImages",
            Features = GetDhakaPoints(),
            Style = CreateBitmapStyle()
        };
    }
    
    private MemoryLayer CreatePointLayerWithText()
    {
        return new MemoryLayer
        {
            Name = "PointsImages",
            Features = GetDhakaPoints(),
            MinVisible = 0,
            MaxVisible = 3,
            Style = new ThemeStyle(f =>
            {
                return new LabelStyle
                {
                    Text = f["Label"]?.ToString(),
                    ForeColor = Color.Black,
                    BackColor = new Brush(Color.White),
                    Offset = new Offset(0, -40),
                    Halo = new Pen(Color.White),
                    Font = new Font { Size = 18, Bold = true},
                    HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                    VerticalAlignment = LabelStyle.VerticalAlignmentEnum.Bottom
                };
            })
        };
    }
    
    private IEnumerable<IFeature> GetDhakaPoints()
    {
        var locations = new[]
        {
            new { Name = "Banani", Lat = 23.7937, Lng = 90.4066 },
            new { Name = "Gulshan-2", Lat = 23.7948, Lng = 90.4143 },
            new { Name = "Jamuna", Lat = 23.8135, Lng = 90.4242 },
            new {Name = "Radisson Blu" , Lat = 23.815843468372407 , Lng = 90.40715624265623 },
            new { Name = "Notun Bazar", Lat = 23.8007, Lng = 90.4262 }
        };

        return locations.Select(c =>
        {
            var feature = new PointFeature(SphericalMercator.FromLonLat(c.Lng, c.Lat).ToMPoint());
            feature["Label"] = c.Name;
            return feature;
        }).ToArray();
    }
    private ImageStyle CreateBitmapStyle()
    {
        var imageSource = "embedded://MapsUiTest1.Assets.bus_pointer_red.svg";
        var bitmapHeight = 176; 
        return new ImageStyle { Image = imageSource, SymbolScale = 0.075, Offset = new Offset(0, bitmapHeight * 0.5) };
    }
    
     private const string WktGr5 = "LINESTRING(23.7937 90.4066,23.7948 90.4143, 23.815843468372407 90.40715624265623,23.8135 90.4242 , 23.8007 90.4262)";
     
}