using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CampingMod.Legacy.ModCompatible
{
    /// <summary>
    /// For tmodloader 1.3, likely can be removed
    /// </summary>
    public class WeaponOutConvertor : GlobalItem
    {
        public static Mod weaponOut = null;

        public override bool IsLoadingEnabled(Mod mod)/* tModPorter Suggestion: If you return false for the purposes of manual loading, use the [Autoload(false)] attribute on your class instead */
        {
            return ModLoader.HasMod("WeaponOut");
        }

        public override bool OnPickup(Item item, Player player)
        {
            ConvertTent(item);
            return true;
        }

        public override void OnCreate(Item item, ItemCreationContext context)
        {
            ConvertTent(item);
        }

        private void ConvertTent(Item item)
        {
            if (item.type == weaponOut.Find<ModItem>("CampTent").Type)
            {
                int stack = item.stack;
                item.type = ItemType<Content.Items.Tents.CampTent>();
                item.SetDefaults(item.type);
                item.stack = stack;
                CampingMod.Print("WeaponOutConvertor.ConvertTent");
            }
            else if (item.type == weaponOut.Find<ModItem>("CampTentMakeshift").Type)
            {
                int stack = item.stack;
                item.type = ItemType<Content.Items.Tents.CampTentMakeshift>();
                item.SetDefaults(item.type);
                item.stack = stack;
                CampingMod.Print("WeaponOutConvertor.ConvertTent");
            }
        }

        public override void Unload() {
            weaponOut = null;
        }
    }
}
