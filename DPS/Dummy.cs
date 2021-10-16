using System.Linq;
using UnityEngine;

namespace DPS {
  public class Dummy {
    private static Character Create() {
      var prefab = ZNetScene.instance.GetPrefab("TrainingDummy");
      return UnityEngine.Object.Instantiate<GameObject>(prefab, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity).GetComponent<Character>();
    }
    private static HitData.DamageModifier GetModifier(string value) {
      value = value.Trim().ToLower();
      switch (value) {
        case "ignore": return HitData.DamageModifier.Ignore;
        case "weak": return HitData.DamageModifier.Weak;
        case "veryweak": return HitData.DamageModifier.VeryWeak;
        case "resistant": return HitData.DamageModifier.Resistant;
        case "veryresistant": return HitData.DamageModifier.VeryResistant;
        case "normal": return HitData.DamageModifier.Normal;
        case "immune": return HitData.DamageModifier.Immune;
        case "-": return HitData.DamageModifier.Ignore;
        case "1.5": return HitData.DamageModifier.Weak;
        case "2.0": return HitData.DamageModifier.VeryWeak;
        case "2": return HitData.DamageModifier.VeryWeak;
        case "0.5": return HitData.DamageModifier.Resistant;
        case "0.25": return HitData.DamageModifier.VeryResistant;
        case "1.0": return HitData.DamageModifier.Normal;
        case "1": return HitData.DamageModifier.Normal;
        case "0.0": return HitData.DamageModifier.Immune;
        case "0": return HitData.DamageModifier.Immune;
        default: return HitData.DamageModifier.Normal;
      }
    }
    public static void Spawn(string[] args) {
      var character = Create();
      var baseAI = character.GetComponent<BaseAI>();
      if (baseAI) baseAI.SetHuntPlayer(true);
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
      foreach (var arg in args) {
        var split = arg.Split('=');
        if (split.Length < 2) continue;
        var type = split[0].ToLower();
        var modifier = GetModifier(split[1]);
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
    }

    public static int Kill() {
      var dummies = Character.GetAllCharacters().Where(item => item.m_name.ToLower() == "trainingdummy");
      foreach (var dummy in dummies) {
        HitData hitData = new HitData();
        hitData.m_damage.m_damage = 1E+10f;
        dummy.Damage(hitData);
      }
      return dummies.Count();
    }
  }
}