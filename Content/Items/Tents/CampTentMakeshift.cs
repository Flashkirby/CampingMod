using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
{
    /// <summary>
    /// Variant of the camping tent, can't be upgraded, but doesn't need a loom
    /// </summary>
    [AutoloadEquip(EquipType.Back)]
    public class CampTentMakeshift : CampTent
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Item.value = Item.sellPrice(0, 0, 0, 50);
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