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

			// Prevents wormholing when something else has taken priority, such as teleporting to pylons
			// Note: teleporting to players takes priority over teleporting to tents
			if (Main.cancelWormHole) return;

			// Click on the map icon
			if (Main.mouseLeft && Main.mouseLeftRelease) {

				bool teleport = false;
                Main.mouseLeftRelease = false;
                Main.mapFullscreen = false;

                if (!teleport && CampingMod.ThoriumModLoaded) { teleport = TeleportWithModdedItem(modPlayer, "ThoriumMod", "WormHoleMirror"); }

				if (!teleport) { teleport = TeleportWithWormholdPotion(modPlayer); }

                if (!teleport) {
					// Can't use this feature without wormhole potions
					CampingMod.PrintInfo("CampTent.CannotTeleportToTentBecauseNotMeetingItemRequirements", Language.GetTextValue("ItemName.WormholePotion"));
				}
			}

		}

		private bool TeleportWithWormholdPotion(CampingModPlayer modPlayer)
		{
			if (modPlayer.Player.HasUnityPotion()) {

                // Consume wormhole potions
                if (modPlayer.TeleportToTent(PlayerSpawnContext.RecallFromItem)) {
                    modPlayer.Player.TakeUnityPotion();
                    // Prevent any other wormhole teleports
                    Main.cancelWormHole = true;
                    return true;
                }
            }
			return false;
		}

		private bool TeleportWithModdedItem(CampingModPlayer modPlayer, string modName, string itemName)
		{
            if (ModLoader.TryGetMod(modName, out Mod mod) && mod.TryFind(itemName, out ModItem modItem)) {
				// equivalent to wormhole potion check, but for this specific item
				bool found = false;

                foreach( Item i in modPlayer.Player.inventory) {
					if (i.type == modItem.Type && i.stack > 0) {
						found = true;
						break;
                    }
				}

				if(!found && modPlayer.Player.useVoidBag()) {
                    foreach (Item i in modPlayer.Player.bank4.item) {
                        if (i.type == modItem.Type && i.stack > 0) {
                            found = true;
                            break;
                        }
                    }
                }

				if (found) {
					modPlayer.TeleportToTent(PlayerSpawnContext.RecallFromItem);
					Main.cancelWormHole = true;
					return true;
				}
			}
			return false;
        }
    }
}
