using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace CampingMod.Content.Tiles.Chests
{
    public class GearHammock : ModTile
    {
        protected const int _TILEWIDTH = 3;
        protected const int _TILEHEIGHT = 2;
        protected const int _FRAMEWIDTH = _TILEWIDTH * 18;
        protected const int _FRAMEHEIGHT = _TILEHEIGHT * 18;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(135, 133, 80), CreateMapEntryName());

            // Don't randomise frames
            Main.tileFrameImportant[Type] = true;
            // Don't attach/merge to with tiles
            Main.tileNoAttach[Type] = true;

            // Shine like a chest
            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileOreFinderPriority[Type] = 50;

            //Main.tileShine2[Type] = true;
            //Main.tileShine[Type] = 1200;

            /// Uses _Highlight
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.BasicChest[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 }; // Height of each frame
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = -4;
            TileObjectData.addTile(Type);

            // Names
            ContainerName.SetDefault(Language.GetTextValue(CampingMod.LANG_KEY + "MapObject.GearHammock"));

            ModTranslation name = CreateMapEntryName();
            name.SetDefault(Language.GetTextValue(CampingMod.LANG_KEY + "MapObject.GearHammock"));
            AddMapEntry(new Color(200, 200, 200), name, MapChestName);

            AdjTiles = new int[] { TileID.Containers };
            ChestDrop = ItemType<Items.Chests.GearHammock>();
            DustType = DustID.Bone;
        }

        public override bool HasSmartInteract(int tX, int tY, SmartInteractScanSettings settings) => true;

        public override ushort GetMapOption(int tX, int tY) => (ushort)(Main.tile[tX, tY].TileFrameX / (_FRAMEWIDTH));

        public static string MapChestName(string name, int tX, int tY)
        {
            int left = tX;
            int top = tY;
            Tile tile = Main.tile[tX, tY];
            if (tile.TileFrameX % _FRAMEWIDTH != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            int chest = Chest.FindChest(left, top);
            if (chest < 0)
            {
                return Language.GetTextValue("LegacyChestType.0");
            }

            if (Main.chest[chest].name == "")
            {
                return name;
            }

            return name + ": " + Main.chest[chest].name;
        }

        public override bool RightClick(int tX, int tY)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[tX, tY];
            Main.mouseRightRelease = false;
            int left = tX;
            int top = tY;
            if (tile.TileFrameX % _FRAMEWIDTH != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            player.CloseSign();
            player.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = "";
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }

            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    Main.stackSplit = 600;
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        player.OpenChest(left, top, chest);
                    }

                    Recipe.FindRecipes();
                }
            }

            return true;
        }

        public override void KillMultiTile(int tX, int tY, int frameX, int frameY)
        {
            // TODO: check game crashing when destroyed via rocket
            Item.NewItem(new EntitySource_TileBreak(tX, tY), tX * 16, tY * 16, 16 * _TILEWIDTH, 16 * _TILEHEIGHT, ItemType<Items.Chests.GearHammock>());
            Chest.DestroyChest(tX, tY);
        }

        public override void MouseOver(int tX, int tY)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[tX, tY];
            int left = tX;
            int top = tY;
            if (tile.TileFrameX % _FRAMEWIDTH != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            int chest = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chest < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                string defaultName = TileLoader.ContainerName(tile.TileType); // This gets the ContainerName text for the currently selected language
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
                if (player.cursorItemIconText == defaultName)
                {
                    player.cursorItemIconID = ItemType<Items.Chests.GearHammock>();
                    player.cursorItemIconText = "";
                }
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int tX, int tY)
        {
            MouseOver(tX, tY);
            Player player = Main.LocalPlayer;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
    }
}
