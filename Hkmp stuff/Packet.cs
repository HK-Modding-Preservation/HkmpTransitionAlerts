using Hkmp.Networking.Packet;

namespace HkmpTransitionAlerts {
    public class GlowPacket: IPacketData {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => false;

        public string scene { get; set; }
        public string name { get; set; }
        public float red { get; set; }
        public float green { get; set; }
        public float blue { get; set; }

        public void WriteData(IPacket packet) {
            packet.Write(scene);
            packet.Write(name);
            packet.Write(red);
            packet.Write(green);
            packet.Write(blue);
        }

        public void ReadData(IPacket packet) {
            scene = packet.ReadString();
            name = packet.ReadString();
            red = packet.ReadFloat();
            green = packet.ReadFloat();
            blue = packet.ReadFloat();
        }
    }
}
