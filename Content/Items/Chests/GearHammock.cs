using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Chests
{
    public class GearHammock : ModItem
    {
        // This is buggy and unstable unfortunately, and too much of a hassle to make a hanging chest.
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.value = 50;
            Item.rare = ItemRarityID.White;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.createTile = ModContent.TileType<Tiles.Chests.GearHammock>();

            Item.useTurn = true;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 50)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}