using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/* All demo console commands:
		* = GS.method not available in full build, will need extra work
		# = needs UI to be implemented in a usable way that isn't "press key for thing"
		! = donezo
		* GameConsole.AddCallback("god", new Action(GS.ToggleGodMode), "Toggle God Mode");
		* GameConsole.AddCallback("buddha", new Action(GS.ToggleBuddhaMode), "Toggle Buddha Mode (Can get hit, can never die)");
		GameConsole.AddCallback("setsavingallowed", new Action<bool>(GS.ToggleSavingAllowed), "Toggle whether or not the game is allowed to save its data.");
		!* GameConsole.AddCallback("noclip", new Action(GS.ToggleNoclip), "Toggle Noclip");
		!* GameConsole.AddCallback("notarget", new Action(GS.ToggleNotarget), "Toggle Notarget (Enemies cannot see you)");
		GameConsole.AddCallback("kill", new Action(GS.Kill), "Suicide");
		GameConsole.AddCallback("level", new Action<string>(GS.Level), "Loads a level");
		GameConsole.AddCallback("levellist", new Action(GS.LevelList), "Prints list of available levels");
		GameConsole.AddCallback("teleport", new Action<string>(GS.TeleportPlayer), "Teleport Player. Available IDs: " + PlayerTeleport.GetCurrentTeleportIDs());
		GameConsole.AddCallback("win", new Action(GS.WinLevel), "Win current level");
		GameConsole.AddCallback("winmedal", new Action<int>(GS.WinLevelMedal), "Win current level with medal");
		GameConsole.AddCallback("unlocklevels", new Action<bool>(GS.UnlockLevels), "Unlocks all levels for the duration of the current game. Doesn't save.");
		GameConsole.AddCallback("unlockall", new Action<bool>(GS.UnlockAll), "Unlocks everything. This completes your save file. Bool: save");
		GameConsole.AddCallback("deletesave", new Action(GS.DeleteSave), "Delete Save Game Data");
		GameConsole.AddCallback("save", new Action(GS.Save), "Save Game Data");
		* GameConsole.AddCallback("showlevelhints", new Action<bool>(GS.ShowLevelHints), "Enable level hints");
		* GameConsole.AddCallback("showplayerghosts", new Action<bool>(GS.ShowPlayerGhosts), "Enable player ghosts");
		# GameConsole.AddCallback("addcard", new Action<string>(GS.AddCard), "Gives the player a card. Usage: addcard KNIFE");
		GameConsole.AddCallback("cardlist", new Action(GS.CardList), "Prints all possible cards");
		GameConsole.AddCallback("acceptnewmission", new Action(GS.AcceptNewMission), "");
		GameConsole.AddCallback("startmission", new Action(GS.StartMission), "");
		GameConsole.AddCallback("setmission", new Action<string, string>(GS.SetCurrentMission), "Usage: setmission ID_CAMPAIGN ID_MISSION");
		GameConsole.AddCallback("getstorystatus", new Action(GS.GetStoryStatus), "");
		GameConsole.AddCallback("gethubvar", new Action<string>(GS.GetHubVar), "Returns the value of a hub var. Beware, if the var doesn't exist, it will return 0 so don't assume you got it right.");
		GameConsole.AddCallback("sethubvar", new Action<string, int>(GS.SetHubVar), "Set the value of a hub var. Usage: sethubvar VAR_NAME 1");
		GameConsole.AddCallback("collectgift", new Action<string, int>(GS.CollectGift), "Automatically collects gifts for a specific character. Usage: collectgift ACTOR_ID 1");
		GameConsole.AddCallback("collectallgifts", new Action(GS.CollectAllGifts), "Automatically collects 99 gifts for every character. No params");
		GameConsole.AddCallback("replaydialogue", new Action<bool>(GS.SetReplayDialogues), "When enabled, always play dialogues even if they have been seen before.");
		GameConsole.AddCallback("replaycardshowcases", new Action<bool>(GS.SetReplayCardShowcases), "When enabled, always play card showcases even if they have been seen before.");
		GameConsole.AddCallback("showcardshowcases", new Action<bool>(GS.SetShowCardShowcases), "By default, card showcases don't play in editor");
		GameConsole.AddCallback("dialogue", new Action<string>(GS.PlayDebugDialogue), "Play dialogue. Usage: dialogue ID");
		! GameConsole.AddCallback("hud", new Action<bool>(GS.SetHud), "Set HUD visibility. Usage: hud false");
		GameConsole.AddCallback("setmedalcount", new Action<int>(GS.SetMedalCount), "Override player's current medal count. Use a negative value to stop overriding medal count.");
		* GameConsole.AddCallback("autowinmode", new Action<bool>(GS.ToggleAutoWinMode), "Toggle a mode that, if enabled, causes levels to complete automatically.");
		GameConsole.AddCallback("red", new Action(GS.AdvanceRed2), "");
		GameConsole.AddCallback("unlocklevelrush", new Action(GS.CheatUnlockLevelRush), "");
		GameConsole.AddCallback("completearchive", new Action<bool>(GS.SetUnlockArchive), "Force Unlock the Level Archive for all levels");
		GameConsole.AddCallback("maxrelationships", new Action(GS.MaxRelationships), "Max out all relationship stats");
		GameConsole.AddCallback("unlockallmemories", new Action(GS.UnlockAllMemories), "Unlocks all memories (needed to choose the true ending");
		GameConsole.AddCallback("togglefpscounter", new Action(GS.ToggleFPSCounter), "Toggles FPS Counter");
		GameConsole.AddCallback("togglerecordmode", new Action(GS.ToggleRecordMode), "Toggles Record Mode");
		GameConsole.AddCallback("toggleframeratecap", new Action(GS.ToggleFramerateCap), "Toggles Capping Framerate at 60fps. Default: Off");
		GameConsole.AddCallback("dialoguedebug", new Action<bool>(GS.DialogueDebug), "Displays debug information on dialogue stage");
		GameConsole.AddCallback("showrelationshipstatus", new Action(GS.ShowRelationshipStatus), "Displays debug information on dialogue stage");
		GameConsole.AddCallback("modifytickets", new Action<int>(GS.ModifyTopPerformerTickets), "Modifies the amount of Top Performer Tickets held");
		GameConsole.AddCallback("unlockhellrush", new Action(GS.CheatUnlockHellRush), "Unlock Hell Rush");
		* GameConsole.AddCallback("autofastforward", new Action<bool>(GS.AutoFastForward), "Unlock Hell Rush");
		GameConsole.AddCallback("setlanguage", new Action<string>(GS.SetLanguage), "Set Language");
		GameConsole.AddCallback("debugplayfromarchive", new Action<bool>(GS.SetDebugPlayFromArchive), "When loading into a level straight from editor, is it considered an archive level?");
		* GameConsole.AddCallback("alwaysspawngifts", new Action<bool>(GS.SetAlwaysSpawnGifts), "Always spawn Gift Collectibles in level even if they would not otherwise be visible?");
		GameConsole.AddCallback("perfmode", new Action<int>(GS.SetPerformanceMode), "Set the gpu clock speed configuration");
		GameConsole.AddCallback("giftreport", new Action(GS.GenerateGiftReport), "Reports if game content contains the correct amount of gifts for each character's optional content");
*/

