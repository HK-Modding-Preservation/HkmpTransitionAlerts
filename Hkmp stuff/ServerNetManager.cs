using System;
using System.Collections.Generic;
using Hkmp.Api.Server;
using Hkmp.Api.Server.Networking;
using Hkmp.Networking.Packet;

namespace HkmpTransitionAlerts {
    public class ServerNetManager {
        public event Action<GlowPacket> GlowEvent;

        private readonly IServerAddonNetworkSender<ClientPacketId> _netSender;

        public ServerNetManager(ServerAddon addon, INetServer netServer) {
            _netSender = netServer.GetNetworkSender<ClientPacketId>(addon);
            var netReceiver = netServer.GetNetworkReceiver<ServerPacketId>(addon, InstantiatePacket);
            netReceiver.RegisterPacketHandler<GlowPacket>(
                ServerPacketId.Glow,
                (id, packetData) => GlowEvent?.Invoke(packetData)
            );
        }

        public void SendGlow(IReadOnlyCollection<IServerPlayer> players, string scene, string name, float red, float green, float blue) {
            foreach(var player in players) {
                try {
                    _netSender.SendSingleData(
                        ClientPacketId.Glow,
                        new GlowPacket {
                            scene = scene,
                            name = name,
                            red = red,
                            green = green,
                            blue = blue
                        },
                        player.Id);
                }
                catch { }
            }
        }

        private static IPacketData InstantiatePacket(ServerPacketId packetId) {
            switch(packetId) {
                case ServerPacketId.Glow:
                    return new GlowPacket();
            }
            return null;
        }
    }
}
