using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using static Terraria.ModLoader.ModContent;
using ReLogic.Content;

using CampingMod.Common.Players;

namespace CampingMod.Content.Tiles.Tents
{
    public class Outpost : ModTile
    {
        protected const int _FRAMEWIDTH = 10;
        protected const int _FRAMEHEIGHT = 5;
        protected const int _ANEMOMETER_FRAMES = 6;
        protected const int _ANEMOMETER_COUNTER = 25;
        static int dropItem = 0;
        static int tileID = -1;

        static Asset<Texture2D> AnemometerTexture;

        static int AnemometerFrameCounter
        {
            get { return Main.tileFrameCounter[tileID]; }
            set { Main.tileFrameCounter[tileID] = value; }
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ) {
                AnemometerTexture = Request<Texture2D>("CampingMod/Content/Tiles/Tents/Outpost_Anemometer");
                tileID = TileType<Outpost>();
            }

            AddMapEntry(new Color(116, 117, 186), CreateMapEntryName());

            // While this is a chest for the purpose of interaction (piggy bank, safe)...
            //TileID.Sets.BasicChest[Type] = true; 
            TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = true;
            // see "DrawEffects" and "SetDrawPositions"
            // ...It isn't a tile that actually stores items that need saving/loading
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

            int logic = GetTileLogic(tX, tY, out int lX, out int lY, out bool mirrored, out _);


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
            int logic = GetTileLogic(tX, tY, out _, out _, out _, out _);
            int itemIcon = dropItem;
            string itemName = "";
            switch (logic)
            {
                case ItemID.PiggyBank:
                    itemIcon = ItemID.PiggyBank; break;
                case ItemID.Safe:
                    itemIcon = ItemID.Safe; break;
                case ItemID.GPS:
                    itemIcon = ItemID.DepthMeter; break;
                case ItemID.WoodenChair:
                    itemIcon = ItemID.WoodenChair;break;
            }
            TileUtils.ShowItemIcon(itemIcon, itemName);
        }

        public override void MouseOverFar(int i, int j) {
            TileUtils.ShowItemIcon(-1, "");
        }

        public override void NearbyEffects(int tX, int tY, bool closer)
        {
            if (Main.LocalPlayer.DeadOrGhost) return;

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
            int logic = GetTileLogic(tX, tY, out _, out _, out _, out bool lightOff);
            float mirroredSign = (tile.TileFrameX >= 18 * _FRAMEWIDTH) ? -1 : 1f;
            switch (logic)
            {
                case ItemID.StarinaBottle:
                    if (lightOff) { break; }
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
                    if (lightOff) { break; }
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
        
        public override void HitWire(int tX, int tY) {
            GetTileLogic(tX, tY, out int localTileX, out int localTileY, out _, out bool lightOff);

            Point topLeft = new Point(tX - localTileX, tY - localTileY);
            short frameAdj = (short)(18 * (lightOff ? -_FRAMEHEIGHT : _FRAMEHEIGHT)); // light on, make off (move to 2nd row), viceversa

            for (int i = 0; i < _FRAMEWIDTH; i++) {
                for (int j = 0; j < _FRAMEHEIGHT; j++) {
                    Main.tile[topLeft.X + i, topLeft.Y + j].TileFrameY += frameAdj;
                    Wiring.SkipWire(topLeft.X + i, topLeft.Y + j);
                }
            }

            // Avoid trying to send packets in singleplayer.
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, _FRAMEWIDTH, _FRAMEHEIGHT, TileChangeType.None);
            }
        }

        private static int GetTileLogic(int tX, int tY, out int localTileX, out int localTileY, out bool mirrored, out bool wireToggled)
        {
            Tile tile = Framing.GetTileSafely(tX, tY);
            mirrored = (tile.TileFrameX >= 18 * _FRAMEWIDTH);
            wireToggled = (tile.TileFrameY >= 18 * _FRAMEHEIGHT);
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
            // unfortunately, this also allows blockswapping with chests via ValidTileForReplacement, which is why
            // DoesntGetReplacedWithTileReplacement must also be set
            TileID.Sets.BasicChest[Type] = true;

            // Add hook on the top left corner tile for SpecialDraw
            bool leftMostFrameTile = drawData.tileFrameX % (18 * _FRAMEWIDTH) == 0;
            bool topMostFrameTile = drawData.tileFrameY % (18 * _FRAMEHEIGHT) == 0;
            if (leftMostFrameTile && topMostFrameTile) {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
            // After the checks, and just before drawing the tile data, disable this tile as being a chest
            // This is because TileDrawing.CacheSpecialDraws attempts to modify tile frames that are in
            // this set, which ends up drawing additional pink lines
            TileID.Sets.BasicChest[Type] = false;
        }
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) {

            // Take the tile, check if it actually exists
            Point p = new Point(i, j);
            Tile tile = Framing.GetTileSafely(p);
            if (tile == null || !tile.HasTile) { return; }
            int direction = (tile.TileFrameX >= 18 * _FRAMEWIDTH) ? -1 : 1;

            Texture2D texture = AnemometerTexture.Value;

            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen) { offScreen = Vector2.Zero; }
            Vector2 worldPos = p.ToWorldCoordinates(
                16f * _FRAMEWIDTH / 2f + 66 * direction, 
                10f);
            Vector2 position = worldPos - Main.screenPosition + offScreen;

            int localFrame = AnemometerFrameCounter / _ANEMOMETER_COUNTER;
            localFrame = (localFrame + (int)(Math.Sin(i) * 3)) % _ANEMOMETER_FRAMES; // wrap to max frames, with sin offset
            if (localFrame < 0) localFrame += _ANEMOMETER_FRAMES;

            if (Framing.GetTileSafely(worldPos.ToTileCoordinates()).WallType > 0) { localFrame = 0; }

            Rectangle frame = texture.Frame(1, _ANEMOMETER_FRAMES, 0, localFrame);

            Color color = Lighting.GetColor(worldPos.ToTileCoordinates());

            Vector2 origin = new Vector2(frame.Width / 2, frame.Height);

            SpriteEffects effects = SpriteEffects.None;

            spriteBatch.Draw(texture, position, frame, color, 0f, origin, 1f, effects, 0f);
        }

        public static void CalculateWindVisual() {
            // 0.1f  = 5 mph, windy day starts at 0.4f
            Main.windSpeedTarget = 0.4f;
            float sensitivity = 20f; // detects changes of at least 2.5mph

            AnemometerFrameCounter += Math.Clamp((int)(Main.WindForVisuals * sensitivity), -5, 5);
            if (AnemometerFrameCounter >= _ANEMOMETER_FRAMES * _ANEMOMETER_COUNTER) {
                AnemometerFrameCounter -= _ANEMOMETER_FRAMES * _ANEMOMETER_COUNTER;
            }
            else if (AnemometerFrameCounter < 0) {
                AnemometerFrameCounter += _ANEMOMETER_FRAMES * _ANEMOMETER_COUNTER;
            }
        }
    }
}