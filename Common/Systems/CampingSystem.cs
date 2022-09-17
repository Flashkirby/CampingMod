using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CampingMod.Common.Systems
{

    public class CampingSystem : ModSystem
    {
        /// <summary>
        /// Display button interface for choosing tent spawn vs home spawn
        /// </summary>
        /// <param name="layers"></param>
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int deathTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
            if (deathTextIndex != -1) {
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
