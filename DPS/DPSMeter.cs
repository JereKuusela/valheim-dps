using System;
using System.Collections.Generic;
using Service;

namespace DPS {
  public class DPSMeter {
    private static DateTime? startTime = null;
    private static DateTime? endTime = null;
    private static float damageTaken = 0;
    private static float damage = 0;
    private static float structureDamage = 0;
    private static float baseDamage = 0;
    private static float baseStructureDamage = 0;
    // Pending damages are used to update DPS at end of the attack to make it more stable.
    private static float pendingDamage = 0;
    private static float pendingStructureDamage = 0;
    private static float pendingBaseDamage = 0;
    private static float pendingBaseStructureDamage = 0;
    private static HitData.DamageTypes? pendingBaseDamageTypes = null;
    private static float stagger = 0;
    private static float totalStamina = 0;
    private static float attackStamina = 0;
    private static float hits = 0;
    public static void Start() {
      if (!Settings.ShowDPS) return;
      if (startTime.HasValue) return;
      Reset();
      startTime = DateTime.Now;
    }
    public static bool Running => startTime.HasValue;
    public static void Reset() {
      startTime = null;
      endTime = null;
      damageTaken = 0;
      damage = 0;
      structureDamage = 0;
      baseDamage = 0;
      baseStructureDamage = 0;
      pendingDamage = 0;
      pendingStructureDamage = 0;
      pendingBaseDamage = 0;
      pendingBaseStructureDamage = 0;
      pendingBaseDamageTypes = null;
      stagger = 0;
      attackStamina = 0;
      totalStamina = 0;
      hits = 0;
    }
    public static void AddBaseDamage(HitData.DamageTypes hit) {
      if (!startTime.HasValue) return;
      if (hit.m_damage > 1E+9f) return;
      // Base damage is only available at start of the attack so it must be stored when the actual hits are resolved.
      pendingBaseDamageTypes = hit;
    }
    private static void AddStructureDamage(HitData hit, int tooltier, HitData.DamageModifiers modifiers) {
      if (!startTime.HasValue) return;
      if (hit.m_damage.m_damage > 1E+9f) return;
      if (hit.m_toolTier < tooltier) return;
      pendingStructureDamage += hit.GetTotalDamage();
      AddPendingBaseStructureDamage(modifiers);
    }
    public static void AddStructureDamage(HitData hit, WearNTear obj) => AddStructureDamage(hit, 0, obj.m_damages);
    public static void AddStructureDamage(HitData hit, TreeLog obj) => AddStructureDamage(hit, obj.m_minToolTier, obj.m_damages);
    public static void AddStructureDamage(HitData hit, TreeBase obj) => AddStructureDamage(hit, obj.m_minToolTier, obj.m_damageModifiers);
    public static void AddStructureDamage(HitData hit, MineRock5 obj) => AddStructureDamage(hit, obj.m_minToolTier, obj.m_damageModifiers);
    public static void AddStructureDamage(HitData hit, MineRock obj) => AddStructureDamage(hit, obj.m_minToolTier, obj.m_damageModifiers);
    public static void AddStructureDamage(HitData hit, Destructible obj) => AddStructureDamage(hit, obj.m_minToolTier, obj.m_damages);
    public static void AddDamageTaken(HitData hit) {
      if (!startTime.HasValue) return;
      damageTaken += hit.GetTotalDamage();
      SetTime();
    }
    private static void AddPendingBaseDamage(Character target) {
      if (!startTime.HasValue) return;
      var hit = new HitData()
      {
        m_damage = pendingBaseDamageTypes.Value
      };

      var damageModifiers = Call.Character_GetDamageModifiers(target);
      hit.ApplyResistance(damageModifiers, out var mod);
      if (target.IsPlayer()) {
        float bodyArmor = target.GetBodyArmor();
        hit.ApplyArmor(bodyArmor);
      }
      pendingBaseDamage += hit.GetTotalDamage();
    }
    private static void AddPendingBaseStructureDamage(HitData.DamageModifiers modifiers) {
      if (!startTime.HasValue) return;
      var hit = new HitData()
      {
        m_damage = pendingBaseDamageTypes.Value.Clone()
      };
      hit.ApplyResistance(modifiers, out var mod);
      pendingBaseStructureDamage += hit.GetTotalDamage();
    }
    public static void AddDamage(HitData hit, Character target) {
      if (!startTime.HasValue) return;
      if (hit.m_damage.m_damage > 1E+9f) return;
      AddPendingBaseDamage(target);
      pendingDamage += hit.GetTotalDamage();
      stagger += hit.m_damage.GetTotalStaggerDamage() * hit.m_staggerMultiplier;
      hits++;
    }
    public static void AddDot(HitData hit) {
      if (!startTime.HasValue) return;
      if (hit.m_damage.m_damage > 1E+9f) return;
      pendingDamage += hit.GetTotalDamage();
    }
    public static void AddAttackStamina(float stamina) {
      if (!startTime.HasValue) return;
      DPSMeter.attackStamina += stamina;
    }
    public static void AddTotalStamina(float stamina) {
      if (!startTime.HasValue) return;
      DPSMeter.totalStamina += stamina;
    }
    public static void SetTime() {
      if (!startTime.HasValue) return;
      endTime = DateTime.Now;
      damage += pendingDamage;
      pendingDamage = 0;
      baseDamage += pendingBaseDamage;
      pendingBaseDamage = 0;
      structureDamage += pendingStructureDamage;
      pendingStructureDamage = 0;
      baseStructureDamage += pendingBaseStructureDamage;
      pendingBaseStructureDamage = 0;
    }
    public static List<string> Get() {
      if (!Settings.ShowDPS) return null;
      var time = 1.0;
      if (startTime.HasValue && endTime.HasValue)
        time = endTime.Value.Subtract(startTime.Value).TotalMilliseconds;
      var damagePerSecond = damage * 1000.0 / time;
      var baseDamagePerSecond = baseDamage * 1000.0 / time;
      var staminaPerSecond = attackStamina * 1000.0 / time;
      var damagePerStamina = attackStamina > 0 ? (damage + pendingDamage) / attackStamina : 0;
      var baseDamagePerStamina = attackStamina > 0 ? (baseDamage + pendingBaseDamage) / attackStamina : 0;
      var staggerPerSecond = stagger * 1000.0 / time;
      var hitsPerSecond = hits * 1000.0 / time;
      var attackSpeed = hits > 0 ? time / hits / 1000.0 : 0;
      var damageTakenPerSecond = damageTaken * 1000.0 / time;
      var lines = new List<string>();
      lines.Add("Time: " + Format.Float(time / 1000.0) + " seconds with " + Format.Float(hits) + " hits");
      lines.Add("DPS: " + Format.Float(damagePerSecond) + " (total " + Format.Float(damage + pendingDamage) + ")"
        + ", per stamina: " + Format.Float(damagePerStamina));
      lines.Add("Normalized: " + Format.Float(baseDamagePerSecond) + " (total " + Format.Float(baseDamage + pendingBaseDamage) + ")"
        + ", per stamina: " + Format.Float(baseDamagePerStamina));
      lines.Add("Stamina: " + Format.Float(staminaPerSecond) + " (total " + Format.Float(attackStamina) + ")");
      if (totalStamina != attackStamina)
        lines.Add("Total stamina: " + Format.Float(totalStamina * 1000.0 / time) + " (total " + Format.Float(totalStamina) + ")");
      lines.Add("Staggering: " + Format.Float(staggerPerSecond) + " (total " + Format.Float(stagger) + ")");
      lines.Add("Attack speed: " + Format.Float(hitsPerSecond) + " (" + Format.Float(attackSpeed) + " s per attack)");
      if (damageTaken > 0)
        lines.Add("Damage taken: " + Format.Float(damageTakenPerSecond) + " (total " + Format.Float(damageTaken) + ")");
      if (structureDamage > 0) {
        var structureDamagePerSecond = structureDamage * 1000.0 / time;
        var structureDamagePerStamina = (structureDamage + pendingStructureDamage) / totalStamina;
        var baseStructureDamagePerSecond = baseStructureDamage * 1000.0 / time;
        var baseStructureDamagePerStamina = (baseStructureDamage + pendingBaseStructureDamage) / totalStamina;
        lines.Add("Structure DPS: " + Format.Float(structureDamagePerSecond) + " (total " + Format.Float(structureDamage + pendingStructureDamage) + ")"
          + ", per stamina: " + Format.Float(structureDamagePerStamina));
        lines.Add("Normalized: " + Format.Float(baseStructureDamagePerSecond) + " (total " + Format.Float(baseStructureDamage + pendingBaseStructureDamage) + ")"
          + ", per stamina: " + Format.Float(baseStructureDamagePerStamina));

      }
      return lines;
    }
  }
}