using HarmonyLib;
using UnityEngine;
namespace DPS;
[HarmonyPatch(typeof(Skills), nameof(Skills.GetSkillFactor))]
public class Skills_GetSkillFactor {
  public static void Postfix(ref float __result) {
    if (Settings.SetSkills < 0) return;
    __result = Settings.SetSkills;
  }
}

[HarmonyPatch(typeof(Skills), nameof(Skills.GetRandomSkillRange))]
public class Skills_GetRandomSkillRange {
  public static void Postfix(Skills __instance, out float min, out float max, Skills.SkillType skillType) {
    // Copy paste from decompiled.
    var range = Settings.PlayerDamageRange >= 0 ? Settings.PlayerDamageRange : 0.15f;
    var skillFactor = __instance.GetSkillFactor(skillType);
    var num = Mathf.Lerp(0.4f, 1f, skillFactor);
    max = Mathf.Clamp01(num + range);
    min = Mathf.Clamp01(num - range);
  }
}
[HarmonyPatch(typeof(Skills), nameof(Skills.GetRandomSkillFactor))]
public class Skills_GetRandomSkillFactor {
  static void Postfix(Skills __instance, ref float __result, Skills.SkillType skillType) {
    if (Settings.PlayerDamageRange < 0) return;
    // Copy paste from decompiled.
    float skillFactor = __instance.GetSkillFactor(skillType);
    float num = Mathf.Lerp(0.4f, 1f, skillFactor);
    float a = Mathf.Clamp01(num - Settings.PlayerDamageRange);
    float b = Mathf.Clamp01(num + Settings.PlayerDamageRange);
    __result = Mathf.Lerp(a, b, UnityEngine.Random.value);
  }
}
[HarmonyPatch(typeof(Character), nameof(Character.GetRandomSkillFactor))]
public class Character_GetRandomSkillFactor {
  static void Postfix(ref float __result) {
    if (Settings.CreatureDamageRange < 0) return;
    __result = UnityEngine.Random.Range(1f - Settings.CreatureDamageRange, 1f);
  }
}
[HarmonyPatch(typeof(Player), nameof(Player.RPC_UseStamina))]
public class Player_RPC_UseStamina {
  static void Prefix(ref float v) {
    if (!Settings.NoStaminaUsage) return;
    v = 0;
  }
}
[HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
public class Attack_Start_CapChain {
  static void Prefix(ref Attack? previousAttack, ref int ___m_currentAttackCainLevel) {
    if (Settings.MaxAttackChainLevels < 0) return;
    if (previousAttack == null) return;
    var nextLevel = previousAttack.m_nextAttackChainLevel;
    if (nextLevel >= Settings.MaxAttackChainLevels) {
      previousAttack = null;
      ___m_currentAttackCainLevel = 0;
    }
  }
}
[HarmonyPatch(typeof(Player), nameof(Player.PlayerAttackInput))]
public class AutoShootBow {
  static void Prefix(Player __instance) {
    if (__instance.InPlaceMode() || !Settings.AutoFireBow) return;
    var currentWeapon = __instance.GetCurrentWeapon();
    if (currentWeapon == null || currentWeapon.m_shared.m_holdDurationMin <= 0f) return;
    if (__instance.GetAttackDrawPercentage() < 1.0f) return;
    __instance.m_attackHold = false;
  }
}
