using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CampingMod
{
    partial class CampingMod : Mod
    {

        protected void SetupModIntegration() {

            // https://steamcommunity.com/sharedfiles/filedetails/?id=2832487441
            ModLoader.TryGetMod("Wikithis", out Mod wikithis);
            if (wikithis != null && !Main.dedServ) {
                wikithis.Call("AddModURL", this, "https://terrariamods.wiki.gg/wiki/Camping/{}");
            }
        }
    }
}