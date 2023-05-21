using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Items.Tents
{
    [AutoloadEquip(EquipType.Back)]
    public class Outpost : ModItem
    {
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tents.Outpost>(), 0);

            Item.width = 16;
            Item.height = 16;
            Item.value = Item.sellPrice(0, 2, 0, 0); 
            Item.rare = ItemRarityID.Blue;

            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            // raw recipe, but use the 2nd recipe as decrafter
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5) // 5/5 bed
                .AddRecipeGroup(RecipeGroupID.Wood, 40) // 26 outpost + 10/10 workbench + 4/4 furnace
                .AddRecipeGroup(RecipeGroupID.IronBar, 20) // 8/10 cooking pot + 4/5 anvil + 8/8 heavy workbench
                .AddIngredient(ItemID.Glass, 3) // 1+2/4 2 bottles + 1 mug + fallen star
                .AddIngredient(ItemID.StoneBlock, 25) // 25/20 furnace
                .AddIngredient(ItemID.FallenStar, 1)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddIngredient(ItemID.PiggyBank, 1)
                .AddIngredient(ItemID.Safe, 1)
                .AddIngredient(ItemID.TinkerersWorkshop, 1)
                .AddTile(TileID.Sawmill)
                .DisableDecraft()
                .Register();

            // completed items recipe
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5) // 5/5 bed
                .AddRecipeGroup(RecipeGroupID.Wood, 40) // 26 outpost + 10/10 workbench + 4/4 furnace
                .AddRecipeGroup(RecipeGroupID.IronBar, 20) // 8/10 cooking pot + 4/5 anvil + 8/8 heavy workbench
                .AddIngredient(ItemID.StoneBlock, 25) // 25/20 furnace
                .AddIngredient(ItemID.Bottle, 2)
                .AddIngredient(ItemID.Mug, 1)
                .AddIngredient(ItemID.StarinaBottle, 1)
                .AddIngredient(ItemID.HeartLantern, 1)
                .AddIngredient(ItemID.PiggyBank, 1)
                .AddIngredient(ItemID.Safe, 1)
                .AddIngredient(ItemID.TinkerersWorkshop, 1)
                .AddTile(TileID.Sawmill)
                .Register();
        }

        public override void UpdateEquip(Player player) {

            // Star Lantern
            Lighting.AddLight(player.Center + new Vector2(0, -player.height / 3) * player.Directions,
                0.9f - Main.demonTorch * 0.2f,
                0.9f - Main.demonTorch * 0.2f,
                0.7f + Main.demonTorch * 0.2f
                );

            // Heart Lantern
            Lighting.AddLight(player.Center + new Vector2(- player.width / 2, -player.height / 3) * player.Directions,
                1f - Main.demonTorch * 0.1f,
                0.3f - Main.demonTorch * 0.2f,
                0.5f + Main.demonTorch * 0.2f
                );

            if (Main.rand.NextBool(20)) {
                Vector2 starPosition = player.Center - new Vector2(10, player.height * 0.625f + 10);
                Gore star = Gore.NewGoreDirect(Player.GetSource_None(), starPosition, player.velocity / 2, Main.rand.Next(16, 18));
                star.scale *= 0.5f + (float)(Main.rand.NextDouble() * 0.25f);
                star.velocity.Y = (star.velocity.Y + 2f) * 2;
                star.velocity /= 2f;
            }

            // Buff all people around you
            // buffScanArea is in tiles, so * 16 but also we need to halve it so * 8
            Rectangle buffArea = new Rectangle(Main.buffScanAreaWidth * -8, Main.buffScanAreaHeight * -8, Main.buffScanAreaWidth * 16, Main.buffScanAreaHeight * 16);
            buffArea.Location += player.Center.ToPoint();
            foreach (Player receiver in Main.player) {
                if (player.active && !player.DeadOrGhost && buffArea.Contains(receiver.Center.ToPoint())) {
                    receiver.AddBuff(BuffID.HeartLamp, 2, false);
                    receiver.AddBuff(BuffID.StarInBottle, 2, false);
                }
            }
        }
    }
}