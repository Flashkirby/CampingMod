using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.GameContent;
using CampingMod.Common.Players;
using CampingMod.Content.Items;
using CampingMod.Content.Tiles.Tents;
using System;

namespace CampingMod.Content.Tiles.Furniture
{
	/// <summary>
	/// Chilling log bench. Grants the bonus effect of calmness if seated near it
	/// </summary>
	public class SittingLog : ModTile
	{
		protected const int _FRAMEWIDTH = 3;
		protected const int _FRAMEHEIGHT = 1;
		int dropItem = 0;

		public override void SetStaticDefaults() {

			AddMapEntry(new Color(191, 142, 111)); // Wood Furniture

			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.CanBeSatOnForNPCs[Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
			TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
			TileID.Sets.DisableSmartCursor[Type] = true;
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

			dropItem = ModContent.ItemType<Items.Furniture.SittingLog>();

			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			DustType = DustID.WoodFurniture;
			AdjTiles = new int[] { TileID.Benches };



			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 18 }; // normally 18 would be 16 + 2 padding, but since we need to include the bottom its 20
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

			// And mirrored
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1); // Facing right will use the second texture style
			TileObjectData.addTile(Type);
		}

		/// <summary>
		/// Calming effect when sat by a campfire
		/// </summary>
		public override void NearbyEffects(int tX, int tY, bool closer) {
			Player player = Main.LocalPlayer;
			bool closest = (int)(player.Bottom.X / 16) == tX && (int)((player.Bottom.Y - 1) / 16) == tY;
			if (closer && Main.SceneMetrics.HasCampfire && player.sitting.isSitting) {
				player.AddBuff(BuffID.Calm, (int)Math.Min(player.lifeRegenTime / 610, 5) * 60 + 30);
			}
		}

        public override void NumDust(int tX, int tY, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

		public override void KillMultiTile(int tX, int tY, int pixelX, int pixelY) {
			Item.NewItem(new EntitySource_TileBreak(tX, tY), tX * 16, tY * 16, 16 * _FRAMEWIDTH, 16 * _FRAMEHEIGHT, dropItem);
		}

		/// <summary>
		/// Allow smart select and drawing _Highlight.png
		/// </summary>
		public override bool HasSmartInteract(int tX, int tY, SmartInteractScanSettings settings) {
			return settings.player.IsWithinSnappngRangeToTile(tX, tY, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range
		}

		public override void ModifySittingTargetInfo(int tX, int tY, ref TileRestingInfo info) {
			// It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for example, use info.restingEntity
			Tile tile = Framing.GetTileSafely(tX, tY);
			bool mirrored = tile.TileFrameX >= 18 * _FRAMEWIDTH;
			int frameX = (tile.TileFrameX / 18) - 1 - 3 * (mirrored ? 1 : 0);

			info.TargetDirection = mirrored ? 1 : -1;
			info.VisualOffset = new Vector2(-2 + -4 * frameX * info.TargetDirection, 0);

			info.AnchorTilePosition.X = tX;
			info.AnchorTilePosition.Y = tY;

		}

		public override bool RightClick(int tX, int tY) {
			Player player = Main.LocalPlayer;

			if (player.IsWithinSnappngRangeToTile(tX, tY, PlayerSittingHelper.ChairSittingMaxDistance)) { // Avoid being able to trigger it from long range
				player.GamepadEnableGrappleCooldown();
				player.sitting.SitDown(player, tX, tY);
			}

			return true;
		}

		public override void MouseOver(int tX, int tY) {
			Player player = Main.LocalPlayer;

			if (!player.IsWithinSnappngRangeToTile(tX, tY, PlayerSittingHelper.ChairSittingMaxDistance)) { // Match condition in RightClick. Interaction should only show if clicking it does something
				return;
			}
			TileUtils.ShowItemIcon(dropItem);
			if (Main.tile[tX, tY].TileFrameX / 18 < _FRAMEWIDTH) {
				player.cursorItemIconReversed = true;
			}
		}
	}
}
