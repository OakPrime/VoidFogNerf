using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace VoidFogNerf
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class VoidFogNerf : BaseUnityPlugin
    {
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "OakPrime";
        public const string PluginName = "VoidFogNerf";
        public const string PluginVersion = "1.2.3";

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            try
            {
                VFNConfig.InitializeConfig();
                IL.RoR2.FogDamageController.MyFixedUpdate += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (VFNConfig.useCurrentHealth.Value)
                    {
                        if (!c.TryGotoNext(
                            x => x.MatchCallOrCallvirt<CharacterBody>("get_healthComponent"),
                            x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth")
                        ))
                        {
                            Logger.LogError("TryGoToNext failed to find fullCombinedHealth instructions in method: " + c.Method);
                            return;
                        }
                        c.Index++;
                        c.RemoveRange(1);
                        c.EmitDelegate<Func<HealthComponent, float>>(healthComponent =>
                        {
                            return healthComponent.combinedHealth;
                        });
                        c.Index++;
                        c.Emit(OpCodes.Ldc_R4, VFNConfig.fogDamageAmp.Value);
                        c.Emit(OpCodes.Mul);
                    }

                    if (!c.TryGotoNext(
                        x => x.MatchLdcI4(0x42),
                        x => x.MatchCallOrCallvirt(out _)
                    ))
                    {
                        Logger.LogError("TryGoToNext failed to find damageType instructions in method: " + c.Method);
                        return;
                    }
                    c.Index++;
                    c.EmitDelegate<Func<int, int>>(damageType =>
                    {
                        return VFNConfig.damageType;
                    });

                };
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + " - " + e.StackTrace);
            }
        }

    }
}
