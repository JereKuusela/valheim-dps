using System;
using System.Collections.Generic;
using Service;
namespace DPS;
class Stats
{
  public float Hits = 0;
  public float Damage = 0;
  public float StructureDamage = 0;
  public float Staggering = 0;
  public float TotalStamina = 0;
  public float Stamina = 0;
  public float DamagePerStamina => Stamina > 0 ? Damage / Stamina : 0;
  public float StructureDamagePerStamina => Stamina > 0 ? StructureDamage / Stamina : 0;
  public static Stats operator +(Stats a, Stats b)
      => new()
      {
        Hits = a.Hits + b.Hits,
        Damage = a.Damage + b.Damage,
        StructureDamage = a.StructureDamage + b.StructureDamage,
        Staggering = a.Staggering + b.Staggering,
        TotalStamina = a.TotalStamina + b.TotalStamina,
        Stamina = a.Stamina + b.Stamina
      };
  public static Stats operator *(Stats a, float b)
      => new()
      {
        Hits = a.Hits * b,
        Damage = a.Damage * b,
        StructureDamage = a.StructureDamage * b,
        Staggering = a.Staggering * b,
        TotalStamina = a.TotalStamina * b,
        Stamina = a.Stamina * b
      };
}
public class DPSMeter
{
  private static DateTime? startTime = null;
  private static DateTime? endTime = null;
  private static float damageTaken = 0;
  private static Stats stats = new();

  // Pending damages are used to update DPS at end of the attack to make it more stable.
  private static Stats pending = new();
  public static void Start()
  {
    if (!Settings.ShowDPS) return;
    if (startTime.HasValue) return;
    Reset();
    startTime = DateTime.Now;
  }
  public static bool Running => startTime.HasValue;
  public static void Reset()
  {
    startTime = null;
    endTime = null;
    damageTaken = 0;
    stats = new();
    pending = new();
  }
  private static void AddStructureDamage(HitData hit, int tooltier)
  {
    if (!startTime.HasValue) return;
    if (hit.m_damage.m_damage > 1E+9f) return;
    if (hit.m_toolTier < tooltier) return;
    pending.StructureDamage += hit.GetTotalDamage();
  }
  public static void AddStructureDamage(HitData hit) => AddStructureDamage(hit, 0);
  public static void AddStructureDamage(HitData hit, TreeLog obj) => AddStructureDamage(hit, obj.m_minToolTier);
  public static void AddStructureDamage(HitData hit, TreeBase obj) => AddStructureDamage(hit, obj.m_minToolTier);
  public static void AddStructureDamage(HitData hit, MineRock5 obj) => AddStructureDamage(hit, obj.m_minToolTier);
  public static void AddStructureDamage(HitData hit, MineRock obj) => AddStructureDamage(hit, obj.m_minToolTier);
  public static void AddStructureDamage(HitData hit, Destructible obj) => AddStructureDamage(hit, obj.m_minToolTier);
  public static void AddDamageTaken(HitData hit)
  {
    if (!startTime.HasValue) return;
    damageTaken += hit.GetTotalDamage();
    SetTime();
  }
  public static void AddDamage(HitData hit)
  {
    if (!startTime.HasValue) return;
    if (hit.m_damage.m_damage > 1E+9f) return;
    pending.Damage += hit.GetTotalDamage();
    pending.Staggering += hit.m_damage.GetTotalStaggerDamage() * hit.m_staggerMultiplier;
  }
  public static void AddDot(HitData hit)
  {
    if (!startTime.HasValue) return;
    if (hit.m_damage.m_damage > 1E+9f) return;
    pending.Damage += hit.GetTotalDamage();
  }
  public static void AddStamina(float stamina)
  {
    if (!startTime.HasValue) return;
    pending.Stamina += stamina;
  }
  public static void AddHit()
  {
    if (!startTime.HasValue) return;
    pending.Hits++;
  }
  public static void AddTotalStamina(float stamina)
  {
    if (!startTime.HasValue) return;
    pending.TotalStamina += stamina;
  }
  public static void SetTime()
  {
    if (!startTime.HasValue) return;
    endTime = DateTime.Now;
    stats += pending;
    pending = new();
  }
  public static List<string>? Get()
  {
    if (!Settings.ShowDPS) return null;
    var time = 0.00001;
    if (startTime.HasValue && endTime.HasValue)
      time = endTime.Value.Subtract(startTime.Value).TotalSeconds;
    var perSecond = 1f / (float)time;
    var ps = stats * perSecond;
    var total = stats + pending;
    var attackSpeed = ps.Hits > 0 ? 1 / ps.Hits : 0;
    List<string> lines = new()
    {
      "Time: " + Format.Float(time) + " seconds with " + Format.Float(stats.Hits) + " hits",
      "DPS: " + Format.Float(ps.Damage) + " (total " + Format.Float(total.Damage) + ")"
      + ", per stamina: " + Format.Float(stats.DamagePerStamina),
      "Stamina: " + Format.Float(ps.Stamina) + " (total " + Format.Float(total.Stamina) + ")"
    };
    if (stats.TotalStamina != stats.Stamina)
      lines.Add("Total stamina: " + Format.Float(ps.TotalStamina) + " (total " + Format.Float(total.TotalStamina) + ")");
    lines.Add("Staggering: " + Format.Float(ps.Staggering) + " (total " + Format.Float(total.Staggering) + ")");
    lines.Add("Attack speed: " + Format.Float(ps.Hits) + " (" + Format.Float(attackSpeed) + " s per attack)");
    if (damageTaken > 0)
      lines.Add("Damage taken: " + Format.Float(damageTaken * perSecond) + " (total " + Format.Float(damageTaken) + ")");
    if (stats.StructureDamage > 0)
    {
      lines.Add("Structure DPS: " + Format.Float(ps.StructureDamage) + " (total " + Format.Float(total.StructureDamage) + ")"
        + ", per stamina: " + Format.Float(stats.StructureDamagePerStamina));
    }
    return lines;
  }
}
