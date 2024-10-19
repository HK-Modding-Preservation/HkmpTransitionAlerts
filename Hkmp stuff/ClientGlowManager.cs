using Modding;
using Hkmp.Api.Client;

namespace HkmpTransitionAlerts {
    public class ClientGlowManager {

        public ClientGlowManager(GlowClientAddon addon, IClientApi clientApi) {
            HkmpTransitionAlerts._netManager = new ClientNetManager(addon, clientApi.NetClient);
            HkmpTransitionAlerts._clientApi = clientApi;
        }

        public void Initialize() {
            Enable();
            HkmpTransitionAlerts._netManager.GlowEvent += OnGlow;
        }

        public void Enable() {
            HkmpTransitionAlerts.instance.addHooks();
        }

        public void Disable() {
            HkmpTransitionAlerts.instance.removeHooks();
        }

        private void OnGlow(GlowPacket packet) {
            HkmpTransitionAlerts.instance.activateTransition(packet.scene, packet.name, packet.red, packet.green, packet.blue);
        }
    }
}
