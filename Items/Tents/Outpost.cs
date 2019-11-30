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
            item.width = 16;
            item.height = 16;
            item.maxStack = 99;
            item.consumable = true;
            item.value = Item.sellPrice(0, 2, 0, 0); 
            item.rare = 1;

            item.useStyle = 1;
            item.useAnimation = 15;
            item.useTime = 10;
            item.createTile = ModContent.TileType<Tiles.Tents.Outpost>();
            item.placeStyle = 0;

            item.accessory = true;

            item.useTurn = true;
            item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
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
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void UpdateEquip(Player player)
        {
            if (player != Main.LocalPlayer) return;
            player.AddBuff(BuffID.HeartLamp, 2, false);
            player.AddBuff(BuffID.StarInBottle, 2, false);
        }
    }
}