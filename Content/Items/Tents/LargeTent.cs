using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
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
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5)
                .AddRecipeGroup(RecipeGroupID.Wood, 25)
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddIngredient(ItemID.Glass, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}