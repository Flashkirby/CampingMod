using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace CampingMod
{
    public class CampingMod : Mod
    {
        public const string LANG_KEY = "Mods.CampingMod.";

        public CampingMod()
        {
            Sets.TemporarySpawn = new HashSet<int>();
        }

        public override void Load()
        {
            SpawnInterfaceHelper.Load(this);
        }

        public override void Unload()
        {
            Sets.TemporarySpawn = null;
            SpawnInterfaceHelper.Unload();
            ModCompatible.WeaponOutConvertor.Unload();
        }

        public static class Sets
        {
            /// <summary> Tiles which are identified as temporary spawn points. </summary>
            public static HashSet<int> TemporarySpawn;
        }

        /// <summary>
        /// Prints a value from the mod's language file
        /// </summary>
        /// <param name="text"></param>
        public static void Print(string text, byte r = 255, byte g = 255, byte b = 255)
        {
            Main.NewText(Language.GetTextValue(LANG_KEY + text), r, g, b);
        }

        /// <summary>
        /// Prints a value from the mod's language file in Yellow
        /// </summary>
        /// <param name="text"></param>
        public static void PrintInfo(string text)
        {
            Main.NewText(Language.GetTextValue(LANG_KEY + text), 255, 240, 20);
        }
    }

    public class CampingSystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int deathTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
            if (deathTextIndex != -1)
            {
                layers.Insert(deathTextIndex, new LegacyGameInterfaceLayer(
                    "CampingMod: Spawn Interface",
                    delegate {
                        SpawnInterfaceHelper.DrawInterface(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}