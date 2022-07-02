using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Items.Tents
{
    public class LargeTent : CampTent
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(0, 0, 2, 50);
            Item.createTile = ModContent.TileType<Tiles.Tents.LargeTent>();
        }
        public override void AddRecipes()
        {
            // Raw craft option
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 5);
            recipe.AddIngredient(ItemID.Wood, 25);
            recipe.AddIngredient(ItemID.IronBar, 8);
            recipe.AddIngredient(ItemID.Glass, 1);
            recipe.anyWood = true;
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}