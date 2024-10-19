using Modding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Satchel.BetterMenus;
using Hkmp.Api.Client;
using Hkmp.Api.Server;

namespace HkmpTransitionAlerts {
    public class HkmpTransitionAlerts: Mod, ICustomMenuMod, IGlobalSettings<GlobalSettings> {
        new public string GetName() => "HkmpTransitionAlerts";
        public override string GetVersion() => "1.0.0.0";
        public static HkmpTransitionAlerts instance;

        Dictionary<string, List<(string, Color, float)>> activeLog = new();

        private Menu MenuRef;
        public static GlobalSettings gs = new();

        public static ClientNetManager _netManager;
        public static IClientApi _clientApi;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            instance = this;

            ClientAddon.RegisterAddon(new GlowClientAddon());
            ServerAddon.RegisterAddon(new GlowServerAddon());
        }

        public void addHooks() {
            On.TransitionPoint.OnTriggerEnter2D += enterTransition;
            On.GameManager.OnNextLevelReady += lateSceneLoad;
        }

        public void removeHooks() {
            On.TransitionPoint.OnTriggerEnter2D -= enterTransition;
            On.GameManager.OnNextLevelReady -= lateSceneLoad;
        }

        public void enterTransition(On.TransitionPoint.orig_OnTriggerEnter2D orig, TransitionPoint self, Collider2D movingObj) {
            if(!self.isADoor && movingObj.gameObject.layer == 9 && GameManager.instance.gameState == GlobalEnums.GameState.PLAYING) {
                if(!string.IsNullOrEmpty(self.targetScene) && !string.IsNullOrEmpty(self.entryPoint)) {
                    if(_clientApi.NetClient.IsConnected) {
                        _netManager.SendGlow(self.gameObject.scene.name, self.gameObject.name, gs.glowRed, gs.glowGreen, gs.glowBlue);
                    }
                }
            }
            orig(self, movingObj);
        }

        public void lateSceneLoad(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            Dictionary<string, List<(string, Color, float)>> expired = new();
            foreach(string key in activeLog.Keys) {
                foreach((string, Color, float) item in activeLog[key]) {
                    if(item.Item3 - Time.time > gs.duration) {
                        if(!expired.ContainsKey(key)) {
                            expired.Add(key, new());
                        }
                        expired[key].Add(item);
                    }
                }
            }
            foreach(string key in expired.Keys) {
                foreach((string, Color, float) item in expired[key]) {
                    activeLog[key].Remove(item);
                }
                if(activeLog[key].Count == 0) {
                    activeLog.Remove(key);
                }
            }
            if(activeLog.ContainsKey(self.sceneName)) {
                foreach((string, Color, float) transition in activeLog[self.sceneName]) {
                    showColor(GameObject.Find(transition.Item1), transition.Item2, transition.Item3);
                }
            }
        }

        private async void showColor(GameObject transition, Color glowColor, float startTime) {
            SpriteRenderer sr = transition.GetComponentInChildren<SpriteRenderer>();
            while(Time.time - startTime < gs.duration) {
                try {
                    sr.color = new Color(glowColor.r, glowColor.g, glowColor.b, 1-((Time.time - startTime) / gs.duration));
                    await Task.Yield();
                }
                catch(Exception) {
                    break;
                }
            }
            try {
                sr.color = new Color(0.7721f, 0.8868f, 1, 0.5373f);
            }
            catch(Exception) { }
        }

        public void activateTransition(string scene, string name, float red, float green, float blue) {
            if(!activeLog.ContainsKey(scene)) {
                activeLog.Add(scene, new());
            }
            for(int i = 0; i < activeLog[scene].Count; i++) {
                if(activeLog[scene][i].Item1 == name) {
                    activeLog[scene].RemoveAt(i);
                    break;
                }
            }
            Color glow = new Color(red, green, blue);
            activeLog[scene].Add((name, glow, Time.time));
            if(GameManager.instance.sceneName == scene) {
                showColor(GameObject.Find(name), glow, Time.time);
            }
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates) {
            MenuRef ??= new Menu(
                name: "HkmpTransitionAlerts",
                elements: new Element[] {
                    new CustomSlider(
                        name: "Glow Duration",
                        storeValue: val => { gs.duration = val; },
                        loadValue: () => gs.duration,
                        minValue: 0,
                        maxValue: 30,
                        wholeNumbers: true
                    ),
                    new TextPanel(""),
                    new TextPanel("Glow Color"),
                    new CustomSlider(
                        name: "r",
                        storeValue: val => { gs.glowRed = val; },
                        loadValue: () => gs.glowRed,
                        minValue: 0,
                        maxValue: 1,
                        wholeNumbers: false
                    ),
                    new CustomSlider(
                        name: "g",
                        storeValue: val => { gs.glowGreen = val; },
                        loadValue: () => gs.glowGreen,
                        minValue: 0,
                        maxValue: 1,
                        wholeNumbers: false
                    ),
                    new CustomSlider(
                        name: "b",
                        storeValue: val => { gs.glowBlue = val; },
                        loadValue: () => gs.glowBlue,
                        minValue: 0,
                        maxValue: 1,
                        wholeNumbers: false
                    )
                }
            );
            return MenuRef.GetMenuScreen(modListMenu);
        }

        public bool ToggleButtonInsideMenu {
            get;
        }

        public void OnLoadGlobal(GlobalSettings s) {
            gs = s;
        }

        public GlobalSettings OnSaveGlobal() {
            return gs;
        }
    }

    public class GlobalSettings {
        public float duration = 5;
        public float glowRed = 1;
        public float glowGreen = 0;
        public float glowBlue = 0;
    }
}