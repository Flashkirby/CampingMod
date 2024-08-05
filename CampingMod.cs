using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CampingMod
{
    partial class CampingMod : Mod
    {
        public static bool ThoriumModLoaded { get { return ModLoader.HasMod("ThoriumMod"); } }

        public const string LANG_KEY = "Mods.CampingMod.";

        public CampingMod() {
            Sets.TemporarySpawn = new HashSet<int>();
        }

        public override void Load() {
            SpawnInterfaceHelper.Load(this);
        }

        public override void Unload() {
            Sets.TemporarySpawn = null;
            SpawnInterfaceHelper.Unload();
        }

        public override void PostSetupContent() {
            SetupModIntegration();
        }



        public static class Sets {
            /// <summary> Tiles which are identified as temporary spawn points. </summary>
            public static HashSet<int> TemporarySpawn;
        }

        /// <summary>
        /// Prints a value from the mod's language file
        /// </summary>
        /// <param name="text"></param>
        public static void Print(string text, byte r = 255, byte g = 255, byte b = 255) {
            Main.NewText(Language.GetTextValue(LANG_KEY + text), r, g, b);
        }

        /// <summary>
        /// Prints a value from the mod's language file in Yellow
        /// </summary>
        /// <param name="text"></param>
        public static void PrintInfo(string text) {
            Main.NewText(Language.GetTextValue(LANG_KEY + text), 255, 240, 20);
        }

        /// <summary>
        /// Prints a value from the mod's language file in Yellow, with args used in String.Format (using {0}, {1} etc.)
        /// </summary>
        /// <param name="text"></param>
        public static void PrintInfo(string text, params string[] args) {
            Main.NewText(Language.GetTextValue(LANG_KEY + text, args), 255, 240, 20);
        }
    }
}