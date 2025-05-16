using BepInEx;
using BepInEx.Configuration;

namespace VoidFogNerf
{
    internal static class VFNConfig
    {
        public static ConfigEntry<float> fogDamageAmp;
        public static ConfigEntry<bool> useCurrentHealth;
        private static ConfigEntry<bool> bypassArmor;
        private static ConfigEntry<bool> bypassBlock;
        private static ConfigEntry<bool> nonlethal;
        public static int damageType;

        public static void InitializeConfig()
        {
            var configFile = new ConfigFile(Paths.ConfigPath + "\\OakPrime.VoidFogNerf.cfg", true);
            useCurrentHealth = configFile.Bind("Main", "Use current health", true, "Change fog damage to percent of current health instead of max health");
            fogDamageAmp = configFile.Bind("Main", "Void Fog damage multiplier", 1.3f, "Multiplier for void fog damage. Default: 1.3 (x current health). "
                + "Vanilla: 1.0 (x max health).");
            bypassArmor = configFile.Bind("Main", "Bypass armor", false, "Void fog bypasses armor and repulsion armor. Default: false. Vanilla: true.");
            bypassBlock = configFile.Bind("Main", "Bypass block", false, "Void fog bypasses block from bears. Default: false. Vanilla: true.");
            nonlethal = configFile.Bind("Main", "Nonlethal", false, "Void fog damage cannot take you below 1 hp.");
            damageType = (int)RoR2.DamageType.BypassArmor | (int)RoR2.DamageType.Generic;
            damageType = (bypassArmor.Value ? (int) RoR2.DamageType.BypassArmor : 0) + (bypassBlock.Value ? (int) RoR2.DamageType.BypassBlock : 0)
                + (nonlethal.Value ? (int) RoR2.DamageType.NonLethal : 0);
        }
    }
}
