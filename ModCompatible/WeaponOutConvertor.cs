using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Camping.ModCompatible
{
    public class WeaponOutConvertor : GlobalItem
    {
        public static Mod weaponOut = null;

        public override bool Autoload(ref string name)
        {
            weaponOut = ModLoader.GetMod("WeaponOut");
            return weaponOut != null;
        }

        public override bool OnPickup(Item item, Player player)
        {
            ConvertTent(item);
            return true;
        }

        public override void OnCraft(Item item, Recipe recipe)
        {
            ConvertTent(item);
        }

        private void ConvertTent(Item item)
        {
            if (item.type == weaponOut.ItemType("CampTent"))
            {
                int stack = item.stack;
                item.type = ItemType<Items.Tents.CampTent>();
                item.SetDefaults(item.type);
                item.stack = stack;
                Camping.Print("WeaponOutConvertor.ConvertTent");
            }
            else if (item.type == weaponOut.ItemType("CampTentMakeshift"))
            {
                int stack = item.stack;
                item.type = ItemType<Items.Tents.CampTentMakeshift>();
                item.SetDefaults(item.type);
                item.stack = stack;
                Camping.Print("WeaponOutConvertor.ConvertTent");
            }
        }

        public static void Unload()
        {
            weaponOut = null;
        }
    }
}
