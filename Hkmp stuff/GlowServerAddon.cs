using Hkmp.Api.Server;

namespace HkmpTransitionAlerts {
    public class GlowServerAddon: ServerAddon {
        public override bool NeedsNetwork => true;
        protected override string Name => "HkmpTransitionAlerts";
        protected override string Version => "1.0.0.1";

        public override void Initialize(IServerApi serverApi) {
            new ServerGlowManager(this, serverApi).Initialize();
        }
    }
}
