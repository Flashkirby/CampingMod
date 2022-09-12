using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
{
    public class SleepingBag : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.value = 100;
            Item.rare = 0;

            Item.useStyle = 1;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.createTile = ModContent.TileType<Tiles.Tents.SleepingBag>();
            Item.placeStyle = 0;

            Item.useTurn = true;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}