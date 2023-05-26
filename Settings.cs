using System.Windows.Forms;
using Rage;

namespace EnhancedWeaponControl
{
    internal class Settings
    {

        internal static Keys WeaponSafetyToggle = Keys.K;
        internal static Keys FiringModeToggle = Keys.F5; 
        internal static InitializationFile iniFile;
        
        internal static void Initialize()
        {
            try
            {
                iniFile = new InitializationFile(@"Plugins/EnhancedWeaponControl.ini");
                iniFile.Create();
                WeaponSafetyToggle = iniFile.ReadEnum("Keybinds", "WeaponSafetyToggle", WeaponSafetyToggle);
                FiringModeToggle = iniFile.ReadEnum("Keybinds", "FiringModeToggle", FiringModeToggle);
            }
            catch(System.Exception e)
            {
                string error = e.ToString();
                Game.LogTrivial("NoMinimapOnFoot: ERROR IN 'Settings.cs, Initialize()': " + error);
                Game.DisplayNotification("NoMinimapOnFoot: Error Occured");
            }
        }
    }
}