using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Items.Tents
{
    public class CraftTent : CampTent
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.createTile = ModContent.TileType<Tiles.Tents.CraftTent>();
        }
        public override void AddRecipes()
        {
            // Raw craft option
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 2);
            recipe.AddIngredient(ItemID.Wood, 18);
            recipe.anyWood = true;
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();

            // Alternate upgrade option
            recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CampTent>());
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.anyWood = true;
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}