namespace NeonWhiteDebugMenu {
    [DisallowMultipleComponent]
    public class ColliderVisualizer : MonoBehaviour {
        // huge shoutout to dmgvol
        // I pretty much ripped this right out of https://github.com/Dmgvol/NeonWhite-Trainer/ to do some debugging of my own

        // TODOs:
        // * hook projectile create to add a collider visualizer to it if collision visualiztion is on
        // * hook up LateUpdate? unsure if that's called at all
        // * figure out tripwire collision boxes & rendering of those
        private static GameObject ColliderVisualizerCanvas {
            get {
                if (ColliderVisualizer._colliderVisualizerCanvas == null) {
                    ColliderVisualizer._colliderVisualizerCanvas = new GameObject("ColliderVisualizerCanvas");
                    ColliderVisualizer._colliderVisualizerCanvas.AddComponent<Canvas>().renderMode = 0;
                    CanvasScaler canvasScaler = ColliderVisualizer._colliderVisualizerCanvas.AddComponent<CanvasScaler>();
                    canvasScaler.uiScaleMode = (CanvasScaler.ScaleMode)1;
                    canvasScaler.referenceResolution = ColliderVisualizer.ReferenceResolution;
                    canvasScaler.matchWidthOrHeight = 1f;
                    ColliderVisualizer._colliderVisualizerCanvas.AddComponent<GraphicRaycaster>();
                }
                return ColliderVisualizer._colliderVisualizerCanvas;
            }
        }

        private static Font Font {
            get {
                Font font;
                if ((font = ColliderVisualizer._font) == null) {
                    font = (ColliderVisualizer._font = Resources.GetBuiltinResource<Font>("Arial.ttf"));
                }
                return font;
            }
        }

        // TODO: figure out hooking this up properly? idt this is called at all rn
        private void LateUpdate() {
            if (this._visualizer == null || this._label == null) {
                return;
            }
            this._label.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, this._visualizer.transform.position);
        }

