using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
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
            Item.rare = ItemRarityID.White;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.createTile = ModContent.TileType<Tiles.Tents.CampTent>();
            Item.placeStyle = 0;

            Item.useTurn = true;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 2)
                .AddRecipeGroup(RecipeGroupID.Wood, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}