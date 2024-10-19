using System;
using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;

namespace HkmpTransitionAlerts {
    public class ClientNetManager {
        public event Action<GlowPacket> GlowEvent;

        private readonly IClientAddonNetworkSender<ServerPacketId> _netSender;

        public ClientNetManager(ClientAddon addon, INetClient netClient) {
            _netSender = netClient.GetNetworkSender<ServerPacketId>(addon);
            var netReceiver = netClient.GetNetworkReceiver<ClientPacketId>(addon, InstantiatePacket);
            netReceiver.RegisterPacketHandler<GlowPacket>(
                ClientPacketId.Glow,
                packetData => GlowEvent?.Invoke(packetData)
            );
        }

        public void SendGlow(string scene, string name, float red, float green, float blue) {
            _netSender.SendSingleData(
                ServerPacketId.Glow,
                new GlowPacket {
                    scene = scene,
                    name = name,
                    red = red,
                    green = green,
                    blue = blue
                }
            );
        }

        private static IPacketData InstantiatePacket(ClientPacketId packetId) {
            switch(packetId) {
                case ClientPacketId.Glow:
                    return new GlowPacket();
            }
            return null;
        }
    }
}
