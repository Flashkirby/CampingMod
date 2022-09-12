using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
{
    [AutoloadEquip(EquipType.Back)]
    public class Outpost : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.value = Item.sellPrice(0, 2, 0, 0); 
            Item.rare = 1;

            Item.useStyle = 1;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.createTile = ModContent.TileType<Tiles.Tents.Outpost>();
            Item.placeStyle = 0;

            Item.accessory = true;

            Item.useTurn = true;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5) // 5/5 bed
                .AddRecipeGroup(RecipeGroupID.Wood, 40) // 26 outpost + 10/10 workbench + 4/4 furnace
                .AddRecipeGroup(RecipeGroupID.IronBar, 20) // 8/10 cooking pot + 4/5 anvil + 8/8 heavy workbench
                .AddIngredient(ItemID.Glass, 3) // 1+2/4 2 bottles
                .AddIngredient(ItemID.StoneBlock, 25) // 25/20 furnace
                .AddIngredient(ItemID.FallenStar, 1)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddIngredient(ItemID.PiggyBank, 1)
                .AddIngredient(ItemID.Safe, 1)
                .AddTile(TileID.Sawmill)
                .Register();
        }

        public override void UpdateEquip(Player player)
        {
            if (player != Main.LocalPlayer) return;
            player.AddBuff(BuffID.HeartLamp, 2, false);
            player.AddBuff(BuffID.StarInBottle, 2, false);
        }
    }
}