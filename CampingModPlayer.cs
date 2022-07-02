using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

using Terraria.ModLoader.IO;
using CampingMod.Tiles.Tents;

namespace CampingMod
{
    class CampingModPlayer : ModPlayer
    {
        public static bool SpawnAtTent = true;

        /// <summary> Spawn vector held by the tent </summary>
        public Point tentSpawn;

        /// <summary> Stores the player's original spawn point during respawn, whilst tent overrides it. </summary>
        private Vector2? localPermaSpawnCache;

        public override void OnEnterWorld(Player player)
        {
            tentSpawn = default(Point);
            localPermaSpawnCache = null;
            SpawnAtTent = true;
        }

        public override void OnRespawn(Player player)
        {
            if(player == Main.LocalPlayer && SpawnAtTent)
            {
                tentSpawnOverride();
            }
        }

        public override bool PreItemCheck()
        {
            //Dust.NewDustPerfect(tentSpawn * 16 + new Vector2(8, 8), DustID.AmberBolt).noGravity = true;
            tentRestoreCachedSpawn();
            return true;
        }

        private void tentSpawnOverride()
        {
            if (tentSpawn != default(Point)) SetTemporaryRespawn();
        }
        private void tentRestoreCachedSpawn()
        {
            if (localPermaSpawnCache != null)
            {
                Player.SpawnX = (int)((Vector2)localPermaSpawnCache).X;
                Player.SpawnY = (int)((Vector2)localPermaSpawnCache).Y;
                localPermaSpawnCache = null;
            }
        }

        /// <summary> Store spawn in cache and override spawn to tent </summary>
        private void SetTemporaryRespawn()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                int tileType = (int)Main.tile[(int)tentSpawn.X, (int)tentSpawn.Y - 1].TileType;

                if (!CampingMod.Sets.TemporarySpawn.Contains(tileType))
                {
                    CampingMod.PrintInfo("CampTent.SpawnMissing");
                    tentSpawn = default;
                    return;
                }

                if (TileUtils.IsTemporarySpawnObstructed(tentSpawn.X, tentSpawn.Y))
                {
                    CampingMod.PrintInfo("CampTent.SpawnBlocked");
                    return;
                }

                localPermaSpawnCache = new Vector2(Player.SpawnX, Player.SpawnY);
                Player.SpawnX = tentSpawn.X;
                Player.SpawnY = tentSpawn.Y;
            }
        }

    }
}
