using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Furniture
{
    public class SittingLog : ModItem
    {
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.SittingLog>());
            Item.maxStack = 99;
            Item.width = 16;
            Item.height = 16;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 3)
                .AddTile(TileID.Campfire)
                .Register();
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 3)
                .AddTile(TileID.Sawmill)
                .Register();

            // Craft back into normal wood at a slight loss (it is a cut log after all)
            Recipe.Create(ItemID.Wood, 2)
                .AddIngredient(this,1)
                .Register();
        }
    }
}
