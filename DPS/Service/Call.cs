using System;
using HarmonyLib;

namespace Service {
  [HarmonyPatch]
  public class Call {
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(Character), "GetDamageModifiers")]
    public static HitData.DamageModifiers Character_GetDamageModifiers(Character instance) {
      throw new NotImplementedException("Dummy");
    }
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(Skills), "GetSkill")]
    public static Skills.Skill Skills_GetSkill(Skills instance, Skills.SkillType skillType) {
      throw new NotImplementedException("Dummy");
    }
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(Skills.Skill), "GetNextLevelRequirement")]
    public static float Skill_GetNextLevelRequirement(Skills.Skill instance) {
      throw new NotImplementedException("Dummy");
    }
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(Attack), "GetAttackStamina")]
    public static float Attack_GetAttackStamina(Attack instance) {
      throw new NotImplementedException("Dummy");
    }
  }
}