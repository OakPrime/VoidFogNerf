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
        public const string PluginVersion = "0.2.0";

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            try
            {
                IL.RoR2.FogDamageController.FixedUpdate += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.TryGotoNext(
                        x => x.MatchCallOrCallvirt<CharacterBody>("get_healthComponent"),
                        x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth")
                    );
                    c.RemoveRange(2);
                    c.EmitDelegate<Func<CharacterBody, float>>(key =>
                    {
                        return key.healthComponent.combinedHealth;
                    });
                    c.TryGotoNext(
                        x => x.MatchDup(),
                        x => x.MatchLdcI4(0x42),
                        x => x.MatchStfld<DamageInfo>("damageType")
                    );
                    c.RemoveRange(3);
                    

                };
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + " - " + e.StackTrace);
            }
        }

    }
}
