using BepInEx.Configuration;
namespace DPS;
public partial class Settings
{
#nullable disable
  public static bool IsCheats => (ZNet.instance && ZNet.instance.IsServer()) || Console.instance.IsCheatsEnabled();
  public static ConfigEntry<bool> configShowDPS;
  public static bool ShowDPS => configShowDPS.Value;
  public static ConfigEntry<bool> configShowExperience;
  public static bool ShowExperience => configShowExperience.Value;
  private static bool SettingsEnabled => IsCheats && (ShowDPS || ShowExperience);
  private static float FromPercent(int value) => (float)value / 100f;
  public static ConfigEntry<int> configSetSkills;
  public static float SetSkills => SettingsEnabled && configSetSkills.Value != -1 ? FromPercent(configSetSkills.Value) : -1;
  public static ConfigEntry<int> configPlayerDamageRange;
  public static float PlayerDamageRange => SettingsEnabled && configPlayerDamageRange.Value != 15 ? FromPercent(configPlayerDamageRange.Value) : -1;
  public static ConfigEntry<int> configCreatureDamageRange;
  public static float CreatureDamageRange => SettingsEnabled && configCreatureDamageRange.Value != 25 ? FromPercent(configCreatureDamageRange.Value) : -1;
  public static ConfigEntry<int> configMaxAttackChainLevels;
  public static int MaxAttackChainLevels => SettingsEnabled && configMaxAttackChainLevels.Value != -1 ? configMaxAttackChainLevels.Value : -1;
  public static ConfigEntry<bool> configNoStaminaUsage;
  public static bool NoStaminaUsage => SettingsEnabled && configNoStaminaUsage.Value;
  public static ConfigEntry<bool> configAutoFireBow;
  public static bool AutoFireBow => SettingsEnabled && configAutoFireBow.Value;
#nullable enable
  public static void Init(ConfigFile config)
  {
    var section = "DPS";
    configShowDPS = config.Bind(section, "Show DPS meter", false, "Show DPS meter (toggle with P button in the game)");
    configShowDPS.SettingChanged += (s, e) =>
    {
      if (!ShowDPS) DPSMeter.Reset();
    };
    configShowExperience = config.Bind(section, "Show experience meter", false, "Show experience meter (toggle with L button in the game)");
    configShowExperience.SettingChanged += (s, e) =>
    {
      if (!ShowExperience) ExperienceMeter.Reset();
    };
    configSetSkills = config.Bind(section, "Override skill levels", -1, new ConfigDescription("Overrides skill level checks (-1 to disable).", new AcceptableValueRange<int>(-1, 100)));
    configPlayerDamageRange = config.Bind(section, "Min/max damage range for players", 15, new ConfigDescription("Overrides player damage range (15 to disable).", new AcceptableValueRange<int>(0, 40)));
    configCreatureDamageRange = config.Bind(section, "Min/max damage range for creatures", 25, new ConfigDescription("Overrides creature damage range (25 to disable).", new AcceptableValueRange<int>(0, 100), new ConfigurationManagerAttributes() { ShowRangeAsPercent = false }));
    configMaxAttackChainLevels = config.Bind(section, "Maximum attack chain levels", -1, new ConfigDescription("Caps the attack chains for easier testing (-1 to disable).", new AcceptableValueRange<int>(-1, 5)));
    configNoStaminaUsage = config.Bind(section, "No stamina usage", false, "Set true to disable stamina usage.");
    configAutoFireBow = config.Bind(section, "Auto fire bow", false, "Automatically fires the bow when fully drawn.");
  }

  public static bool CheatsRequired() => !IsCheats && (
    configSetSkills.Value >= 0 ||
    configPlayerDamageRange.Value != 15 ||
    configCreatureDamageRange.Value != 25 ||
    configMaxAttackChainLevels.Value >= 0 ||
    configNoStaminaUsage.Value ||
    configAutoFireBow.Value
  );
}
