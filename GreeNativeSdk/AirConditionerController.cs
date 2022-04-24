namespace GreeNativeSdk;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CzomPack.Logging;
using GreeNativeSdk.Protocol;

public class AirConditionerController
{
    private readonly AirConditioner _model;
    private readonly string _logPrefix;

    public AirConditionerController(AirConditioner model)
    {
        Parameters = new Dictionary<string, int>();
        _model = model;
        _logPrefix = $"[Controller({_model.Name}/{_model.Id})] ";
        Logger.Debug($"{_logPrefix}Controller created");
    }

    public delegate void DeviceStatusChangedEventHandler(object sender, DeviceStatusChangedEventArgs e);

    public event DeviceStatusChangedEventHandler DeviceStatusChanged;

    public string DeviceName { get => _model.Name; private set { } }

    public string DeviceId { get => _model.Id; private set { } }

    public Dictionary<string, int> Parameters { get; private set; }

    public async Task UpdateDeviceStatus()
    {
        Logger.Debug($"{_logPrefix}Updating device status");

        var columns = typeof(DeviceParameterKeys).GetFields()
            .Where((f) => f.FieldType == typeof(string))
            .Select((f) => f.GetValue(null) as string)
            .ToList();

        var pack = DeviceStatusRequestPack.Create(_model.Id, columns);
        var json = JsonSerializer.Serialize(pack);

        var encrypted = Crypto.EncryptData(json, _model.PrivateKey);
        if (encrypted == null)
        {
            Logger.Warning($"{_logPrefix}Failed to encrypt DeviceStatusRequestPack");
            return;
        }

        var request = Request.Create(_model.Id, encrypted);

        ResponsePackInfo response;

        try
        {
            response = await SendDeviceRequest(request);
        }
        catch (Exception e)
        {
            Logger.Warning($"{_logPrefix}Failed to send DeviceStatusRequestPack. Error: {{error}}", e);
            return;
        }

        json = Crypto.DecryptData(response.Pack, _model.PrivateKey);
        if (json == null)
        {
            Logger.Warning($"{_logPrefix}Failed to decrypt DeviceStatusResponsePack");
            return;
        }

        var responsePack = JsonSerializer.Deserialize<DeviceStatusResponsePack>(json);
        if (responsePack == null)
        {
            Logger.Warning($"{_logPrefix}Failed to deserialize DeviceStatusReponsePack");
            return;
        }

        var updatedParameters = responsePack.Columns
            .Zip(responsePack.Values, (k, v) => new { k, v })
            .ToDictionary(x => x.k, x => x.v);

        bool parametersChanged = !Parameters.OrderBy(pair => pair.Key)
            .SequenceEqual(updatedParameters.OrderBy(pair => pair.Key));

        if (parametersChanged)
        {
            Logger.Debug($"{_logPrefix}Device parameters updated");
            Parameters = updatedParameters;

            DeviceStatusChanged?.Invoke(
                this,
                new DeviceStatusChangedEventArgs()
                {
                    Parameters = updatedParameters
                });
        }
    }

    public async Task SetDeviceParameter(string name, int value)
    {
        Logger.Debug($"{_logPrefix}Setting parameter: {{name}}={{value}}", name, value);

        var pack = CommandRequestPack.Create(
            DeviceId,
            new List<string>() { name },
            new List<int>() { value });

        var json = JsonSerializer.Serialize(pack);
        var request = Request.Create(DeviceId, Crypto.EncryptData(json, _model.PrivateKey));

        ResponsePackInfo response;
        try
        {
            response = await SendDeviceRequest(request);
        }
        catch (IOException e)
        {
            Logger.Warning($"{_logPrefix}Failed to send CommandRequestPack: {{error}}", e);
            return;
        }

        json = Crypto.DecryptData(response.Pack, _model.PrivateKey);
        var responsePack = JsonSerializer.Deserialize<CommandResponsePack>(json);

        if (!responsePack.Options.Contains(name))
        {
            Logger.Warning($"{_logPrefix}Parameter cannot be changed.");
        }
    }

    /// <summary>
    /// Sends a request to the actual device and waits a few seconds for the response.
    /// </summary>
    /// <param name="request">Request object which encapsulates the encrypted pack</param>
    /// <returns>The response object which encapsulates the encrypted response pack</returns>
    /// <exception cref="IOException"/>
    private async Task<ResponsePackInfo> SendDeviceRequest(Request request)
    {
        Logger.Debug($"{_logPrefix}Sending device request");

        var datagram = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(request));
        Logger.Debug($"{_logPrefix}{{byteCount}} bytes will be sent", datagram.Length);

        using (var udp = new UdpClient())
        {
            var sent = await udp.SendAsync(datagram, datagram.Length, _model.Address, 7000);
            Logger.Debug($"{_logPrefix}{{byteCount}} bytes sent to {{address}}", sent, _model.Address);

            for (int i = 0; i < 20; ++i)
            {
                if (udp.Available > 0)
                {
                    var results = await udp.ReceiveAsync();
                    Logger.Debug($"{_logPrefix}Got response, {{byteCount}} bytes", results.Buffer.Length);

                    var json = Encoding.ASCII.GetString(results.Buffer);
                    var response = JsonSerializer.Deserialize<ResponsePackInfo>(json);

                    return response;
                }

                await Task.Delay(100);
            }

            Logger.Warning("Request timed out");

            throw new IOException("Device request timed out");
        }
    }
}
