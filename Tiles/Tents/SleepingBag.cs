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
    public class SleepingBag : ModTile
    {
        protected const int _FRAMEWIDTH = 4;
        protected const int _FRAMEHEIGHT = 2;
        int dropItem = 0;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(96, 91, 121), CreateMapEntryName());

            TileID.Sets.HasOutlines[Type] = true;
            Camping.Sets.TemporarySpawn.Add(Type);

            dropItem = ModContent.ItemType<Items.Tents.SleepingBag>();

            //extra info
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            DustType = -1;
            disableSmartCursor/* tModPorter Note: Removed. Use TileID.Sets.DisableSmartCursor instead */ = true;
            AdjTiles = new int[] { TileID.Beds };
            bed/* tModPorter Note: Removed. Use TileID.Sets.CanBeSleptIn instead */ = true;

            CampTent.SetTentBaseTileObjectData(_FRAMEWIDTH, _FRAMEHEIGHT);
            //placement centre and offset on ground
            TileObjectData.newTile.Origin = new Point16(1, 1);

            // Add mirrored version from base, and commit object data
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int tX, int tY, int pixelX, int pixelY)
        {
            Item.NewItem(tX * 16, tY * 16, 16 * _FRAMEWIDTH, 16 * _FRAMEWIDTH, dropItem);
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
            TileUtils.GetTentSpawnPosition(tX, tY, out int spawnX, out int spawnY, _FRAMEWIDTH, _FRAMEHEIGHT, 1, 1);
            TileUtils.ToggleTemporarySpawnPoint(modPlayer, spawnX, spawnY);
            return true;
        }

        public override void MouseOver(int tX, int tY)
        {
            TileUtils.ShowItemIcon(tX, tY, dropItem);
        }
    }
}