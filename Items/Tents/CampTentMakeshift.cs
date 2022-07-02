using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Items.Tents
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
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cobweb, 30);
            recipe.AddIngredient(ItemID.Wood, 8);
            recipe.anyWood = true;
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}