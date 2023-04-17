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
    public class LargeTent : ModTile
    {
        protected const int _FRAMEWIDTH = 6;
        protected const int _FRAMEHEIGHT = 3;
        int dropItem = 0;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(116, 117, 186), CreateMapEntryName());

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSleptIn[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            CampingMod.Sets.TemporarySpawn.Add(Type);

            dropItem = ModContent.ItemType<Items.Tents.LargeTent>();

            TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable); // Workbenches count as tables for room creation

            //extra info
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            DustType = -1;
            AdjTiles = new int[] {
                    TileID.Beds, TileID.Chairs, TileID.Tables, TileID.Tables2,
                    TileID.WorkBenches, TileID.Bottles, TileID.CookingPots
                };

            CampTent.SetTentBaseTileObjectData(_FRAMEWIDTH, _FRAMEHEIGHT);
            //placement centre and offset on ground
            TileObjectData.newTile.Origin = new Point16(2, 2);

            // Add mirrored version from base, and commit object data
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int tX, int tY, int pixelX, int pixelY)
        {
            Item.NewItem(new EntitySource_TileBreak(tX, tY), tX * 16, tY * 16, 16 * _FRAMEWIDTH, 16 * _FRAMEHEIGHT, dropItem);
        }

        /// <summary>
        /// Allow smart select and drawing _Highlight.png
        /// </summary>
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        { return true; }

        public override bool RightClick(int tX, int tY)
        {
            Tile tile = Framing.GetTileSafely(tX, tY);
            int frameX = (tile.TileFrameX / 18) % _FRAMEWIDTH;
            int direction = (tile.TileFrameX >= 18 * _FRAMEWIDTH) ? -1 : 1;
            int logic = GetTileLogic(tX, tY);

            Player player = Main.LocalPlayer;

            if (logic == ItemID.WoodenChair) {
                int offsetX = (frameX == 1 || frameX == 4) ? -direction : 0;
                TileUtils.SetPlayerSitInChair(player, tX + offsetX, tY);
            }
            else if(logic == ItemID.SleepingIcon) {
                player.GamepadEnableGrappleCooldown();
                player.sleeping.StartSleeping(player, tX, tY);
            }
            else {
                CampingModPlayer modPlayer = player.GetModPlayer<CampingModPlayer>();
                TileUtils.GetTentSpawnPosition(tX, tY, out int spawnX, out int spawnY, _FRAMEWIDTH, _FRAMEHEIGHT, 2, 1);
                TileUtils.ToggleTemporarySpawnPoint(modPlayer, spawnX, spawnY);
            }

            return true;
        }

        private static int GetTileLogic(int tX, int tY) {
            Tile tile = Main.tile[tX, tY];
            bool mirrored = (tile.TileFrameX >= 18 * _FRAMEWIDTH);
            int localTileX = tile.TileFrameX % (18 * _FRAMEWIDTH) / 18;
            int localTileY = tile.TileFrameY % (18 * _FRAMEHEIGHT) / 18;
            if (localTileY == 2) {
                if ((!mirrored && (localTileX == 0 || localTileX == 1))
                    ||
                    (mirrored && (localTileX == 4 || localTileX == 5))) {
                    if(Main.LocalPlayer.IsWithinSnappngRangeToTile(tX, tY, PlayerSittingHelper.ChairSittingMaxDistance)) {
                        return ItemID.WoodenChair; // A chair
                    }
                }
            }
            if (localTileY == 1 || localTileY == 2) {
                if (localTileX == (mirrored ? 3 : 2)) {
                    if(Main.LocalPlayer.IsWithinSnappngRangeToTile(tX, tY, PlayerSleepingHelper.BedSleepingMaxDistance)) {
                        return ItemID.SleepingIcon; // Sleeping
                    }
                }
            }
            return -1;
        }


        public override void ModifySittingTargetInfo(int tX, int tY, ref TileRestingInfo info) {
            // It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for example, use info.restingEntity) {
            Tile tile = Main.tile[tX, tY];
            bool mirrored = (tile.TileFrameX >= 18 * _FRAMEWIDTH);

            info.TargetDirection = mirrored ? 1 : -1;
            info.VisualOffset = new Vector2(-8, 2);

            info.AnchorTilePosition.X = tX;
            info.AnchorTilePosition.Y = tY;
        }

        public override void ModifySleepingTargetInfo(int tX, int tY, ref TileRestingInfo info) {
            Tile tile = Main.tile[tX, tY];
            int localTileY = tile.TileFrameY % (18 * _FRAMEHEIGHT) / 18;
            info.AnchorTilePosition.Y = tY + 1 - localTileY;
            info.DirectionOffset = 4 + 16 * info.TargetDirection;

            // Basically, hide the player when sleeping
            info.VisualOffset.Y -= Main.screenPosition.Y - Player.defaultHeight;
        }

        public override void MouseOver(int tX, int tY)
        {
            int logic = GetTileLogic(tX, tY);
            if (logic == ItemID.WoodenChair) {
                TileUtils.ShowItemIcon(ItemID.BarStool);
            }
            else if (logic == ItemID.SleepingIcon) {
                TileUtils.ShowItemIcon(ItemID.SleepingIcon);
            }
            else {
                TileUtils.ShowItemIcon(dropItem);
            }
        }
    }
}