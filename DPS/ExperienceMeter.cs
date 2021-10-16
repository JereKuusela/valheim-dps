using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Service;

namespace DPS {
  [HarmonyPatch(typeof(Skills.Skill), "Raise")]
  public class Skill_Raise {
    public static void Prefix(Skills.Skill __instance, float factor) {
      ExperienceMeter.AddExperience(__instance.m_info.m_skill, factor * __instance.m_info.m_increseStep);
    }
  }
  public class ExperienceMeter {
    private static DateTime? startTime = null;
    private static DateTime? endTime = null;
    private static Dictionary<Skills.SkillType, float> experiences = new Dictionary<Skills.SkillType, float>();
    private static Dictionary<Skills.SkillType, int> instances = new Dictionary<Skills.SkillType, int>();
    public static void Start() {
      if (!Settings.ShowExperience) return;
      if (startTime.HasValue) return;
      Reset();
      startTime = DateTime.Now;
    }
    public static bool Running => startTime.HasValue;
    public static void Reset() {
      startTime = null;
      endTime = null;
      experiences.Clear();
      instances.Clear();
    }
    public static void AddExperience(Skills.SkillType skill, float value = 1f) {
      Start();
      if (!startTime.HasValue) return;
      if (!experiences.ContainsKey(skill))
        experiences.Add(skill, 0);
      if (!instances.ContainsKey(skill))
        instances.Add(skill, 0);
      experiences[skill] += value;
      instances[skill]++;
      endTime = DateTime.Now;
    }
    public static float GetExperienceModifier() {
      var seMan = GetValue.Seman(Player.m_localPlayer);
      var mod = 1f;
      seMan.ModifyRaiseSkill(Skills.SkillType.All, ref mod);
      return mod;
    }
    public static float GetLevel(Skills.SkillType type) {
      var skills = GetValue.Skills(Player.m_localPlayer);
      var skill = Call.Skills_GetSkill(skills, type);
      return skill.m_level;
    }
    public static float GetCurrent(Skills.SkillType type) {
      var skills = GetValue.Skills(Player.m_localPlayer);
      var skill = Call.Skills_GetSkill(skills, type);
      return skill.m_accumulator;
    }
    public static float GetTotal(Skills.SkillType type) {
      var skills = GetValue.Skills(Player.m_localPlayer);
      var skill = Call.Skills_GetSkill(skills, type);
      return Call.Skill_GetNextLevelRequirement(skill);
    }
    public static List<string> Get() {
      if (!Settings.ShowExperience) return null;
      var time = 1.0;
      if (startTime.HasValue && endTime.HasValue)
        time = endTime.Value.Subtract(startTime.Value).TotalMilliseconds;
      time /= 60000.0;
      if (time == 0) time = 1;
      var lines = experiences.Select(kvp => {
        var key = kvp.Key;
        var value = kvp.Value;
        var text = key.ToString() + " " + Format.Float(GetLevel(key), color: "white") + " (" + Format.Progress(GetCurrent(key), GetTotal(key)) + "): ";
        // Time doesn't track the first instance so the total value must be scaled accordingly.
        var scaledValue = value * (instances[key] - 1) / instances[key];
        text += Format.Float(kvp.Value) + " (" + Format.Float(scaledValue / time) + " per minute)";
        return text;
      }).ToList();
      lines.Insert(0, "Experience gain: " + Format.Percent(GetExperienceModifier()));
      return lines;
    }
  }
}