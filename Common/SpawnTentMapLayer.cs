using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Map;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

using CampingMod.Common.Players;

namespace CampingMod.Common
{
    public class SpawnTentMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text) {
            if (Main.LocalPlayer == null) return;
            var modPlayer = Main.LocalPlayer.GetModPlayer<CampingModPlayer>();

            if(modPlayer.tentSpawn != null) {
                var result = context.Draw(
                    SpawnInterfaceHelper.spawnTent,
                    modPlayer.tentSpawn.Value.ToVector2(),
                    Alignment.Bottom);
                if (result.IsMouseOver) {
                    text = Language.GetTextValue($"{CampingMod.LANG_KEY}Common.TemporarySpawnPoint");
                }
            }
        }
    }
}
