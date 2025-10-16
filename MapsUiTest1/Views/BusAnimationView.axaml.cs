using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Animations;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Layers.AnimatedLayers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.Tiling.Layers;
using MapsUiTest1.Services;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Image = Avalonia.Controls.Image;

namespace MapsUiTest1.Views;

public partial class BusAnimationView : UserControl
{
    
    private readonly List<(double Lon, double Lat)> coordinates = new List<(double Lon, double Lat)>
    {
        (90.404872,23.793442),
        (90.403028,23.793702),
        (90.403104,23.794116),
        (90.402325,23.794233),
        (90.401967,23.794287),
        (90.401786,23.794314),
        (90.401048,23.794407),
        (90.40092,23.794424),
        (90.400936,23.794539),
        (90.401016,23.795081),
        (90.401195,23.796126),
        (90.401325,23.796886),
        (90.401333,23.796937),
        (90.40129,23.79714),
        (90.40131,23.797874),
        (90.401302,23.798014),
        (90.401224,23.798535),
        (90.401237,23.798658),
        (90.40142,23.799621),
        (90.401614,23.800792),
        (90.401674,23.801198),
        (90.401738,23.801629),
        (90.401818,23.802137),
        (90.401904,23.802705),
        (90.401957,23.80305),
        (90.402009,23.803373),
        (90.402036,23.803526),
        (90.402064,23.803648),
        (90.402089,23.803776),
        (90.40216,23.804104),
        (90.4022,23.804244),
        (90.402244,23.804364),
        (90.402318,23.804724),
        (90.402481,23.805775),
        (90.402693,23.807078),
        (90.40291,23.808397),
        (90.402956,23.80864),
        (90.402994,23.808841),
        (90.403055,23.809091),
        (90.403114,23.809282),
        (90.403186,23.809489),
        (90.403268,23.809723),
        (90.403373,23.80996),
        (90.403488,23.810197),
        (90.403556,23.810335),
        (90.403646,23.810481),
        (90.403824,23.810759),
        (90.404079,23.811104),
        (90.404298,23.811375),
        (90.404436,23.811545),
        (90.404612,23.811727),
        (90.404791,23.811903),
        (90.405144,23.812203),
        (90.40529,23.81232),
        (90.405462,23.812437),
        (90.405826,23.812671),
        (90.406988,23.813421),
        (90.408067,23.814022),
        (90.408671,23.814328),
        (90.409539,23.814748),
        (90.410115,23.815042),
        (90.41067,23.815363),
        (90.412712,23.816684),
        (90.413522,23.817267),
        (90.414084,23.817765),
        (90.414435,23.818066),
        (90.414724,23.818311),
        (90.415014,23.818517),
        (90.415259,23.818676),
        (90.415344,23.818767),
        (90.415456,23.818854),
        (90.415596,23.818925),
        (90.41575,23.819),
        (90.41592,23.819066),
        (90.416146,23.819147),
        (90.416489,23.819277),
        (90.416754,23.819383),
        (90.416964,23.819487),
        (90.417199,23.819626),
        (90.418308,23.820314),
        (90.418845,23.820626),
        (90.418976,23.820715),
        (90.419093,23.820803),
        (90.419271,23.820977),
        (90.419466,23.821209),
        (90.419678,23.821447),
        (90.419806,23.821573),
        (90.419967,23.821782),
        (90.420259,23.82217),
        (90.420309,23.822245),
        (90.420354,23.822322),
        (90.420464,23.822534),
        (90.420496,23.822598),
        (90.420507,23.822619),
        (90.420543,23.822679),
        (90.420669,23.822983),
        (90.420761,23.823201),
        (90.420836,23.823406),
        (90.420931,23.823705),
        (90.420973,23.823878),
        (90.421009,23.824059),
        (90.42103,23.82417),
        (90.42105,23.824271),
        (90.421068,23.82446),
        (90.421081,23.824615),
        (90.421089,23.824776),
        (90.421102,23.824934),
        (90.421106,23.825001),
        (90.421132,23.825176),
        (90.421163,23.825255),
        (90.421196,23.825314),
        (90.421245,23.825392),
        (90.421298,23.825452),
        (90.421345,23.825494),
        (90.421395,23.825534),
        (90.421501,23.82554),
        (90.421572,23.825548),
        (90.421642,23.825544),
        (90.421688,23.825533),
        (90.421743,23.825511),
        (90.421791,23.825481),
        (90.421843,23.825426),
        (90.421877,23.825375),
        (90.421897,23.825319),
        (90.421903,23.825277),
        (90.421911,23.82523),
        (90.421896,23.825188),
        (90.421865,23.825118),
        (90.421788,23.825031),
        (90.421718,23.824942),
        (90.421681,23.824901),
        (90.421641,23.824843),
        (90.421598,23.82477),
        (90.421559,23.824694),
        (90.421528,23.824608),
        (90.421508,23.824521),
        (90.421468,23.824085),
        (90.421422,23.823209),
        (90.421401,23.822944),
        (90.421374,23.822733),
        (90.421357,23.822629),
        (90.421342,23.822443),
        (90.42132,23.8223),
        (90.421208,23.821996),
        (90.421139,23.821799),
        (90.421029,23.821455),
        (90.420911,23.821146),
        (90.420824,23.82095),
        (90.42077,23.820856),
        (90.420694,23.820734),
        (90.42063,23.820668),
        (90.420574,23.820634),
        (90.420509,23.820593),
        (90.420445,23.82053),
        (90.420384,23.820482),
        (90.420326,23.820423),
        (90.420297,23.820377),
        (90.420289,23.820303),
        (90.420287,23.820278),
        (90.420286,23.820216),
        (90.420331,23.820004),
        (90.420374,23.819864),
        (90.420425,23.819741),
        (90.420548,23.819463),
        (90.420743,23.81908),
        (90.420825,23.818902),
        (90.420922,23.818653),
        (90.42098,23.818437),
        (90.421007,23.818352),
        (90.421047,23.818194),
        (90.421083,23.818046),
        (90.421111,23.817863),
        (90.421134,23.817599),
        (90.421156,23.817287),
        (90.421167,23.816914),
        (90.421147,23.816329),
        (90.421194,23.81501),
        (90.421209,23.814536),
        (90.421982,23.814495),
        (90.422187,23.814468),
        (90.422401,23.814497),
        (90.422466,23.814484),
        (90.422571,23.814345),
        (90.422675,23.814295),
        (90.422736,23.814285),
        (90.422823,23.81427),
        (90.423006,23.814279),
        (90.423812,23.814443)
    };
    public BusAnimationView()
    {
        InitializeComponent();
        
        var map = new Mapsui.UI.Avalonia.MapControl();
        var google = new TileLayer(new HttpTileSource(
            new GlobalSphericalMercator(),
            "https://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}",
            name: "Google Maps"
        ));
        
        map.Map.Layers.Add(google);
        
        var bananiProjected = new MPoint( SphericalMercator.FromLonLat(90.4066, 23.7937));
        
        var lineStringLayer = CreateLineStringLayer(CreateLineStringStyle());
        map.Map.Layers.Add(lineStringLayer);
        map.Map.Layers.Add(CreatePointLayerWithImage());
        map.Map.Layers.Add(CreatePointLayerWithText());
        var busPointProvider = new BusPointProvider();
        var routeProvider = new RouteProvider(busPointProvider);
        routeProvider.StartListening();
        
        map.Map.Layers.Add(new AnimatedPointLayer(busPointProvider)
        {
            Name = "Buses",
            Style = CreateCarBitmapStyle()
        });
        
        map.Map.CRS = "EPSG:3857";
        map.Map.Navigator.CenterOnAndZoomTo(bananiProjected, map.Map.Navigator.Resolutions[16]);
        
        (busPointProvider as BusPointProvider).DataChanged += async (_, _) =>
        {
            var idx = busPointProvider.GetCurrentIndex();
            if (idx < 1) return;
            var (curlon, curlat) = busPointProvider.GetCurrentPosition();
            var newPos = SphericalMercator.FromLonLat(curlon, curlat).ToMPoint();

            var getRotationAngle = busPointProvider.CalculateRotation();
            var animatedLayer = map.Map.Layers.FirstOrDefault(l => l.Name == "Buses") as AnimatedPointLayer;
            if (animatedLayer != null)
                animatedLayer.Style = CreateCarBitmapStyle(getRotationAngle);

            if (idx % 10 == 0)
                map.Map.Navigator.FlyTo(newPos, map.Map.Navigator.Resolutions[16], 1000);
        };
        
        Content = map;
    }
    public ILayer CreateLineStringLayer(IStyle? style = null)
    {
        var projectedCoords = coordinates
            .Select(c => SphericalMercator.FromLonLat(c.Lon, c.Lat).ToCoordinate())
            .ToArray();
        var lineString = new LineString(projectedCoords);

        return new MemoryLayer
        {
            Features = new[] { new GeometryFeature { Geometry = lineString } },
            Name = "BusRouteLine",
            Style = style
        };
    }
    
    public IStyle CreateLineStringStyle()
    {
        return new VectorStyle
        {
            Outline = new Pen { Color = Color.Black, Width = 1 },
            Line = new Pen { Color = Color.OrangeRed, Width = 4 }
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
            new { Name = "Banani", Lat = 23.793599, Lng = 90.404899 },
            new { Name = "Jamuna Future Park", Lat = 23.813572, Lng = 90.424024 },
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

    private ThemeStyle CreateCarBitmapStyle(double rotation = 0)
    {
        var style = new ThemeStyle(f =>
        {
            double rotation = 0;
            if (f["Rotation"] is double r)
                rotation = r;

            return new ImageStyle
            {
                Image = "embedded://MapsUiTest1.Assets.blue_car_marker.png",
                SymbolScale = 0.1,
                Offset = new Offset(0, 88), // adjust for center alignment
                SymbolRotation = rotation
            };
        });

        return style;
    }
    
    

}