using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CampingMod.Content.Buffs.Outpost
{
    /// <summary>
    /// Outpost Stage when standing in the outpost
    /// </summary>
    public class OutpostStage2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.accWatch = 3; // Accurate Time
            player.accDepthMeter = 1; // Show Depth
            player.accCompass = 1; // Show Location

            // Timer that counts up as long as the player is unharmed and can naturally regenerate life
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

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int remaining = Math.Max(0, 30 - (int)(Main.LocalPlayer.lifeRegenTime / 60));
            tip = tip.Replace("%TIME", "" + remaining); // See BuffDescription.OutpostStage2
        }
    }
}
