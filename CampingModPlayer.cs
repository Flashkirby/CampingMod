﻿using System;
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
using Camping.Tiles.Tents;

namespace Camping
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
                player.SpawnX = (int)((Vector2)localPermaSpawnCache).X;
                player.SpawnY = (int)((Vector2)localPermaSpawnCache).Y;
                localPermaSpawnCache = null;
            }
        }

        /// <summary> Store spawn in cache and override spawn to tent </summary>
        private void SetTemporaryRespawn()
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int tileType = (int)Main.tile[(int)tentSpawn.X, (int)tentSpawn.Y - 1].type;

                if (!Camping.Sets.TemporarySpawn.Contains(tileType))
                {
                    Camping.PrintInfo("CampTent.SpawnMissing");
                    tentSpawn = default;
                    return;
                }

                if (TileUtils.IsTemporarySpawnObstructed(tentSpawn.X, tentSpawn.Y))
                {
                    Camping.PrintInfo("CampTent.SpawnBlocked");
                    return;
                }

                localPermaSpawnCache = new Vector2(player.SpawnX, player.SpawnY);
                player.SpawnX = tentSpawn.X;
                player.SpawnY = tentSpawn.Y;
            }
        }

    }
}
