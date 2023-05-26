using System.Collections.Generic;
using System.Security.Policy;
using Rage;
using System.Windows.Forms;
using Rage.Native;
[assembly: Rage.Attributes.Plugin("EnhancedWeaponControl", Author = "Roheat", PrefersSingleInstance = true)]
namespace EnhancedWeaponControl
{
    
    internal class EntryPoint
    {
        internal static Ped MainPlayer => Game.LocalPlayer.Character;
        internal static bool weaponSafety = false;
        internal static bool firstShotTaken = false;
        internal static int firemode = 1;
        
        internal static void Main()
        {
            Game.LogTrivial("Enhanced Weapon Control Loaded");
            Settings.Initialize();
            Game.FrameRender += OnFrameRender;
            while (true)
            {
                GameFiber.Yield();
                OnTick();
            }
        }
        public static void OnFrameRender(object sender, GraphicsEventArgs e)
        {
            if (NativeFunction.Natives.IS_CONTROL_JUST_PRESSED<bool>(0, 24) || firstShotTaken)
            {
                NativeFunction.Natives.DISABLE_PLAYER_FIRING(MainPlayer, true);
                firstShotTaken = true;
            }

            if (NativeFunction.Natives.IS_DISABLED_CONTROL_JUST_RELEASED<bool>(0, 24))
            {
                firstShotTaken = false;
            }
        }
        public static void OnTick()
        {
            if (MainPlayer.Inventory.EquippedWeapon != null && !IsPlayerInVehicle())
            {

                // If the weapon safety feature is turned on, disable the weapon from firing.
                if (weaponSafety)
                {
                    // Disable shooting.
                    NativeFunction.Natives.DISABLE_PLAYER_FIRING(MainPlayer, true);

                    // If the user tries to shoot while the safety is enabled, notify them.
                    if (NativeFunction.Natives.IS_DISABLED_CONTROL_PRESSED<bool>(0, 24))
                    {
                        Game.DisplayNotification($"~r~Weapon safety mode is enabled!~n~~w~Press~y~ {Settings.WeaponSafetyToggle.ToString()} ~w~to switch it off.");
                    }
                }

                // If the player pressed K (311/Rockstar Editor Keyframe Help display button) ON KEYBOARD ONLY(!) then toggle the safety mode.
                if (Game.IsKeyDown(Settings.WeaponSafetyToggle))
                {
                    weaponSafety = !weaponSafety;
                    string weaponSafetyString = (weaponSafety ? "~g~enabled" : "~r~disabled");
                    string oppWeaponSafetyString = (!weaponSafety ? "enable" : "disable");
                    Game.DisplayNotification($"~y~Weapon safety mode: {weaponSafetyString}~n~~y~Press {Settings.WeaponSafetyToggle.ToString()} to {oppWeaponSafetyString}");
                }

                if (Game.IsKeyDown(Settings.FiringModeToggle) && firemode == 2)
                {
                    // (2) Full Auto firing mode
                    firemode = 1;
                    Game.FrameRender -= OnFrameRender;
                    Game.DisplayNotification($"~b~Weapon mode: Full Auto.~n~Press {Settings.FiringModeToggle} to switch.");
                }
                else if (Game.IsKeyDown(Settings.FiringModeToggle) && firemode == 1)
                {
                    // (1) Single shot firing mode
                    firemode = 1;
                    Game.FrameRender += OnFrameRender;
                    Game.DisplayNotification($"~b~Weapon mode: Single Shot.~n~Press {Settings.FiringModeToggle} to switch.");
                }
                // We don't need to have a function that handles firing mode 0, since that's full auto mode and that's enabled by default anyway.
            }
        }
        internal static bool IsPlayerInVehicle() => MainPlayer.IsInAnyVehicle(false);
        
        internal static void OnUnload(bool Exit)
        {
            Game.FrameRender -= OnFrameRender;
            Game.LogTrivial("Enhanced Weapon Control Unloaded");
        }
    }
}