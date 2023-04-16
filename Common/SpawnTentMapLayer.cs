using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Map;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

using CampingMod.Common.Players;
using Terraria.DataStructures;

namespace CampingMod.Common
{
    public class SpawnTentMapLayer : ModMapLayer
    {
		public override void Draw(ref MapOverlayDrawContext context, ref string text) {
			if (Main.LocalPlayer == null) return;
			var modPlayer = Main.LocalPlayer.GetModPlayer<CampingModPlayer>();

			if (modPlayer.tentSpawn != null) {

				// Disable map interaction if wormhole potion is cancelled (caused by pylon selection)
				bool disableInteraction = Main.cancelWormHole;

				// Draw the map with selection scaling
				var result = context.Draw(
					SpawnInterfaceHelper.spawnTent,
					modPlayer.tentSpawn.Value.ToVector2(), Color.White,
					new SpriteFrame(1, 1, 0, 0), 1f, disableInteraction ? 1f : 2f,
					Alignment.Bottom);

				if (result.IsMouseOver) {
					MouseOverIcon(ref context, ref text, modPlayer);
				}

			}
		}

		private void MouseOverIcon(ref MapOverlayDrawContext context, ref string text, CampingModPlayer modPlayer) {
			string mapIconName = Language.GetTextValue($"{CampingMod.LANG_KEY}Common.TemporarySpawnPoint");
			text = Language.GetTextValue("Game.TeleportTo", mapIconName);

			if (Main.cancelWormHole) return;

			// Click on the map icon
			if (Main.mouseLeft && Main.mouseLeftRelease) {

				if (!modPlayer.Player.HasUnityPotion()) {
					// Can't use this feature without wormhole potions
					string errorText = "lmao"; //Language.GetTextValue("Game.TeleportTo", player[num34].name);

					SystemLoader.PostDrawFullscreenMap(ref errorText);

					CampingMod.PrintInfo("CampTent.CannotTeleportToTentBecauseNotMeetingItemRequirements", Language.GetTextValue("ItemName.WormholePotion"));
				}
				else {
					Main.mouseLeftRelease = false;
                    Main.mapFullscreen = false;

					// Consume wormhole potions
					if (modPlayer.TeleportToTent(PlayerSpawnContext.RecallFromItem)) {
						modPlayer.Player.TakeUnityPotion();
					}
                }

			}

		}
    }
}
