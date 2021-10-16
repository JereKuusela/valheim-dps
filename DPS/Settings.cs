using BepInEx.Configuration;
using Service;

namespace DPS {
  public partial class Settings {
    public static ConfigEntry<bool> configShowDPS;
    public static bool ShowDPS => configShowDPS.Value;
    public static ConfigEntry<bool> configShowExperience;
    public static bool ShowExperience => configShowExperience.Value;
    private static bool settingsEnabled => Admin.Enabled && (ShowDPS || ShowExperience);
    private static float fromPercent(int value) => (float)value / 100f;
    public static ConfigEntry<int> configSetSkills;
    public static float SetSkills => settingsEnabled && configSetSkills.Value != -1 ? fromPercent(configSetSkills.Value) : -1;
    public static ConfigEntry<int> configPlayerDamageRange;
    public static float PlayerDamageRange => settingsEnabled && configPlayerDamageRange.Value != 15 ? fromPercent(configPlayerDamageRange.Value) : -1;
    public static ConfigEntry<int> configCreatureDamageRange;
    public static float CreatureDamageRange => settingsEnabled && configCreatureDamageRange.Value != 25 ? fromPercent(configCreatureDamageRange.Value) : -1;
    public static ConfigEntry<int> configMaxAttackChainLevels;
    public static int MaxAttackChainLevels => settingsEnabled && configMaxAttackChainLevels.Value != -1 ? configMaxAttackChainLevels.Value : -1;
    public static ConfigEntry<bool> configNoStaminaUsage;
    public static bool NoStaminaUsage => settingsEnabled && configNoStaminaUsage.Value;
    public static void Init(ConfigFile config) {
      var section = "DPS";
      configShowDPS = config.Bind(section, "Show DPS meter", false, "Show DPS meter (toggle with P button in the game)");
      configShowDPS.SettingChanged += (s, e) => {
        if (ShowDPS) Admin.Check();
        if (!ShowDPS) DPSMeter.Reset();
      };
      configShowExperience = config.Bind(section, "Show experience meter", false, "Show experience meter (toggle with L button in the game)");
      configShowExperience.SettingChanged += (s, e) => {
        if (ShowExperience) Admin.Check();
        if (!ShowExperience) ExperienceMeter.Reset();
      };
      configSetSkills = config.Bind(section, "Override skill levels", -1, new ConfigDescription("Overrides skill level checks (-1 to disable).", new AcceptableValueRange<int>(-1, 100)));
      configPlayerDamageRange = config.Bind(section, "Min/max damage range for players", 15, new ConfigDescription("Overrides player damage range (15 to disable).", new AcceptableValueRange<int>(0, 40)));
      configCreatureDamageRange = config.Bind(section, "Min/max damage range for creatures", 25, new ConfigDescription("Overrides creature damage range (25 to disable).", new AcceptableValueRange<int>(0, 100), new ConfigurationManagerAttributes() { ShowRangeAsPercent = false }));
      configMaxAttackChainLevels = config.Bind(section, "Maximum attack chain levels", -1, new ConfigDescription("Caps the attack chains for easier testing (-1 to disable).", new AcceptableValueRange<int>(-1, 5)));
      configNoStaminaUsage = config.Bind(section, "No stamina usage", false, "Set true to disable stamina usage");
    }

    public static bool AdminRequired() => !Admin.Enabled && (
      configSetSkills.Value >= 0 ||
      configPlayerDamageRange.Value != 15 ||
      configCreatureDamageRange.Value != 25 ||
      configMaxAttackChainLevels.Value >= 0 ||
      configNoStaminaUsage.Value
    );
  }
}
