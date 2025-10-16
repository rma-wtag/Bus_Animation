using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace MapsUiTest1.Services;

public class RouteProvider
{
    private static readonly string Broker = "n0cf6f8c.ala.asia-southeast1.emqxsl.com";
    private static readonly int Port = 8883;
    private static readonly string Username = "mapsuiRider";
    private static readonly string Password = "mapsuiRider";
    private static readonly string Topic = "routeCoordinates";
    private static readonly string CaCertPath = "/opt/emqxsl-ca.crt";
    private static IMqttClient? _mqttClient;
    private static MqttClientOptions? _mqttOptions;
    private readonly BusPointProvider _busProvider;

    public RouteProvider(BusPointProvider busProvider)
    {
        _busProvider = busProvider;
        Console.WriteLine("🚀 Starting MQTT Route Subscription...");
        var clientId = Guid.NewGuid().ToString();
        _mqttClient = new MqttFactory().CreateMqttClient();
        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        _mqttOptions = BuildMqttOptions(clientId);
    }

    public async void StartListening()
    {
        if (await ConnectAsync())
        {
            await SubscribeAsync(Topic);
        }
    }

    private MqttClientOptions BuildMqttOptions(string clientId)
    {
        return new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(Broker, Port)
            .WithCredentials(Username, Password)
            .WithCleanSession()
            .WithTls(o =>
            {
                o.SslProtocol = SslProtocols.Tls12;
                o.CertificateValidationHandler = _ => true; // ⚠️ Disable validation (testing)

                if (File.Exists(CaCertPath))
                {
                    var cert = new X509Certificate2(CaCertPath);
                    o.Certificates = new List<X509Certificate> { cert };
                    Console.WriteLine("[INFO] Loaded CA certificate for TLS.");
                }
                else
                {
                    Console.WriteLine("[WARNING] CA certificate not found. Proceeding without it.");
                }
            })
            .Build();
    }
    private async Task<bool> ConnectAsync()
    {
        try
        {
            Console.WriteLine("[INFO] Connecting to MQTT broker...");
            var result = await _mqttClient!.ConnectAsync(_mqttOptions!);

            if (result.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("[SUCCESS] Connected to MQTT broker.");
                return true;
            }

            Console.WriteLine($"[ERROR] Failed to connect: {result.ResultCode}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] Connect failed: {ex.Message}");
            return false;
        }
    }
    
    private async Task SubscribeAsync(string topic)
    {
        try
        {
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();

            await _mqttClient!.SubscribeAsync(subscribeOptions);
            Console.WriteLine($"[INFO] Subscribed to topic: {topic}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] Subscribe failed: {ex.Message}");
        }
    }
    private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            Console.WriteLine($"[MESSAGE RECEIVED] Topic: {e.ApplicationMessage.Topic} | Payload: {payload}");

            // Expected payload format: "lon,lat"  OR  "lat,lon"
            var parts = payload.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 &&
                double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double lon) &&
                double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat))
            {
                _busProvider.UpdatePosition(lon, lat);
            }
            else
            {
                Console.WriteLine($"[WARNING] Invalid coordinate payload: {payload}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] OnMessageReceived: {ex.Message}");
        }

        return Task.CompletedTask;
    }
    
    private async Task DisconnectAsync()
    {
        try
        {
            await _mqttClient!.UnsubscribeAsync(Topic);
            await _mqttClient.DisconnectAsync();
            Console.WriteLine("[INFO] Disconnected from MQTT broker.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] Disconnect failed: {ex.Message}");
        }
    }
    
}