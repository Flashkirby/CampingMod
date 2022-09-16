
using CampingMod.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace CampingMod.Content.Tiles.Tents
{
    internal static class TileUtils
    {
        public static void ShowItemIcon(int tX, int tY, int itemType)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = itemType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tX"></param>
        /// <param name="tY"></param>
        /// <param name="spawnX">The tile spawn X pos</param>
        /// <param name="spawnY">The tile spawn Y pos</param>
        /// <param name="frameWidth">Tile width occupied</param>
        /// <param name="frameHeight">Tile height occupied</param>
        /// <param name="leftOffset">Offset from left side. For a 5 wide, 2 would be the 3rd tile along. </param>
        /// <param name="mirroredOffset">Additional offset on the mirrored right face, after frame width is added. </param>
        public static void GetTentSpawnPosition(int tX, int tY, out int spawnX, out int spawnY, int frameWidth, int frameHeight, int leftOffset, int mirroredOffset = 0)
        {
            Tile tile = Main.tile[tX, tY];
            int localFrameX = tile.TileFrameX % (18 * frameWidth);
            int localFrameY = tile.TileFrameY % (18 * frameHeight);
            spawnX = tX - localFrameX / 18 + leftOffset;
            spawnY = tY - localFrameY / 18 + frameHeight;
            if (tile.TileFrameX >= 18 * frameWidth) spawnX += mirroredOffset;  // Sprite is mirrored, add tile offset
        }

        /// <summary>
        /// Toggle the temporary spawn point of a modplayer and show message as appropriate.
        /// </summary>
        /// <param name="modPlayer"></param>
        /// <param name="spawnX">spawnX position provided by GetTentSpawnPosition</param>
        /// <param name="spawnY">spawnY position provided by GetTentSpawnPosition</param>
        public static void ToggleTemporarySpawnPoint(CampingModPlayer modPlayer, int spawnX, int spawnY)
        {

            if (modPlayer.tentSpawn != new Point(spawnX, spawnY))
            {
                if (IsTemporarySpawnObstructed(spawnX, spawnY))
                {
                    CampingMod.PrintInfo("CampTent.SpawnBlocked");
                    return;
                }

                CampingMod.PrintInfo("CampTent.SpawnSet");
                modPlayer.SetTentSpawn(new Point(spawnX, spawnY));
                if (modPlayer.Player == Main.LocalPlayer)
                {
                    CampingModPlayer.ChooseToSpawnAtTent = true;
                }
            }
            else
            {
                if (modPlayer.Player.SpawnX == -1 && modPlayer.Player.SpawnY == -1)
                {
                    CampingMod.PrintInfo("CampTent.SpawnRemove");
                }
                else
                {
                    CampingMod.PrintInfo("CampTent.SpawnRemoveBed");
                }
                modPlayer.SetTentSpawn(null);
            }
        }

        /// <summary>
        /// Spawn point is the ground i tile under the bed
        /// </summary>
        /// <param name="spawnX"></param>
        /// <param name="spawnY"></param>
        /// <returns></returns>
        public static bool IsTemporarySpawnObstructed(int spawnX, int spawnY)
        {
            for (int x = spawnX - 1; x <= spawnX + 1; x++)
            {
                for (int y = spawnY - 3; y <= spawnY - 1; y++)
                {
                    try
                    {
                        Tile t = Main.tile[x, y];
                        if (t.HasTile && Main.tileSolid[t.TileType])
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is IndexOutOfRangeException) { continue; }
                    }
                }
            }
            return false;
        }

        public static void DisplayTimeInChat()
        {
            // Player.cs@24867 if (Main.tile[myX, myY].type == 104)
            string period = "AM";
            double currentTime = Main.time;

            if (!Main.dayTime)
            {
                currentTime += Main.dayLength; // Night starts here
            }
            currentTime = currentTime / 86400.0 * 24.0; // Convert to 24 Hours
            currentTime = currentTime - 19.5; // Offset by 7:30PM (Start of night
            if (currentTime < 0.0)
            {
                currentTime += 24.0;
            }
            if (currentTime >= 12.0)
            {
                period = "PM";
            }
            int currentHour = (int)currentTime;
            double currentMinute = currentTime - (double)currentHour;
            currentMinute = (int)(currentMinute * 60.0);
            string displayMinute = string.Concat(currentMinute);
            if (currentMinute < 10.0)
            {
                displayMinute = "0" + displayMinute;
            }
            if (currentHour > 12)
            {
                currentHour -= 12;
            }
            if (currentHour == 0)
            {
                currentHour = 12;
            }
            string message = Language.GetTextValue("Game.Time", currentHour + ":" + displayMinute + " " + period);
            Main.NewText(message, byte.MaxValue, 240, 20);
        }

        public static void TogglePiggyBank(int tX, int tY, Player player)
        {
            ToggleOpenSpecialChest(tX, tY, player, -2);
        }

        public static void ToggleSafe(int tX, int tY, Player player)
        {
            ToggleOpenSpecialChest(tX, tY, player, -3);
        }

        public static void ToggleDefenderForge(int tX, int tY, Player player)
        {
            ToggleOpenSpecialChest(tX, tY, player, -4);
        }

        public static void ToggleVoidBag(int tX, int tY, Player player) 
        {
            ToggleOpenSpecialChest(tX, tY, player, -5);
        }

        private static void ToggleOpenSpecialChest(int tX, int tY, Player player, int specialChestType)
        {
            Main.stackSplit = 600;
            if (specialChestType == player.chest)
            {
                player.chest = -1;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            else
            {
                SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                player.OpenChest(tX, tY, specialChestType);
            }
            Recipe.FindRecipes();
        }
    }
}
