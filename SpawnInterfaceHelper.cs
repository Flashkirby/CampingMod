using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ReLogic.Content;

namespace CampingMod
{
    internal static class SpawnInterfaceHelper
    {
        public const int spawnButtonSize = 44;

        public static Texture2D spawnButtons;

        public static void Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                spawnButtons = mod.Assets.Request<Texture2D>("UI/SpawnButtons", AssetRequestMode.ImmediateLoad).Value;
            }
        }

        public static void Unload()
        {
            spawnButtons = default;
        }

        public static void DrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu) return;
            Player player = Main.LocalPlayer;
            if (player == null) return;
            if (!player.dead) return;
            if (player.GetModPlayer<CampingModPlayer>().tentSpawn == default) return;

            Color deathAlpha = player.GetDeathAlpha(Color.Transparent);

            DrawData homeButton = GenerateUISpawnButton(
                spawnButtons,
                spawnButtonSize,
                0,
                CampingModPlayer.SpawnAtTent ? 1 : 0,
                -26,
                84,
                deathAlpha.A);
            DrawData tentButton = GenerateUISpawnButton(
                spawnButtons,
                spawnButtonSize,
                1,
                CampingModPlayer.SpawnAtTent ? 0 : 1,
                26,
                84,
                deathAlpha.A);

            int hover = 0;
            if (GenerateContainerBox(homeButton).Contains(Main.mouseX, Main.mouseY)) hover = 1;
            if (GenerateContainerBox(tentButton).Contains(Main.mouseX, Main.mouseY)) hover = 2;

            if (hover > 0)// && !Main.blockMouse)
            {
                Main.blockMouse = true;
                bool leftClicked = !Main.mouseRight && Main.mouseLeft && Main.mouseLeftRelease;

                // Prevent item use and show text

                if (hover == 1)
                {
                    Main.hoverItemName = Language.GetTextValue(CampingMod.LANG_KEY + "CampTent.SpawnAtHome");
                    homeButton.color = Color.White;
                    if (leftClicked)
                    {
                        CampingModPlayer.SpawnAtTent = false;
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (hover == 2)
                {
                    Main.hoverItemName = Language.GetTextValue(CampingMod.LANG_KEY + "CampTent.SpawnAtTent");
                    tentButton.color = Color.White;
                    if (leftClicked)
                    {
                        CampingModPlayer.SpawnAtTent = true;
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
            }

            DrawSpriteBatchData(spriteBatch, homeButton);
            DrawSpriteBatchData(spriteBatch, tentButton);
        }

        private static void DrawSpriteBatchData(SpriteBatch spriteBatch, DrawData drawData)
        {
            spriteBatch.Draw(
                drawData.texture,
                drawData.position,
                drawData.sourceRect,
                drawData.color);
        }

        private static DrawData GenerateUISpawnButton(Texture2D texture, int buttonSize, int frameX, int frameY, int drawOffsetX, int drawOffsetY, byte alpha)
        {
            // Get the part of the spritesheet we want to draw
            Rectangle sourceBox = new Rectangle(
                (buttonSize + 2) * frameX,
                (buttonSize + 2) * frameY,
                buttonSize,
                buttonSize);
            // Get the area of the screen we want to draw it to (top left origin)
            Vector2 drawPosition = new Vector2(
                Main.screenWidth / 2 - sourceBox.Width / 2 + drawOffsetX,
                Main.screenHeight / 2 - sourceBox.Height / 2 + drawOffsetY);

            DrawData drawData = new DrawData(
                texture,
                drawPosition,
                sourceBox,
                new Color(alpha, alpha, alpha, alpha)
            );
            return drawData;
        }

        private static Rectangle GenerateContainerBox(DrawData data)
        {
            return new Rectangle(
                (int)(data.position.X), (int)(data.position.Y),
                data.sourceRect.Value.Width, data.sourceRect.Value.Height
                );
        }
    }
}
