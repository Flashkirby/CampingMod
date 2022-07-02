using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Items.Tents
{
    public class CampTent : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = 0;

            Item.useStyle = 1;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.createTile = ModContent.TileType<Tiles.Tents.CampTent>();
            Item.placeStyle = 0;

            Item.useTurn = true;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 2);
            recipe.AddIngredient(ItemID.Wood, 8);
            recipe.anyWood = true;
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}