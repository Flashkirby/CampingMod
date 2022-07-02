using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;

namespace Camping.Tiles.Tents
{
    public class CampTent : ModTile
    {
        protected const int _FRAMEWIDTH = 5;
        protected const int _FRAMEHEIGHT = 3;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(90, 190, 20), CreateMapEntryName());

            TileID.Sets.HasOutlines[Type] = true;
            Camping.Sets.TemporarySpawn.Add(Type);

            //extra info
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            DustType = -1;
            disableSmartCursor/* tModPorter Note: Removed. Use TileID.Sets.DisableSmartCursor instead */ = true;
            AdjTiles = new int[] { TileID.Beds };
            bed/* tModPorter Note: Removed. Use TileID.Sets.CanBeSleptIn instead */ = true;

            SetTentBaseTileObjectData(_FRAMEWIDTH, _FRAMEHEIGHT);

            // Add mirrored version from base, and commit object data
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
        }

        public static void SetTentBaseTileObjectData(int frameWidth, int frameHeight)
        {
            //style is set up like a bed
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            //strangely enough this means styles are read VERTICALLY
            TileObjectData.newTile.StyleHorizontal = true;
            //width in blocks, and define required ground anchor
            TileObjectData.newTile.Width = frameWidth;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

            //height, and coordinates of each row
            TileObjectData.newTile.Height = frameHeight;
            TileObjectData.newTile.CoordinateHeights = new int[frameHeight];
            for(int i = 0; i < frameHeight; i++)
            { TileObjectData.newTile.CoordinateHeights[i] = 16; }

            //placement centre and offset on ground
            TileObjectData.newTile.Origin = new Point16(2, 2);
            TileObjectData.newTile.DrawYOffset = 2;
        }

        public override void KillMultiTile(int tX, int tY, int pixelX, int pixelY)
        {
            int type = GetItemTypeFromStyle(pixelY);
            if (type > 0)
            { Item.NewItem(tX * 16, tY * 16, 16 * _FRAMEWIDTH, 16 * _FRAMEWIDTH, type); }
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
            TileUtils.GetTentSpawnPosition(tX, tY, out int spawnX, out int spawnY, _FRAMEWIDTH, _FRAMEHEIGHT, 2);
            TileUtils.ToggleTemporarySpawnPoint(modPlayer, spawnX, spawnY);
            return true;
        }

        public override void MouseOver(int tX, int tY)
        {
            TileUtils.ShowItemIcon(tX, tY, GetItemTypeFromStyle(Main.tile[tX, tY].TileFrameY));
        }

        /// <summary>
        /// Calculate which item this tile is assigned to based on which section of the spritesheet the frameY is at.
        /// </summary>
        /// <param name="pixelY">The y pixel position of the spritesheet</param>
        /// <returns></returns>
        private int GetItemTypeFromStyle(int pixelY)
        {
            int style = pixelY / (18 * _FRAMEHEIGHT);
            int type = 0;
            switch (style)
            {
                case 0:
                    type = ModContent.ItemType<Items.Tents.CampTent>();
                    break;
                case 1:
                    type = ModContent.ItemType<Items.Tents.CampTentMakeshift>();
                    break;
            }
            return type;
        }
    }
}