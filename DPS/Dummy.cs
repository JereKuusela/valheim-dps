using System;
using System.Linq;
using UnityEngine;
namespace DPS;
public class Dummy
{
  private static Character Create()
  {
    var prefab = ZNetScene.instance.GetPrefab("TrainingDummy");
    return UnityEngine.Object.Instantiate<GameObject>(prefab, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity).GetComponent<Character>();
  }
  private static HitData.DamageModifier GetModifier(string value)
  {
    value = value.Trim().ToLower();
    return value switch
    {
      "ignore" => HitData.DamageModifier.Ignore,
      "weak" => HitData.DamageModifier.Weak,
      "veryweak" => HitData.DamageModifier.VeryWeak,
      "resistant" => HitData.DamageModifier.Resistant,
      "veryresistant" => HitData.DamageModifier.VeryResistant,
      "normal" => HitData.DamageModifier.Normal,
      "immune" => HitData.DamageModifier.Immune,
      "-" => HitData.DamageModifier.Ignore,
      "1.5" => HitData.DamageModifier.Weak,
      "2.0" => HitData.DamageModifier.VeryWeak,
      "2" => HitData.DamageModifier.VeryWeak,
      "0.5" => HitData.DamageModifier.Resistant,
      "0.25" => HitData.DamageModifier.VeryResistant,
      "1.0" => HitData.DamageModifier.Normal,
      "1" => HitData.DamageModifier.Normal,
      "0.0" => HitData.DamageModifier.Immune,
      "0" => HitData.DamageModifier.Immune,
      _ => HitData.DamageModifier.Normal,
    };
  }
  public static void Spawn(string[] args)
  {
    var character = Create();
    var baseAI = character.GetComponent<BaseAI>();
    if (baseAI) baseAI.SetHuntPlayer(true);
    var seMan = character.GetSEMan();
    character.m_damageModifiers.m_blunt = HitData.DamageModifier.Normal;
    character.m_damageModifiers.m_chop = HitData.DamageModifier.Ignore;
    character.m_damageModifiers.m_fire = HitData.DamageModifier.Normal;
    character.m_damageModifiers.m_frost = HitData.DamageModifier.Normal;
    character.m_damageModifiers.m_lightning = HitData.DamageModifier.Normal;
    character.m_damageModifiers.m_pickaxe = HitData.DamageModifier.Ignore;
    character.m_damageModifiers.m_pierce = HitData.DamageModifier.Normal;
    character.m_damageModifiers.m_poison = HitData.DamageModifier.Normal;
    character.m_damageModifiers.m_slash = HitData.DamageModifier.Normal;
    character.m_damageModifiers.m_spirit = HitData.DamageModifier.Normal;
    foreach (var arg in args.OrderBy(item => item))
    {
      var split = arg.Split('=');
      var type = split[0].ToLower();
      var modifier = split.Length > 1 ? GetModifier(split[1]) : HitData.DamageModifier.Normal;
      var hasDuration = int.TryParse(split.Length > 1 ? split[1] : "", out var duration);
      UpdateModifier(type, character, modifier);
      var seName = GetStatusEffect(type);
      if (seName != "")
      {
        var se = seMan.AddStatusEffect(seName.GetStableHashCode(), true);
        if (hasDuration)
          se.m_ttl = duration;
      };
    }
  }
  private static string GetStatusEffect(string value)
  {
    value = value.Trim().ToLower();
    return value switch
    {
      "cold" => "Cold",
      "corpserun" => "CorpseRun",
      "freezing" => "Freezing",
      "shield" => "GoblinShaman_shield",
      "bonemass" => "GP_Bonemass",
      "eikthyr" => "GP_Eikthyr",
      "moder" => "GP_Moder",
      "elder" => "GP_TheElder",
      "yagluth" => "GP_Yagluth",
      "barleywine" => "Potion_barleywine",
      "frostresist" => "Potion_frostresist",
      "poisonresist" => "Potion_poisonresist",
      "tared" => "Tared",
      "wet" => "Wet",
      _ => "",
    };
  }
  private static void UpdateModifier(string type, Character character, HitData.DamageModifier modifier)
  {
    if (type == "*" || type == "blunt")
      character.m_damageModifiers.m_blunt = modifier;
    if (type == "*" || type == "chop")
      character.m_damageModifiers.m_chop = modifier;
    if (type == "*" || type == "fire")
      character.m_damageModifiers.m_fire = modifier;
    if (type == "*" || type == "frost")
      character.m_damageModifiers.m_frost = modifier;
    if (type == "*" || type == "lightning")
      character.m_damageModifiers.m_lightning = modifier;
    if (type == "*" || type == "pickaxe")
      character.m_damageModifiers.m_pickaxe = modifier;
    if (type == "*" || type == "pierce")
      character.m_damageModifiers.m_pierce = modifier;
    if (type == "*" || type == "poison")
      character.m_damageModifiers.m_poison = modifier;
    if (type == "*" || type == "slash")
      character.m_damageModifiers.m_slash = modifier;
    if (type == "*" || type == "spirit")
      character.m_damageModifiers.m_spirit = modifier;
  }
  public static int Kill()
  {
    var dummies = Character.GetAllCharacters().Where(item => item.m_name.ToLower() == "trainingdummy");
    foreach (var dummy in dummies)
    {
      HitData hitData = new();
      hitData.m_damage.m_damage = 1E+10f;
      dummy.Damage(hitData);
    }
    return dummies.Count();
  }
}
