using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using CampingMod.Content.Tiles.Tents;
using Terraria.ID;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace CampingMod.Common.Players
{
    /// <summary>
    /// Vanilla terraria supports up to 200 worlds in a FIFO array.
    /// Because tag compounds are loaded for every player at menu level (not on entering world)
    /// we are going to have to do something similar here, and match it up to the world on entry
    /// </summary>
    partial class CampingModPlayer : ModPlayer
    {
        private static readonly string TAG_SPAWN_ID = "SpawnWorldID"; // an int 200 array
        private static readonly string TAG_SPAWN_X = "SpawnWorldX"; // an int 200 array
        private static readonly string TAG_SPAWN_Y = "SpawnWorldY"; // an int 200 array

        /// <summary>
        /// Spawn Index - this is the data for the save file. It consists of
        /// [][0] TAG_SPAWN_ID current worldID
        /// [][1] TAG_SPAWN_X spawn tile X
        /// [][2] TAG_SPAWN_Y spawn tile Y
        /// </summary>
        private int[] sI = new int[200], sX = new int[200], sY = new int[200];

        /// <summary>
        /// Reset all values on entering the world.
        /// Also attempt to load a saved spawn point, see Player.FindSpawn
        /// </summary>
        public override void OnEnterWorld() {

            localPermaSpawnCache = null;
            ChooseToSpawnAtTent = true;

            tentSpawn = null;
            int worldIndex = new ArrayList(sI).IndexOf(Main.worldID);
            if (worldIndex >= 0) {
                tentSpawn = new Point(sX[worldIndex], sY[worldIndex]);
                Console.WriteLine($"CampingMod: Loaded data for CampingModPlayer:{Player.name}:{Main.worldID} @ {worldIndex}/{sI.Length} ({tentSpawn.Value.X}x{tentSpawn.Value.Y})");
            }
        }

        public override void SaveData(TagCompound tag) {
            string consoleMessage;
            var saveID = new List<int>(sI);
            var saveX = new List<int>(sX);
            var saveY = new List<int>(sY);

            // If already in, find and and remove it
            int index = saveID.IndexOf(Main.worldID);
            if (index >= 0) {
                saveID.RemoveAt(index);
                saveX.RemoveAt(index);
                saveY.RemoveAt(index);
                consoleMessage = $"Removed World ID {Main.worldID}";
            }
            else {
                consoleMessage = $"No existing world ID {Main.worldID}";
            }
            // Then add to the front if its not already there
            if (tentSpawn != null) {
                InsertTruncate(ref saveID, Main.worldID, 200);
                InsertTruncate(ref saveX, tentSpawn.Value.X, 200);
                InsertTruncate(ref saveY, tentSpawn.Value.Y, 200);
                consoleMessage += ", saved tent spawn location";
            }
            else {
                consoleMessage += ", no tent spawn to save";
            }

            tag.Set(TAG_SPAWN_ID, saveID.ToArray(), true);
            tag.Set(TAG_SPAWN_X, saveX.ToArray(), true);
            tag.Set(TAG_SPAWN_Y, saveY.ToArray(), true);
            Console.WriteLine($"CampingMod: Saving data for CampingModPlayer:{Player.name}: {consoleMessage}");
        }

        /// <summary>
        /// Add a new value to the start of a list, truncating any data past the capacity.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="capacity"></param>
        private void InsertTruncate(ref List<int> list, int value, int capacity) {
            list.Insert(0, value);
            try {
                list.RemoveRange(capacity, list.Count - capacity);
            }
            catch (ArgumentException) { }
        }

        public override void LoadData(TagCompound tag) {
            if (tag.TryGet(TAG_SPAWN_ID, out int[] result)) {
                sI = tag.GetIntArray(TAG_SPAWN_ID);
                sX = tag.GetIntArray(TAG_SPAWN_X);
                sY = tag.GetIntArray(TAG_SPAWN_Y);
                Console.WriteLine($"CampingMod: Tags loaded for CampingModPlayer:{Player.name}");
            }
            else {
            }
        }

    }
}
