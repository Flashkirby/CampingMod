using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using CampingMod.Common.Players;
using Terraria.GameContent;

namespace CampingMod.Content.Tiles.Tents
{
    public class SleepingBag : ModTile
    {
        protected const int _FRAMEWIDTH = 4;
        protected const int _FRAMEHEIGHT = 2;
        int itemIcon = 0;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(96, 91, 121), CreateMapEntryName());

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSleptIn[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            CampingMod.Sets.TemporarySpawn.Add(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation

            itemIcon = ModContent.ItemType<Items.Tents.SleepingBag>();

            //extra info
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            DustType = -1;
            AdjTiles = new int[] { TileID.Beds };

            CampTent.SetTentBaseTileObjectData(_FRAMEWIDTH, _FRAMEHEIGHT);
            //placement centre and offset on ground
            TileObjectData.newTile.Origin = new Point16(1, 1);

            // Add mirrored version from base, and commit object data
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
        }

        /// <summary>
        /// Allow smart select and drawing _Highlight.png
        /// </summary>
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        { return true; }

        // See Example bed for more details
        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY) {
            width = _FRAMEWIDTH / 2;
            height = _FRAMEHEIGHT;
        }

        public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info) {
            // Default values match the regular vanilla bed
            // You might need to mess with the info here if your bed is not a typical 4x2 tile
            info.VisualOffset.Y += 4f; // Move player down a notch because the bed is not as high as a regular bed
        }

        public override bool RightClick(int tX, int tY)
        {
            Player player = Main.LocalPlayer;

            if (!Player.IsHoveringOverABottomSideOfABed(tX, tY)) { // This assumes your bed is 4x2 with 2x2 sections. You have to write your own code here otherwise
                if (player.IsWithinSnappngRangeToTile(tX, tY, PlayerSleepingHelper.BedSleepingMaxDistance)) {
                    player.GamepadEnableGrappleCooldown();
                    player.sleeping.StartSleeping(player, tX, tY);
                }
            }
            else {
                CampingModPlayer modPlayer = player.GetModPlayer<CampingModPlayer>();
                TileUtils.GetTentSpawnPosition(tX, tY, out int spawnX, out int spawnY, _FRAMEWIDTH, _FRAMEHEIGHT, 1, 1);
                TileUtils.ToggleTemporarySpawnPoint(modPlayer, spawnX, spawnY);
            }
            return true;
        }

        public override void MouseOver(int tX, int tY) {
            Player player = Main.LocalPlayer;

            if (!Player.IsHoveringOverABottomSideOfABed(tX, tY)) {
                if (player.IsWithinSnappngRangeToTile(tX, tY, PlayerSleepingHelper.BedSleepingMaxDistance)) { // Match condition in RightClick. Interaction should only show if clicking it does something
                    player.noThrow = 2;
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = ItemID.SleepingIcon;
                }
            }
            else {
                TileUtils.ShowItemIcon(itemIcon);
            }

        }
    }
}