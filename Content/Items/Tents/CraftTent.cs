using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
{
    [AutoloadEquip(EquipType.Back)]
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
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 2)
                .AddRecipeGroup(RecipeGroupID.Wood, 18)
                .AddTile(TileID.WorkBenches)
                .Register();

            // Alternate upgrade option
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CampTent>())
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}