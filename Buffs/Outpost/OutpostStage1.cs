using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Buffs.Outpost
{
    public class OutpostStage1 : ModBuff
    {
        public override void SetDefaults()
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
