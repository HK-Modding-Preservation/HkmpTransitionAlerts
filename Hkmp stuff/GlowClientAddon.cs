using Hkmp.Api.Client;

namespace HkmpTransitionAlerts {
    public class GlowClientAddon: TogglableClientAddon {
        public override bool NeedsNetwork => true;
        protected override string Name => "HkmpTransitionAlerts";
        protected override string Version => "1.0.0.0";

        private ClientGlowManager _glowManager;

        public override void Initialize(IClientApi clientApi) {
            _glowManager = new ClientGlowManager(this, clientApi);
            _glowManager.Initialize();
        }

        protected override void OnEnable() {
            _glowManager.Enable();
        }

        protected override void OnDisable() {
            _glowManager.Disable();
        }
    }
}
