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
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            AddMapEntry(new Color(90, 190, 20), CreateMapEntryName());

            CampingMod.Sets.TemporarySpawn.Add(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable); // Workbenches count as tables for room creation

            DustType = -1;
            AdjTiles = new int[] { TileID.Beds, TileID.WorkBenches };
        }

        public override void MouseOver(int tX, int tY)
        {
            TileUtils.ShowItemIcon(ModContent.ItemType<Items.Tents.CraftTent>());
        }
    }
}