        private void OnDestroy() {
            if (this._label == null) {
                return;
            }
            UnityEngine.Object.Destroy(this._label.gameObject);
            UnityEngine.Object.Destroy(this._visualizer);
        }

        public void Initialize(ColliderVisualizer.VisualizerColorType visualizerColor, string message, int fontSize) {
            this.Initialize(ColliderVisualizer.VisualizerColorDictionary[visualizerColor], message, fontSize);
        }

        public void Initialize(Color color, string message, int fontSize) {
            Collider component = base.GetComponent<Collider>();
            if (component is BoxCollider) {
                this._visualizer = this.CreateVisualizer((PrimitiveType)3);
                this.SetVisualizerTransform((BoxCollider)component);
            } else if (component is SphereCollider) {
                this._visualizer = this.CreateVisualizer(0);
                this.SetVisualizerTransform((SphereCollider)component);
            } else {
                if (!(component is CapsuleCollider)) {
                    return;
                }
                this._visualizer = this.CreateVisualizer((PrimitiveType)1);
                this.SetVisualizerTransform((CapsuleCollider)component);
            }
            Material material = this._visualizer.GetComponent<Renderer>().material;
            material.shader = Shader.Find("Sprites/Default");
            material.color = color;
            this.CreateLabel(message, fontSize);
        }

        private GameObject CreateVisualizer(PrimitiveType primitiveType) {
            GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
            gameObject.transform.SetParent(base.transform, false);
            Collider component = gameObject.GetComponent<Collider>();
            component.enabled = false;
            UnityEngine.Object.Destroy(component);
            return gameObject;
        }

        private void SetVisualizerTransform(BoxCollider boxCollider) {
            Transform transform = this._visualizer.transform;
            transform.localPosition += boxCollider.center;
            transform.localScale = Vector3.Scale(transform.localScale, boxCollider.size);
        }

        private void SetVisualizerTransform(SphereCollider sphereCollider) {
            Transform transform = this._visualizer.transform;
            transform.localPosition += sphereCollider.center;
            transform.localScale *= sphereCollider.radius * 2f;
        }

