using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Camping.Tiles.Chests
{
    public class GearHammock : ModTile
    {
        protected const int _TILEWIDTH = 3;
        protected const int _TILEHEIGHT = 2;
        protected const int _FRAMEWIDTH = _TILEWIDTH * 18;
        protected const int _FRAMEHEIGHT = _TILEHEIGHT * 18;

        public override void SetDefaults()
        {
            AddMapEntry(new Color(135, 133, 80), CreateMapEntryName());

            // Don't randomise frames
            Main.tileFrameImportant[Type] = true;
            // Don't attach/merge to with tiles
            Main.tileNoAttach[Type] = true;

            // Shine like a chest
            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileValue[Type] = 50;

            //Main.tileShine2[Type] = true;
            //Main.tileShine[Type] = 1200;

            /// Uses _Highlight
            TileID.Sets.HasOutlines[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 }; // Height of each frame
            TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = -4;
            TileObjectData.addTile(Type);

            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Containers };
            chest = Language.GetTextValue(Camping.LANG_KEY + "MapObject.GearHammock");
            chestDrop = ItemType<Items.Chests.GearHammock>();
            dustType = 26;
        }

        public override bool HasSmartInteract() => true;

        public override ushort GetMapOption(int tX, int tY) => (ushort)(Main.tile[tX, tY].frameX / (_FRAMEWIDTH));

        /// <summary> Doesn't seem to actually do anything atm </summary>
        public string MapChestName(string name, int tX, int tY)
        {
            GetTileAndChestPosition(tX, tY, out Tile tile, out int left, out int top);
            int chest = Chest.FindChest(left, top);
            if (chest < 0)
            {
                return Language.GetTextValue("LegacyChestType.0");
            }
            else if (Main.chest[chest].name == "")
            {
                return name;
            }
            else
            {
                return name + ": " + Main.chest[chest].name;
            }
        }

        public override void MouseOver(int tX, int tY)
        {
            Player player = Main.LocalPlayer;
            GetTileAndChestPosition(tX, tY, out Tile tile, out int left, out int top);
            int chest = Chest.FindChest(left, top);
            player.showItemIcon2 = -1;
            if (chest < 0)
            {
                player.showItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                player.showItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : this.chest;
                // Show text, or item icon if the text is the default name
                if (player.showItemIconText == this.chest)
                {
                    player.showItemIcon2 = ItemType<Items.Chests.GearHammock>();
                    player.showItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.showItemIcon = true;
        }

        public override void MouseOverFar(int tX, int tY)
        {
            MouseOver(tX, tY);
            Player player = Main.LocalPlayer;
            if (player.showItemIconText == "")
            {
                player.showItemIcon = false;
                player.showItemIcon2 = 0;
            }
        }

        public override bool NewRightClick(int tX, int tY)
        {
            Player player = Main.LocalPlayer;
            GetTileAndChestPosition(tX, tY, out Tile tile, out int left, out int top);
            Main.mouseRightRelease = false;
            Main.defaultChestName = chest;

            // Close sign if open
            if (player.sign >= 0)
            {
                Main.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }

            // Swap Chests
            if (Main.editChest)
            {
                Main.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }

            // Update Chest name
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }

            if (Main.netMode == 1)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    // If already open, close
                    player.chest = -1;
                    Recipe.FindRecipes();
                    Main.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    // If not opened, attempt to open
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    Main.stackSplit = 600;

                    // Close if this is already open
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        Main.PlaySound(SoundID.MenuClose);
                    }
                    // Otherwise, open sesame
                    else
                    {
                        player.chest = chest;
                        Main.playerInventory = true;
                        Main.recBigList = false;
                        player.chestX = left;
                        player.chestY = top;
                        Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                    }
                    Recipe.FindRecipes();
                }
            }
            return true;
        }

        public override void KillMultiTile(int tX, int tY, int frameX, int frameY)
        {
            Item.NewItem(tX * 16, tY * 16, 16 * _TILEWIDTH, 16 * _TILEHEIGHT, ItemType<Items.Chests.GearHammock>());
            Chest.DestroyChest(tX, tY);
        }

        private static void GetTileAndChestPosition(int tX, int tY, out Tile tile, out int left, out int top)
        {
            tile = Main.tile[tX, tY];
            left = tX;
            top = tY;
            left -= tile.frameX / 18;
            if (tile.frameY != 0)
            { top--; }
        }
    }
}
