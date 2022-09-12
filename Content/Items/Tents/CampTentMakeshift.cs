using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
{
    public class CampTentMakeshift : CampTent
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.placeStyle = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 30)
                .AddRecipeGroup(RecipeGroupID.Wood, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}