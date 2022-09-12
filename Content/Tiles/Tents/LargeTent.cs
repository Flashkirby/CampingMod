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
            TileUtils.GetTentSpawnPosition(tX, tY, out int spawnX, out int spawnY, _FRAMEWIDTH, _FRAMEHEIGHT, 2, 1);
            TileUtils.ToggleTemporarySpawnPoint(modPlayer, spawnX, spawnY);
            return true;
        }

        public override void MouseOver(int tX, int tY)
        {
            TileUtils.ShowItemIcon(tX, tY, dropItem);
        }
    }
}