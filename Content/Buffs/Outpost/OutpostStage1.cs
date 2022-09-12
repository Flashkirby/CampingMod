using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Buffs.Outpost
{
    /// <summary>
    /// Outpost Stage when near an outpost, but not by the actual tile
    /// </summary>
    public class OutpostStage1 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.accWatch = 2; // Half Hour Time
        }
    }
}
