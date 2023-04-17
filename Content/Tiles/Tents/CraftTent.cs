using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;

namespace CampingMod.Content.Tiles.Tents
{
    public class CraftTent : CampTent
    {
        int dropItem = 0;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            AddMapEntry(new Color(90, 190, 20), CreateMapEntryName());

            CampingMod.Sets.TemporarySpawn.Add(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable); // Workbenches count as tables for room creation

            dropItem = ModContent.ItemType<Items.Tents.CraftTent>();

            DustType = -1;
            AdjTiles = new int[] { TileID.Beds, TileID.WorkBenches };
        }

        public override void KillMultiTile(int tX, int tY, int pixelX, int pixelY)
        {
            Item.NewItem(new EntitySource_TileBreak(tX, tY), tX * 16, tY * 16, 16 * _FRAMEWIDTH, 16 * _FRAMEHEIGHT, dropItem);
        }

        public override void MouseOver(int tX, int tY)
        {
            TileUtils.ShowItemIcon(dropItem);
        }
    }
}