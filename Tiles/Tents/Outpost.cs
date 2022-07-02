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

namespace CampingMod.Tiles.Tents
{
    public class Outpost : ModTile
    {
        protected const int _FRAMEWIDTH = 10;
        protected const int _FRAMEHEIGHT = 5;
        int dropItem = 0;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(116, 117, 186), CreateMapEntryName());

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
                    TileID.PiggyBank, TileID.Safes
                };
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

            int logic = GetTileLogic(tX, tY);


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
                default:
                    TileUtils.GetTentSpawnPosition(tX, tY, out int spawnX, out int spawnY, _FRAMEWIDTH, _FRAMEHEIGHT, 6, -2);
                    TileUtils.ToggleTemporarySpawnPoint(modPlayer, spawnX, spawnY);
                    break;
            }
            return true;
        }

        public override void MouseOver(int tX, int tY)
        {
            int logic = GetTileLogic(tX, tY);
            int itemIcon = dropItem;
            switch (logic)
            {
                case ItemID.PiggyBank:
                    itemIcon = ItemID.PiggyBank; break;
                case ItemID.Safe:
                    itemIcon = ItemID.Safe; break;
                case ItemID.GPS:
                    itemIcon = ItemID.DepthMeter; break;
            }
            TileUtils.ShowItemIcon(tX, tY, itemIcon);
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
            int logic = GetTileLogic(tX, tY);
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

        private static int GetTileLogic(int tX, int tY)
        {
            Tile tile = Main.tile[tX, tY];
            bool mirrored = (tile.TileFrameX >= 18 * _FRAMEWIDTH);
            int localTileX = tile.TileFrameX % (18 * _FRAMEWIDTH) / 18;
            int localTileY = tile.TileFrameY % (18 * _FRAMEHEIGHT) / 18;
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
            return 0;
        }
    }
}