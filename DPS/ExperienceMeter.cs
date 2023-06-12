using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Service;
namespace DPS;
[HarmonyPatch(typeof(Skills.Skill), nameof(Skills.Skill.Raise))]
public class Skill_Raise
{
  static void Prefix(Skills.Skill __instance, float factor)
  {
    ExperienceMeter.AddExperience(__instance.m_info.m_skill, factor * __instance.m_info.m_increseStep);
  }
}
public class ExperienceMeter
{
  private static DateTime? startTime = null;
  private static DateTime? endTime = null;
  private static readonly Dictionary<Skills.SkillType, float> experiences = new();
  private static readonly Dictionary<Skills.SkillType, int> instances = new();
  public static void Start()
  {
    if (!Settings.ShowExperience) return;
    if (startTime.HasValue) return;
    Reset();
    startTime = DateTime.Now;
  }
  public static bool Running => startTime.HasValue;
  public static void Reset()
  {
    startTime = null;
    endTime = null;
    experiences.Clear();
    instances.Clear();
  }
  public static void AddExperience(Skills.SkillType skill, float value = 1f)
  {
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
  public static float GetExperienceModifier()
  {
    var seMan = Player.m_localPlayer.m_seman;
    var mod = 1f;
    seMan.ModifyRaiseSkill(Skills.SkillType.All, ref mod);
    return mod;
  }
  private static Skills.Skill GetSkill(Skills.SkillType type) => Player.m_localPlayer.m_skills.GetSkill(type);
  public static float GetLevel(Skills.SkillType type) => GetSkill(type).m_level;
  public static float GetCurrent(Skills.SkillType type) => GetSkill(type).m_accumulator;
  public static float GetTotal(Skills.SkillType type) => GetSkill(type).GetNextLevelRequirement();
  public static List<string>? Get()
  {
    if (!Settings.ShowExperience) return null;
    var time = 1.0;
    if (startTime.HasValue && endTime.HasValue)
      time = endTime.Value.Subtract(startTime.Value).TotalMilliseconds;
    time /= 60000.0;
    if (time == 0) time = 1;
    var lines = experiences.Select(kvp =>
    {
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
