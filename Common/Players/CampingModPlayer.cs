using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using CampingMod.Content.Tiles.Tents;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace CampingMod.Common.Players
{
    partial class CampingModPlayer : ModPlayer
    {

        /// <summary>
        /// By default, the player will prefer spawning at a tent (if set) over a bed.
        /// </summary>
        public static bool ChooseToSpawnAtTent = true;

        /// <summary> 
        /// Spawn vector held by the tent.
        /// Null until a tent spawn is set up.
        /// </summary>
        public Point? tentSpawn { get; private set; }

        /// <summary> 
        /// Stores the player's original spawn point during respawn, whilst tent overrides it.
        /// Should only be set to a value for 1 frame (that during which the player spawns)
        /// Null while the player's original spawn point is active
        /// </summary>
        private Vector2? localPermaSpawnCache;

        /// <summary>
        /// Restore spawn point if it's still cached
        /// </summary>
        public override void PreSavePlayer() {
            UseOriginalSpawnPoint();
        }

        /// <summary>
        /// Swap out the spawn positions just before respawning
        /// </summary>
        /// <returns></returns>
        public override void OnRespawn() {
            if (Player == Main.LocalPlayer && ChooseToSpawnAtTent) {
                if (ValidateTentSpawnPoint()) {
                    UseTentSpawnPoint();
                }
            }
        }

        /// <summary>
        /// After respawning, reset the spawnpoint back to normal
        /// </summary>
        /// <returns></returns>
        public override bool PreItemCheck() {
            UseOriginalSpawnPoint();
            return true;
        }

        public void SetTentSpawn(Point? spawnPoint) {
            //Dust.NewDustPerfect(tentSpawn * 16 + new Vector2(8, 8), DustID.AmberBolt).noGravity = true;
            this.tentSpawn = spawnPoint;
            UseOriginalSpawnPoint();
        }

        /// <summary> Restore the original spawn point. </summary>
        private void UseOriginalSpawnPoint() {
            if (localPermaSpawnCache != null) {
                Player.SpawnX = (int)localPermaSpawnCache.Value.X;
                Player.SpawnY = (int)localPermaSpawnCache.Value.Y;
                localPermaSpawnCache = null;
            }
        }

        /// <summary> Switch the spawn point to the tent, and save the current if not done so yet. </summary>
        private void UseTentSpawnPoint() {
            if (tentSpawn != null) {

                // If the player's spawn isn't cached yet
                if (localPermaSpawnCache == null) {
                    // Cache the current player's spawn point
                    localPermaSpawnCache = new Vector2(Player.SpawnX, Player.SpawnY);

                }
                // Swap in the tent's one
                Player.SpawnX = tentSpawn.Value.X;
                Player.SpawnY = tentSpawn.Value.Y;
            }
        }

        /// <summary>
        /// Check if the tent spawn point is valid, and send a chat message if it is not.
        /// </summary>
        /// <returns>true if valid, false if not</returns>
        private bool ValidateTentSpawnPoint() {
            if (tentSpawn != null) {
                int tileType = (int)Main.tile[(int)tentSpawn.Value.X, (int)tentSpawn.Value.Y - 1].TileType;

                if (!CampingMod.Sets.TemporarySpawn.Contains(tileType)) {
                    CampingMod.PrintInfo("CampTent.SpawnMissing");
                    tentSpawn = default;
                    return false;
                }

                if (TileUtils.IsTemporarySpawnObstructed(tentSpawn.Value.X, tentSpawn.Value.Y)) {
                    CampingMod.PrintInfo("CampTent.SpawnBlocked");
                    return false;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Implementation of private method Player.Spawn_SetPosition to fetch spawn tent position as world coords
        /// </summary>
        public Vector2? GetTentSpawnPosition() {
            if (!tentSpawn.HasValue) return null;
            return new Vector2(tentSpawn.Value.X * 16 + 8 - Player.width / 2f, tentSpawn.Value.Y * 16 - Player.height);
        }

        public bool TeleportToTent(PlayerSpawnContext context) {
            if (ValidateTentSpawnPoint()) {
                // Using DoPotionOfReturnTeleportationAndSetTheComebackPoint as a reference
                Player.RemoveAllGrapplingHooks();

                //See also Player.UnityTeleport, and Player.DoPotionOfReturnReturnToOriginalUsePosition
                Vector2 teleportPosition = (Vector2)GetTentSpawnPosition();
                int teleportStyle = TeleportationStyleID.RecallPotion; 
                Player.Teleport(teleportPosition, teleportStyle);
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, Player.position.X, Player.position.Y, teleportStyle);
                NetMessage.SendData(MessageID.PlayerControls, -1, Player.whoAmI, null, Player.whoAmI);
                return true;
            }
            return false;
        }
    }
}
