using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Items.Tents
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
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 5); // 5/5 bed
            recipe.AddIngredient(ItemID.Wood, 40); // 26 outpost + 10/10 workbench + 4/4 furnace
            recipe.AddIngredient(ItemID.IronBar, 20); // 8/10 cooking pot + 4/5 anvil + 8/8 heavy workbench
            recipe.AddIngredient(ItemID.Glass, 3); // 1+2/4 2 bottles
            recipe.AddIngredient(ItemID.StoneBlock, 25); // 25/20 furnace
            recipe.AddIngredient(ItemID.FallenStar, 1);
            recipe.AddIngredient(ItemID.LifeCrystal, 1);
            recipe.AddIngredient(ItemID.PiggyBank, 1);
            recipe.AddIngredient(ItemID.Safe, 1);
            recipe.anyWood = true;
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Sawmill);
            recipe.Register();
        }

        public override void UpdateEquip(Player player)
        {
            if (player != Main.LocalPlayer) return;
            player.AddBuff(BuffID.HeartLamp, 2, false);
            player.AddBuff(BuffID.StarInBottle, 2, false);
        }
    }
}