        private void SetVisualizerTransform(CapsuleCollider capsuleCollider) {
            Transform transform = this._visualizer.transform;
            transform.localPosition += capsuleCollider.center;
            switch (capsuleCollider.direction) {
                case 0:
                    transform.Rotate(Vector3.forward * 90f);
                    break;
                case 1:
                    break;
                case 2:
                    transform.Rotate(Vector3.right * 90f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Vector3 localScale = transform.localScale;
            float radius = capsuleCollider.radius;
            float num = localScale.x * radius * 2f;
            float num2 = localScale.y * capsuleCollider.height * 0.5f;
            float num3 = localScale.z * radius * 2f;
            transform.localScale = new Vector3(num, num2, num3);
        }

        private void CreateLabel(string message, int fontSize) {
            GameObject gameObject = new GameObject("Label");
            gameObject.transform.SetParent(ColliderVisualizer.ColliderVisualizerCanvas.transform, false);
            this._label = gameObject.AddComponent<Text>();
            this._label.font = ColliderVisualizer.Font;
            this._label.fontSize = fontSize;
            this._label.alignment = (TextAnchor)4;
            this._label.raycastTarget = false;
            this._label.text = message;
            ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = (ContentSizeFitter.FitMode)2;
            contentSizeFitter.verticalFit = (ContentSizeFitter.FitMode)2;
        }

        public void InitializeSpecificCollider(Color color, string message, int fontSize, Collider collider) {
            if (collider is BoxCollider) {
                this._visualizer = this.CreateVisualizer((PrimitiveType)3);
                this.SetVisualizerTransform((BoxCollider)collider);
            } else if (collider is SphereCollider) {
                this._visualizer = this.CreateVisualizer(0);
                this.SetVisualizerTransform((SphereCollider)collider);
            } else {
                if (!(collider is CapsuleCollider)) {
                    return;
                }
                this._visualizer = this.CreateVisualizer((PrimitiveType)1);
                this.SetVisualizerTransform((CapsuleCollider)collider);
            }
            Material material = this._visualizer.GetComponent<Renderer>().material;
            material.shader = Shader.Find("Sprites/Default");
            material.color = color;
            this.CreateLabel(message, fontSize);
        }

        private static readonly Vector2 ReferenceResolution = new Vector2(800f, 600f);

        private static readonly Dictionary<ColliderVisualizer.VisualizerColorType, Color> VisualizerColorDictionary = new Dictionary<ColliderVisualizer.VisualizerColorType, Color>(new ColliderVisualizer.VisualizerColorTypeComparer())
        {
        {
            ColliderVisualizer.VisualizerColorType.Red,
            new Color32(byte.MaxValue, 0, 0, 50)
        },
        {
            ColliderVisualizer.VisualizerColorType.Green,
            new Color32(0, byte.MaxValue, 0, 50)
        },
        {
            ColliderVisualizer.VisualizerColorType.Blue,
            new Color32(0, 0, byte.MaxValue, 50)
        }
    };

        private Text _label;

        private static GameObject _colliderVisualizerCanvas;

        private static Font _font;

        private GameObject _visualizer;

        public enum VisualizerColorType {
            Red,
            Green,
            Blue
        }

        public class VisualizerColorTypeComparer : IEqualityComparer<ColliderVisualizer.VisualizerColorType> {
            public bool Equals(ColliderVisualizer.VisualizerColorType x, ColliderVisualizer.VisualizerColorType y) {
                return x == y;
            }
            public int GetHashCode(ColliderVisualizer.VisualizerColorType obj) {
                return (int)obj;
            }
        }
    }
    class DisablePBUpdating_Patch {
        [HarmonyPatch(typeof(LevelStats), "UpdateTimeMicroseconds")]
        [HarmonyPrefix]
        static bool SkipUpdatingPb(LevelStats __instance, long newTime) {
            __instance._timeLastMicroseconds = newTime;
            return false;
        }
    }

    public class DebugLoggingPatches {
        public static int FramesToLogVelocityInfo = 0;
        public static string GetVelocitiesInfo(string prefix) {
            Vector3 Velocity = RM.drifter.Velocity;
            Vector3 MovementVelocity = RM.drifter.MovementVelocity;
            Vector3 BaseVelocity = RM.drifter.Motor.BaseVelocity; // the real source of truth on momentum
                                                                  // reflection time!
            FieldInfo MoveDirField = typeof(FirstPersonDrifter).GetField("moveDirection", BindingFlags.Instance | BindingFlags.NonPublic);
            Vector3 MoveDirection = (Vector3)MoveDirField.GetValue(RM.drifter);

            // will need to comment this out if i test fireball. yolo
            Velocity.y = 0;
            MovementVelocity.y = 0;
            MoveDirection.y = 0;
            BaseVelocity.y = 0;

            string ret = prefix + "Total Velocity: " + BaseVelocity.magnitude.ToString() + "  |  " + BaseVelocity.x.ToString("N2") + ", " + BaseVelocity.z.ToString("N2");
            ret += prefix + " Bonus Velocity: " + Velocity.magnitude.ToString() + "  |  " + Velocity.x.ToString("N2") + ", " + Velocity.z.ToString("N2");
            ret += prefix + " Movement Velocity: " + MovementVelocity.magnitude.ToString() + "  |  " + MovementVelocity.x.ToString("N2") + ", " + MovementVelocity.z.ToString("N2");
            ret += prefix + "MoveDir: " + MoveDirection.magnitude.ToString() + "  |  " + MoveDirection.x.ToString("N2") + ", " + MoveDirection.z.ToString("N2");

            return ret;
        }

        /*
		[HarmonyPatch(typeof(FirstPersonDrifter), "OnParry")]
        [HarmonyPrefix]
		public static void OnParryCall() {
			MelonLogger.Msg("parry boost multiplier: " + RM.drifter.parryBoostSpeed.ToString() + GetVelocitiesInfo("\n>"));
        }
		[HarmonyPatch(typeof(FirstPersonDrifter), "OnParry")]
		[HarmonyPostfix]
		public static void OnParryReturn() {
			Vector3 PostVelocity = RM.drifter.Velocity;
			PostVelocity.y = 0;
			MelonLogger.Msg("\n<Bonus Velocity (next frame): " + PostVelocity.magnitude.ToString());
			FramesToLogVelocityInfo = 5;
		}
		*/

        [HarmonyPatch(typeof(FirstPersonDrifter), "UpdateVelocity")]
        [HarmonyPostfix]
        public static void LogVelocityTicks(float deltaTime) {
            if (DebugLoggingPatches.FramesToLogVelocityInfo-- > 0) {
                Vector3 velocity = RM.drifter.Motor.BaseVelocity;
                velocity.y = 0;
                string dashing = "[";
                if (RM.drifter.GetIsDashing()) {
                    dashing = "[dashing, ";
                }
                MelonLogger.Msg(dashing + "tick took " + deltaTime.ToString() + "]" +
                    DebugLoggingPatches.GetVelocitiesInfo($"\n[{DebugLoggingPatches.FramesToLogVelocityInfo}] "));
            }
        }

        [HarmonyPatch(typeof(FirstPersonDrifter), "ForceDash")]
        [HarmonyPrefix]
        public static void LogDashSpeeds(float newDashSpeed, float newDashTime, Vector3 newDashEndVelocity, bool isGodspeed) {
            if (isGodspeed) {
                MelonLogger.Msg($"godspeed forceDash of speed {newDashSpeed} for duration {newDashTime}; end velocity of {newDashEndVelocity}");
            } else {
                MelonLogger.Msg($"forceDash of speed {newDashSpeed} for duration {newDashTime}; end velocity of {newDashEndVelocity}");
            }
        }

        /*
        [HarmonyPatch(typeof(FirstPersonDrifter), "ForceJump")]
        [HarmonyPrefix]
		public static void ForceJumpLogging(float upwardVelocity) {
			MelonLogger.Msg("Adding upwards force of " + upwardVelocity.ToString());
			FramesToLogVelocityInfo = 3;
		}

		[HarmonyPatch(typeof(FirstPersonDrifter), "AddVelocity")]
		[HarmonyPrefix]
		public static void AddVelocityLogging(Vector3 vel) {
			float y = vel.y;
			vel.y = 0;
			float speed = vel.magnitude;
			MelonLogger.Msg("Adding velocity of speed " + speed.ToString() + " and upward component " + y.ToString());
			FramesToLogVelocityInfo = 3;
		}

		[HarmonyPatch(typeof(FirstPersonDrifter), "AddExternalVelocity")]
		[HarmonyPrefix]
		public static void AddExternalVelocityLogging(Vector3 vel) {
			float y = vel.y;
			vel.y = 0;
			float speed = vel.magnitude;
			MelonLogger.Msg("Adding extVelocity of speed " + speed.ToString() + " and upward component " + y.ToString());
			FramesToLogVelocityInfo = 3;
		}
		

		[HarmonyPatch(typeof(ShockWeapon), "OnTelefrag")]
		[HarmonyPrefix]
		public static void ShockWeaponOnTelefrag(Vector3 direction) {
			float y = direction.y;
			direction.y = 0;
			float speed = direction.magnitude;
			MelonLogger.Msg("ShockWepTelefrag called; " + (RM.drifter.GetIsDashing() ? "dashing" : "not dashing"));
		}

		[HarmonyPatch(typeof(ShockWeapon), "DoShock")]
		[HarmonyPrefix]
		public static void OnShock(Vector3 forcedDirection, bool useForcedDirection, bool ignorePosition) {
			float y = forcedDirection.y;
			forcedDirection.y = 0;
			float speed = forcedDirection.magnitude;
			if (forcedDirection == Vector3.zero) {
				MelonLogger.Msg("DoShock w/ no forced direction");
            } else {
				MelonLogger.Msg("DoShock called with direction of " + speed.ToString() + " and upward component " + y.ToString());
			}
		}

		[HarmonyPatch(typeof(BaseDamageable), "Die")]
		[HarmonyPrefix]
		public static void BaseShockerDeath(BaseDamageable __instance) {
			EnemyShocker shocker = __instance.GetComponent<EnemyShocker>();
			if (shocker != null) {
				float d = shocker._shockWeapon.dieDelay;
				MelonLogger.Msg("BaseShocker died after delay of " + d.ToString() + "!");
			}
		}
		*/
    }

    public class DebugMenu : MelonMod {
        public static GUIStyle TextStyle(int size) {
            GUIStyle style = new GUIStyle();

            style.fixedHeight = size;
            style.fontSize = size;

            return style;
        }
        public static void DrawText(int x_offset, int y_offset, string s, int size, Color c) {
            GUIStyle style = TextStyle(size);
            style.normal.textColor = c;

            GUIStyle outline_style = TextStyle(size);
            outline_style.normal.textColor = Color.black;
            int outline_strength = 2;

            Rect r = new Rect(x_offset, y_offset, 120, 30);

            for (int i = -outline_strength; i <= outline_strength; ++i) {
                GUI.Label(new Rect(r.x - outline_strength, r.y + i, r.width, r.height), s, outline_style);
                GUI.Label(new Rect(r.x + outline_strength, r.y + i, r.width, r.height), s, outline_style);
            }
            for (int i = -outline_strength + 1; i <= outline_strength - 1; ++i) {
                GUI.Label(new Rect(r.x + i, r.y - outline_strength, r.width, r.height), s, outline_style);
                GUI.Label(new Rect(r.x + i, r.y + outline_strength, r.width, r.height), s, outline_style);
            }
            GUI.Label(r, s, style);
        }
        public static MelonPreferences_Category debug_menu;
        public static MelonPreferences_Category enemy_ai_debug;
        public static MelonPreferences_Category debug_load_level;
        // granular NoTarget prefs
        public static MelonPreferences_Entry<bool> disable_frog;
        public static MelonPreferences_Entry<bool> disable_jock;
        public static MelonPreferences_Entry<bool> disable_jumper;
        public static MelonPreferences_Entry<bool> disable_guardian;
        public static MelonPreferences_Entry<bool> disable_ringer;
        public static MelonPreferences_Entry<bool> disable_shocker;
        public static MelonPreferences_Entry<bool> disable_mimic;
        public static MelonPreferences_Entry<bool> disable_barnacle;

        // noclip; what it says on the box. requires postfix patching FirstPersonDrifter.Start() to update setting.
        public static MelonPreferences_Entry<bool> noclip;
        public static MelonPreferences_Entry<bool> hud;
        public static MelonPreferences_Entry<float> timescale;

        // temp "i wanna load a level" thing
        public static MelonPreferences_Entry<bool> load_on_next_save;
        public static MelonPreferences_Entry<string> level_name;

        // GS only exposes 'toggles' for these, tracking state would be weird
        // they also can collide with each other, so ignoring for now.
        // Properly implementing these would require writing my own dev console :')
        public static MelonPreferences_Entry<bool> record_mode;
        // seems to be implemented in power user prefs. Should be moved to speedometer mod as that's all allowed.
        public static MelonPreferences_Entry<bool> fps_counter;

        class SetNoclipOnStart {
            [HarmonyPatch(typeof(FirstPersonDrifter), "Start")]
            [HarmonyPostfix]
            static void SetNoclip(FirstPersonDrifter __instance) {
                __instance.SetNoclip(noclip.Value);
            }
        }

        class EnemyAI_Patch {
            [HarmonyPatch(typeof(Enemy), "OnUpdate")]
            [HarmonyPrefix]
            // the preference is for "disable", e.g. true == no AI
            // returning true will make enemy .OnUpdate() still tick for that type.
            // so, we need to return !value
            static bool BlockEnemyAI(Enemy __instance) {
                switch (__instance.GetEnemyType()) {
                    case Enemy.Type.shocker: {
                            return !disable_shocker.Value;
                        }
                    case Enemy.Type.barnacle: {
                            return !disable_barnacle.Value;
                        }
                    case Enemy.Type.frog: {
                            return !disable_frog.Value;
                        }
                    case Enemy.Type.jock: {
                            return !disable_jock.Value;
                        }
                    case Enemy.Type.jumper: {
                            return !disable_jumper.Value;
                        }
                    case Enemy.Type.guardian: {
                            return !disable_guardian.Value;
                        }
                    case Enemy.Type.ringer: {
                            return !disable_ringer.Value;
                        }
                    case Enemy.Type.mimic: {
                            return !disable_mimic.Value;
                        }
                    default: {
                            // fallback: who is still enabled?
                            return true;
                        }
                }
            }
        }

        class CollisionVisualizer {
            public static bool enabled = false;
            public static void ToggleState() {
                // thank you dmgvol
                if (enabled) {
                    // turn that shit off
                    ColliderVisualizer[] array2 = Object.FindObjectsOfType<ColliderVisualizer>();
                    for (int j = 0; j < array2.Length; j++) {
                        Object.Destroy(array2[j]);
                    }
                } else {
                    // turn that shit on
                    foreach (Collider collider in from c in Object.FindObjectsOfType<Collider>()
                                                  where !c.isTrigger && c.gameObject.name != "Player" && !c.GetComponent<BaseDamageable>()
                                                  select c) {
                        try {
                            collider.gameObject.AddComponent<ColliderVisualizer>().Initialize((collider.GetType() == typeof(MeshCollider) && !((MeshCollider)collider).convex) ? ColliderVisualizer.VisualizerColorType.Red : ColliderVisualizer.VisualizerColorType.Green, "", 12);
                        } catch {
                        }
                    }
                    BaseDamageable[] array = Object.FindObjectsOfType<BaseDamageable>();
                    for (int i = 0; i < array.Length; i++) {
                        array[i].gameObject.AddComponent<ColliderVisualizer>().Initialize(ColliderVisualizer.VisualizerColorType.Red, "", 12);
                    }
                }
                enabled = !enabled;
            }
            public static void AddInputBinding() {
                InputAction visualizerAction = new InputAction();
                visualizerAction.AddBinding("<Keyboard>/end");
                visualizerAction.Enable();
                visualizerAction.performed += delegate (InputAction.CallbackContext obj) {
                    CollisionVisualizer.ToggleState();
                };
            }
        }

        public override void OnApplicationLateStart() {
            GameDataManager.powerPrefs.dontUploadToLeaderboard = true;
            HarmonyLib.Harmony instance = this.HarmonyInstance;
            instance.PatchAll(typeof(DisablePBUpdating_Patch));
            instance.PatchAll(typeof(EnemyAI_Patch));
            instance.PatchAll(typeof(SetNoclipOnStart));
            instance.PatchAll(typeof(DebugLoggingPatches));

            // set up prefs here
            debug_menu = MelonPreferences.CreateCategory("Debug Menu");
            noclip = debug_menu.CreateEntry("Noclip", false);
            hud = debug_menu.CreateEntry("HUD", true);
            timescale = debug_menu.CreateEntry("Timescale", 1f);

            enemy_ai_debug = MelonPreferences.CreateCategory("Debug: Enemy AI");
            disable_barnacle = enemy_ai_debug.CreateEntry("Disable Barnacle (basic imp)", false);
            disable_frog = enemy_ai_debug.CreateEntry("Disable Frog (yellow)", false);
            disable_jock = enemy_ai_debug.CreateEntry("Disable Jock (blue)", false);
            disable_jumper = enemy_ai_debug.CreateEntry("Disable Jumper (green)", false);
            disable_guardian = enemy_ai_debug.CreateEntry("Disable Guardian", false);
            disable_ringer = enemy_ai_debug.CreateEntry("Disable Ringer (blob)", false);
            disable_shocker = enemy_ai_debug.CreateEntry("Disable Shocker", false);
            disable_mimic = enemy_ai_debug.CreateEntry("Disable Mimic", false);

            debug_load_level = MelonPreferences.CreateCategory("Load Level");
            load_on_next_save = debug_load_level.CreateEntry("Load this level when saving preferences?", false);
            level_name = debug_load_level.CreateEntry("Level name to load", "");


            AddCardBindings();
            CollisionVisualizer.AddInputBinding();
        }

        public override void OnPreferencesSaved() {
            // apply preferences here. every time. yolo.
            if (RM.drifter) {
                RM.drifter.SetNoclip(noclip.Value);
            }
            GS.SetHud(hud.Value);
            if (load_on_next_save.Value) {
                GS.Level(level_name.Value);
                load_on_next_save.Value = false;
            }
        }

        public override void OnUpdate() {
            // note: when timescale fuckery is going on, stuff starts happening when waiting for level start.
            // it resets on level start, but probably need to gate this better (e.g. not adjust timescale if timescale == 0)
            if (RM.time != null && RM.time.GetTargetTimeScale() != 0f) {
                RM.time.SetTargetTimescale(timescale.Value);
            }
        }

        public override void OnGUI() {
            DrawText(0, 0, "Debug kit loaded; PBs disabled!", 12, Color.magenta);
        }

        public void AddCardBindings() {
            InputAction elevateAction = new InputAction();
            elevateAction.AddBinding("<Keyboard>/1", null, null, null);
            elevateAction.Enable();
            elevateAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("PISTOL");
            };

            InputAction purifyAction = new InputAction();
            purifyAction.AddBinding("<Keyboard>/2", null, null, null);
            purifyAction.Enable();
            purifyAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("MACHINEGUN");
            };

            InputAction godspeedAction = new InputAction();
            godspeedAction.AddBinding("<Keyboard>/3", null, null, null);
            godspeedAction.Enable();
            godspeedAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("RIFLE");
            };

