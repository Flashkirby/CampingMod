using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.GameInput;
using Terraria.GameContent.Achievements;
using static Terraria.ModLoader.ModContent;
using CampingMod.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace CampingMod.Content.Tiles.Tents
{
    public class Outpost : ModTile
    {
        protected const int _FRAMEWIDTH = 10;
        protected const int _FRAMEHEIGHT = 5;
        int dropItem = 0;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(116, 117, 186), CreateMapEntryName());

            // While this is a chest for the purpose of interaction (piggy bank, safe)...
            //TileID.Sets.BasicChest[Type] = true; 
            // see "DrawEffects" and "SetDrawPositions"

            // It isn't a tile that actually stores items that need saving/loading
            Main.tileContainer[Type] = false;

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSleptIn[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            CampingMod.Sets.TemporarySpawn.Add(Type);

            dropItem = ItemType<Items.Tents.Outpost>();

            //extra info
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            DustType = -1;
            AdjTiles = new int[] {
                    TileID.Beds, TileID.Chairs, TileID.Tables, TileID.Tables2,
                    TileID.WorkBenches, TileID.Bottles, TileID.CookingPots,
                    TileID.Anvils, TileID.Furnaces, TileID.HeavyWorkBench,
                    TileID.PiggyBank, TileID.Safes, TileID.TinkerersWorkbench
                };
            TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
                
            CampTent.SetTentBaseTileObjectData(_FRAMEWIDTH, _FRAMEHEIGHT);
            //placement centre and offset on ground
            TileObjectData.newTile.Origin = new Point16(5, 4);

            // Add mirrored version from base, and commit object data
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int tX, int tY, int pixelX, int pixelY)
        {
            Item.NewItem(new EntitySource_TileBreak(tX, tY), tX * 16, tY * 16, 16 * _FRAMEWIDTH, 16 * _FRAMEWIDTH, dropItem);
        }

        /// <summary>
        /// Allow smart select and drawing _Highlight.png
        /// </summary>
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        { return true; }

        public override bool RightClick(int tX, int tY)
        {
            Player player = Main.LocalPlayer;
            CampingModPlayer modPlayer = player.GetModPlayer<CampingModPlayer>();

            int logic = GetTileLogic(tX, tY, out int lX, out int lY, out bool mirrored);


            // https://github.com/tModLoader/tModLoader/blob/master/ExampleMod/Tiles/ExampleChest.cs
            switch (logic)
            {
                case ItemID.PiggyBank:
                    TileUtils.TogglePiggyBank(tX, tY, player);
                    break;
                case ItemID.Safe:
                    TileUtils.ToggleSafe(tX, tY, player);
                    break;
                case ItemID.GPS:
                    TileUtils.DisplayTimeInChat();
                    break;
                case ItemID.WoodenChair:
                    TileUtils.SetPlayerSitInChair(player, tX, tY);
                    break;
                default:
                    TileUtils.GetTentSpawnPosition(tX, tY, out int spawnX, out int spawnY, _FRAMEWIDTH, _FRAMEHEIGHT, 6, -2);
                    TileUtils.ToggleTemporarySpawnPoint(modPlayer, spawnX, spawnY);
                    break;
            }
            return true;
        }

        public override void MouseOver(int tX, int tY)
        {
            int logic = GetTileLogic(tX, tY, out _, out _, out _);
            int itemIcon = dropItem;
            string itemName = "";
            switch (logic)
            {
                case ItemID.PiggyBank:
                    itemIcon = ItemID.PiggyBank;
                    itemName = Language.GetTextValue("ItemName.PiggyBank");
                    break;
                case ItemID.Safe:
                    itemIcon = ItemID.Safe;
                    itemName = Language.GetTextValue("ItemName.Safe");
                    break;
                case ItemID.GPS:
                    itemIcon = ItemID.DepthMeter; break;
                case ItemID.WoodenChair:
                    itemIcon = ItemID.WoodenChair;break;
            }
            TileUtils.ShowItemIcon(tX, tY, itemIcon, itemName);
        }

        public override void NearbyEffects(int tX, int tY, bool closer)
        {
            bool closest = (int)(Main.LocalPlayer.Center / 16).X == tX && (int)(Main.LocalPlayer.Center / 16).Y == tY;
            if (closest)
            {
                if (Main.LocalPlayer.lifeRegenTime >= 30 * 60)
                {
                    Main.LocalPlayer.ClearBuff(BuffType<Buffs.Outpost.OutpostStage1>());
                    Main.LocalPlayer.ClearBuff(BuffType<Buffs.Outpost.OutpostStage2>());
                    Main.LocalPlayer.AddBuff(BuffType<Buffs.Outpost.OutpostStage3>(), 30, quiet: false);
                }
                else
                {
                    Main.LocalPlayer.ClearBuff(BuffType<Buffs.Outpost.OutpostStage1>());
                    Main.LocalPlayer.AddBuff(BuffType<Buffs.Outpost.OutpostStage2>(), 30, quiet: false);
                    Main.LocalPlayer.ClearBuff(BuffType<Buffs.Outpost.OutpostStage3>());
                }
            }
            else if (!Main.LocalPlayer.HasBuff(BuffType<Buffs.Outpost.OutpostStage2>())
                && !Main.LocalPlayer.HasBuff(BuffType<Buffs.Outpost.OutpostStage3>()))
            {
                Main.LocalPlayer.AddBuff(BuffType<Buffs.Outpost.OutpostStage1>(), 30, quiet: false);
            }

            if (closer)
            {
                Main.SceneMetrics.HasStarInBottle = true;
                Main.SceneMetrics.HasHeartLantern = true;
            }
        }

        public override void ModifyLight(int tX, int tY, ref float r, ref float g, ref float b)
        {
            //https://github.com/tModLoader/tModLoader/blob/master/ExampleMod/Tiles/ExampleTorch.cs
            //star in a bottle emits fallen stars gores

            Tile tile = Main.tile[tX, tY];
            int logic = GetTileLogic(tX, tY, out _, out _, out _);
            float mirroredSign = (tile.TileFrameX >= 18 * _FRAMEWIDTH) ? -1 : 1f;
            switch (logic)
            {
                case ItemID.StarinaBottle:
                    float torchPulse = Main.demonTorch * 0.2f;
                    r = 0.9f - torchPulse;
                    g = 0.9f - torchPulse;
                    b = 0.7f + torchPulse;
                    if (Main.rand.NextBool(20))
                    {
                        Gore star = Gore.NewGoreDirect(new EntitySource_TileUpdate(tX, tY), new Vector2(tX * 16 - 2, tY * 16 - 8), default(Vector2), Main.rand.Next(16, 18));
                        star.scale *= 0.5f + (float)(Main.rand.NextDouble() * 0.25f);
                        star.velocity.Y = (star.velocity.Y + 2f) * 2;
                        star.velocity /= 2f;
                    }
                    break;
                case ItemID.HeartLantern:
                    r = 1f - Main.demonTorch * 0.1f;
                    g = 0.3f - Main.demonTorch * 0.2f;
                    b = 0.5f + Main.demonTorch * 0.2f;
                    break;
                case ItemID.Furnace:
                    r = 0.83f;
                    g = 0.6f;
                    b = 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Dust ember = Dust.NewDustDirect(new Vector2(tX * 16 + 2 + 6 * mirroredSign, tY * 16 + 18), 8, 2, DustID.Torch);
                        ember.noGravity = true;
                        ember.velocity.X *= 0.5f;
                        ember.velocity.Y -= 0.5f;
                        ember.fadeIn = 0.2f;
                    }
                    break;
            }
        }

        //

        private static int GetTileLogic(int tX, int tY, out int localTileX, out int localTileY, out bool mirrored)
        {
            Tile tile = Main.tile[tX, tY];
            mirrored = (tile.TileFrameX >= 18 * _FRAMEWIDTH);
            localTileX = tile.TileFrameX % (18 * _FRAMEWIDTH) / 18;
            localTileY = tile.TileFrameY % (18 * _FRAMEHEIGHT) / 18;
            if (localTileY == 2)
            {
                if ((!mirrored && (localTileX == 1 || localTileX == 2))
                    ||
                    (mirrored && (localTileX == 7 || localTileX == 8)))
                {
                    return ItemID.PiggyBank; // Piggy Bank
                }
            }
            if (localTileY == 3 || localTileY == 4)
            {
                if ((!mirrored && (localTileX == 1 || localTileX == 2))
                    ||
                    (mirrored && (localTileX == 7 || localTileX == 8)))
                {
                    return ItemID.Safe; // Safe
                }
            }
            if (localTileY == 0 || localTileY == 1)
            {
                if ((!mirrored && (localTileX == 8 || localTileX == 9))
                    ||
                    (mirrored && (localTileX == 0 || localTileX == 1)))
                {
                    return ItemID.GPS; // Clock
                }
            }
            if (localTileY == 1)
            {
                if ((!mirrored && localTileX == 0)
                    ||
                    (mirrored && localTileX == 9))
                {
                    return ItemID.HeartLantern; // Heart Lantern
                }
            }
            if (localTileY == 2)
            {
                if ((!mirrored && localTileX == 5)
                    ||
                    (mirrored && localTileX == 4))
                {
                    return ItemID.StarinaBottle; // Star in a Bottle
                }
            }
            if (localTileY == 3)
            {
                if ((!mirrored && localTileX == 8)
                    ||
                    (mirrored && localTileX == 1))
                {
                    return ItemID.Furnace;
                }
            }
            if (localTileY == 4) {
                if ((!mirrored && localTileX == 5 || localTileX == 4)
                    ||
                    (mirrored && localTileX == 4 || localTileX == 5)) {
                    return ItemID.WoodenChair;
                }
            }
            return -1;
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) {
            // It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for example, use info.restingEntity
            Tile tile = Framing.GetTileSafely(i, j);
            bool mirrored = (tile.TileFrameX >= 18 * _FRAMEWIDTH);
            bool furnaceSide = tile.TileFrameX % 36 == 0 ^ mirrored;

            info.TargetDirection = mirrored == furnaceSide ? 1 : -1;
            info.VisualOffset = new Vector2((furnaceSide ? - 8 : - 3), 1);

            info.AnchorTilePosition.X = i;
            info.AnchorTilePosition.Y = j;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
            // Set to true so that the player will recognise this furniture as a chest item
            // this prevents the chest from being closed on the next frame
            TileID.Sets.BasicChest[Type] = true; 
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
            // After the checks, and just before drawing the tile data, disable this tile as being a chest
            // This is because TileDrawing.CacheSpecialDraws attempts to modify tile frames that are in
            // this set, which ends up drawing additional pink lines
            TileID.Sets.BasicChest[Type] = false; 
        }
    }
}