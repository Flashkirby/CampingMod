using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Buffs.Outpost
{
    public class OutpostStage2 : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.accWatch = 3; // Accurate Time
            player.accDepthMeter = 1; // Show Depth
            player.accCompass = 1; // Show Location

            if (player.lifeRegenTime >= 15 * 60)
            {
                player.lifeRegenCount += 1;
                player.buffImmune[BuffID.Bleeding] = true;

                if(player.HasBuff(BuffID.Campfire))
                {
                    player.buffImmune[BuffID.Chilled] = true;
                }
            }
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            int remaining = Math.Max(0, 30 - (int)(Main.LocalPlayer.lifeRegenTime / 60));
            tip = tip.Replace("%TIME", "" + remaining);
        }
    }
}
