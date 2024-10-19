using Hkmp.Api.Server;

namespace HkmpTransitionAlerts {
    public class ServerGlowManager {

        private readonly IServerApi _serverApi;
        private readonly ServerNetManager _netManager;

        public ServerGlowManager(GlowServerAddon addon, IServerApi serverApi) {
            _serverApi = serverApi;
            _netManager = new ServerNetManager(addon, serverApi.NetServer);
        }

        public void Initialize() {
            _netManager.GlowEvent += packet => OnGlow(packet.scene, packet.name, packet.red, packet.green, packet.blue);
        }

        private void OnGlow(string scene, string name, float red, float green, float blue) {
            _netManager.SendGlow(_serverApi.ServerManager.Players, scene, name, red, green, blue);
        }
    }
}
