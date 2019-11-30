using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Camping.Buffs.Outpost
{
    public class OutpostStage3 : ModBuff
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

            // Being near the outpost boosts natural healing after 15 seconds, by giving a flat bonus to the Regeneration Counter
            // Also clears bleeding after 15 seconds, and reduces other annoying debuffs after 30
            // See https://terraria.gamepedia.com/Health_regeneration#Technical_explanation
            player.lifeRegenCount += 2;
            player.buffImmune[BuffID.Bleeding] = true;

            if (player.HasBuff(BuffID.Campfire))
            {
                player.buffImmune[BuffID.Chilled] = true;
            }

            for (int i = 0; i < player.buffTime.Length; i++)
            {
                if (player.buffType[i] == BuffID.Weak ||
                    player.buffType[i] == BuffID.BrokenArmor ||
                    player.buffType[i] == BuffID.Chilled ||
                    player.buffType[i] == BuffID.Rabies)
                {
                    player.buffTime[i] = Math.Max(0, player.buffTime[i] - 7);
                }
            }
        }
    }
}
