namespace GreeNativeSdk
{
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using CzomPack.Logging;
    using GreeNativeSdk.Protocol;

    public static class Scanner
    {
        public static async Task<List<AirConditioner>> Scan(string broadcastAddresses)
        {
            var foundUnits = new List<AirConditioner>();

            var responses = await DiscoverLocalDevices(broadcastAddresses);

            foreach (var response in responses)
            {
                var responsePackInfo = JsonSerializer.Deserialize<ResponsePackInfo>(response.Json);
                if (responsePackInfo.Type != "pack")
                {
                    continue;
                }

                var decryptedPack = Crypto.DecryptGenericData(responsePackInfo.Pack);

                var packInfo = JsonSerializer.Deserialize<PackInfo>(decryptedPack);
                if (packInfo.Type != "dev")
                {
                    continue;
                }

                var deviceInfo = JsonSerializer.Deserialize<DeviceInfoResponsePack>(decryptedPack);

                Logger.Info($"Found: " +
                    "ClientId={clientId}, " +
                    "FirmwareVersion={firmwareVersion}, " +
                    "Name={name}, " +
                    "Address={address}", deviceInfo.ClientId, deviceInfo.FirmwareVersion, deviceInfo.FriendlyName, response.Address);

                //// TODO check if already bound

                Logger.Debug("  Binding");

                var bindRequestPack = new BindRequestPack() { MAC = deviceInfo.ClientId };
                var request = Request.Create(deviceInfo.ClientId, Crypto.EncryptGenericData(JsonSerializer.Serialize(bindRequestPack)), 1);
                var requestJson = JsonSerializer.Serialize(request);

                var datagram = Encoding.ASCII.GetBytes(requestJson);

                using (var udp = new UdpClient())
                {
                    var sent = await udp.SendAsync(datagram, datagram.Length, response.Address, 7000);

                    if (sent != datagram.Length)
                    {
                        Logger.Warning("  Binding request cannot be sent");
                        continue;
                    }

                    for (int i = 0; i < 50; ++i)
                    {
                        if (udp.Available > 0)
                        {
                            var result = await udp.ReceiveAsync();
                            if (result.RemoteEndPoint.Address.ToString() != response.Address)
                            {
                                Logger.Warning($"  Got binding response from the wrong device: {{address}}", result.RemoteEndPoint.Address);
                                continue;
                            }

                            var responseJson = Encoding.ASCII.GetString(result.Buffer);

                            responsePackInfo = JsonSerializer.Deserialize<ResponsePackInfo>(responseJson);
                            if (responsePackInfo.Type != "pack")
                            {
                                continue;
                            }

                            var bindResponse = JsonSerializer.Deserialize<BindResponsePack>(Crypto.DecryptGenericData(responsePackInfo.Pack));

                            Logger.Debug($"  Success. Key: {{key}}", bindResponse.Key);

                            foundUnits.Add(new AirConditioner()
                            {
                                Id = deviceInfo.ClientId,
                                Name = deviceInfo.FriendlyName,
                                Address = result.RemoteEndPoint.Address.ToString(),
                                PrivateKey = bindResponse.Key
                            });

                            break;
                        }

                        await Task.Delay(100);
                    }
                }
            }

            Logger.Info("Scan finished");

            return foundUnits;
        }

        private static async Task<List<DeviceDiscoveryResponse>> DiscoverLocalDevices(string broadcastAddress)
        {
            var responses = new List<DeviceDiscoveryResponse>();

            using (var udp = new UdpClient())
            {
                udp.EnableBroadcast = true;

                Logger.Debug("Sending scan packet");

                var bytes = Encoding.ASCII.GetBytes("{ \"t\": \"scan\" }");

                var sent = await udp.SendAsync(bytes, bytes.Length, broadcastAddress, 7000);

                Logger.Debug($"Sent bytes: {{sentBytes}}", sent);

                for (int i = 0; i < 20; ++i)
                {
                    if (udp.Available > 0)
                    {
                        var result = await udp.ReceiveAsync();

                        responses.Add(new DeviceDiscoveryResponse()
                        {
                            Json = Encoding.ASCII.GetString(result.Buffer),
                            Address = result.RemoteEndPoint.Address.ToString()
                        });

                        Logger.Debug($"Got response from {{address}}.", result.RemoteEndPoint.Address);
                    }

                    await Task.Delay(100);
                }
            }

            return responses;
        }
    }
}
