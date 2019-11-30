using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Camping.Tiles.Chests
{
    /// <summary>
    /// Chests protect tiles under them when they contain items.
    /// This does the same but for the ceiling, since the gear hammock hangs from the ceiling.
    /// 
    /// ... and who wants to crash the game :^)
    /// </summary>
    public class GearHammockTileProtector : GlobalTile
    {
        public override bool CanKillTile(int tX, int tY, int type, ref bool blockDamaged)
        {
            int hammock = TileType<GearHammock>();
            Tile tile = Main.tile[tX, tY];
            Tile below = Main.tile[tX, tY + 1];
            if (tile.type != hammock && below.type == hammock)
            {
                int left = tX - below.frameX / 18 % 3;
                int top = tY + 1 + below.frameY / 18;
                if (!Chest.CanDestroyChest(left, top))
                {
                    blockDamaged = false;
                    return false;
                }
            }
            return true;
        }
    }
}
