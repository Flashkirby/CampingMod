using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;

namespace Camping.Tiles.Tents
{
    public class CraftTent : CampTent
    {
        int dropItem = 0;

        public override void SetDefaults()
        {
            base.SetDefaults();

            AddMapEntry(new Color(90, 190, 20), CreateMapEntryName());

            Camping.Sets.TemporarySpawn.Add(Type);
            dropItem = ModContent.ItemType<Items.Tents.CraftTent>();

            dustType = -1;
            adjTiles = new int[] { TileID.Beds, TileID.WorkBenches };
        }

        public override void KillMultiTile(int tX, int tY, int pixelX, int pixelY)
        {
            Item.NewItem(tX * 16, tY * 16, 16 * _FRAMEWIDTH, 16 * _FRAMEWIDTH, dropItem);
        }

        public override void MouseOver(int tX, int tY)
        {
            TileUtils.ShowItemIcon(tX, tY, dropItem);
        }
    }
}