            InputAction stompAction = new InputAction();
            stompAction.AddBinding("<Keyboard>/4", null, null, null);
            stompAction.Enable();
            stompAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("UZI");
            };

            InputAction fireballAction = new InputAction();
            fireballAction.AddBinding("<Keyboard>/5", null, null, null);
            fireballAction.Enable();
            fireballAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("SHOTGUN");
            };

            InputAction dominionAction = new InputAction();
            dominionAction.AddBinding("<Keyboard>/6", null, null, null);
            dominionAction.Enable();
            dominionAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("ROCKETLAUNCHER");
            };

            InputAction boofAction = new InputAction();
            boofAction.AddBinding("<Keyboard>/7", null, null, null);
            boofAction.Enable();
            boofAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("RAPTURE");
            };

            InputAction miracleAction = new InputAction();
            miracleAction.AddBinding("<Keyboard>/8", null, null, null);
            miracleAction.Enable();
            miracleAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("KATANA_MIRACLE");
            };

            InputAction revolverAction = new InputAction();
            revolverAction.AddBinding("<Keyboard>/9", null, null, null);
            revolverAction.Enable();
            revolverAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("REVOLVER");
            };

            InputAction resurrectAction = new InputAction();
            resurrectAction.AddBinding("<Keyboard>/0", null, null, null);
            resurrectAction.Enable();
            resurrectAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("RESURRECT");
            };


            InputAction grenadeAction = new InputAction();
            grenadeAction.AddBinding("<Keyboard>/g", null, null, null);
            grenadeAction.Enable();
            grenadeAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("GRENADE");
            };

            InputAction wingsAction = new InputAction();
            wingsAction.AddBinding("<Keyboard>/backspace", null, null, null);
            wingsAction.Enable();
            wingsAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("WINGS");
            };

            InputAction whipAction = new InputAction();
            whipAction.AddBinding("<Keyboard>/b", null, null, null);
            whipAction.Enable();
            whipAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("WHIP");
            };

            InputAction assaultAction = new InputAction();
            assaultAction.AddBinding("<Keyboard>/u", null, null, null);
            assaultAction.Enable();
            assaultAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("ASSAULTRIFLE");
            };

            InputAction knifeAction = new InputAction();
            knifeAction.AddBinding("<Keyboard>/z", null, null, null);
            knifeAction.Enable();
            knifeAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("KNIFE");
            };


            InputAction lanceAction = new InputAction();
            lanceAction.AddBinding("<Keyboard>/l", null, null, null);
            lanceAction.Enable();
            lanceAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("LANCE");
            };

            InputAction maceAction = new InputAction();
            maceAction.AddBinding("<Keyboard>/m", null, null, null);
            maceAction.Enable();
            maceAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("MACE");
            };

            InputAction throwingKnifeAction = new InputAction();
            throwingKnifeAction.AddBinding("<Keyboard>/t", null, null, null);
            throwingKnifeAction.Enable();
            throwingKnifeAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("THROWING_KNIFE");
            };

            InputAction curseAction = new InputAction();
            curseAction.AddBinding("<Keyboard>/h", null, null, null);
            curseAction.Enable();
            curseAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("CURSE");
            };

            InputAction silencerAction = new InputAction();
            silencerAction.AddBinding("<Keyboard>/j", null, null, null);
            silencerAction.Enable();
            silencerAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("PISTOL_SILENCER");
            };

            InputAction shieldAction = new InputAction();
            shieldAction.AddBinding("<Keyboard>/n", null, null, null);
            shieldAction.Enable();
            shieldAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("SHIELD");
            };

            InputAction tempAction = new InputAction();
            tempAction.AddBinding("<Keyboard>/z", null, null, null);
            tempAction.Enable();
            tempAction.performed += delegate (InputAction.CallbackContext obj) {
                GS.AddCard("TEMPLATE");
            };
        }
    }
}

/* card IDs

FISTS
KATANA
KATANA_MIRACLE
MACHINEGUN // purify
PISTOL     // elevate
RIFLE      // godspeed
SHOTGUN    // fireball
UZI        // stomp
ROCKETLAUNCHER 

AMMO
HEALTH

REVOLVER // 9
RESURRECT // 0
WINGS // backsp
CHAINSAW // eh
CLAYMORE // y
GRENADE // g
KATANA_EXPLOSIVE // x
KATANA_STUN // c
KATANA_VAMPIRE // v
KNIFE // z

LANCE // L
MACE // M
RAPIER // R
THROWING_KNIFE // T
WHIP // eh
ASSAULTRIFLE // u
CURSE        // h
PISTOL_SILENCER  // j
PROJECTILE_TESTER // k
SHIELD // n
TEMPLATE // m
RAPTURE // boof?

GIFT_MIKEY
GIFT_RAZ
GIFT_RED
GIFT_VIOLET
GIFT_YELLOW

MEMORY

TUTORIAL_MIRACLE
TUTORIAL_BOMB_JUMP
TUTORIAL_BOMB_JUMP2X
TUTORIAL_DISCARD_PURIFY_RED_DOORS
TUTORIAL_EXPEL
TUTORIAL_RESTART
TUTORIAL_ROCKET_JUMP
TUTORIAL_SIDEQUEST
TUTORIAL_SIDEQUEST_2
TUTORIAL_ZIPLINE
TUTORIAL_CONTROLS_SHOOT
TUTORIAL_CONTROLS_MOVEMENT
TUTORIAL_CONTROLS_JUMP
TUTORIAL_DISCARD_PURIFY_ATTACK
TUTORIAL_FORCEFIELD_ESCAPE

LORE_COLLECTIBLE_EMPTY
LORE_COLLECTIBLE_BIG
LORE_COLLECTIBLE_SMALL

			*/