using HarmonyLib;
namespace DPS;
#region Tracking attacks for DPS meter.
[HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
public class Attack_Start {
  static void Postfix(Attack __instance, Humanoid character, bool __result, int ___m_currentAttackCainLevel) {
    if (__result && character == Player.m_localPlayer) {
      DPSMeter.Start();
      var stamina = __instance.GetAttackStamina();
      DPSMeter.AddStamina(stamina);
      DPSMeter.AddHit();
    }
  }
}
[HarmonyPatch(typeof(Attack), nameof(Attack.OnAttackDone))]
public class Attack_OnAttackDone {
  static void Postfix(Humanoid ___m_character) {
    if (___m_character == Player.m_localPlayer)
      DPSMeter.SetTime();
  }
}
#endregion
#region Tracking structure damage for DPS meter.
[HarmonyPatch(typeof(Destructible), nameof(Destructible.RPC_Damage))]
public class Destructible_RPC_Damage {
  static void Postfix(Destructible __instance, HitData hit) {
    if (hit.GetAttacker() == Player.m_localPlayer)
      DPSMeter.AddStructureDamage(hit, __instance);
  }
}
[HarmonyPatch(typeof(MineRock), nameof(MineRock.RPC_Hit))]
public class MineRock_RPC_Hit {
  static void Postfix(MineRock __instance, HitData hit) {
    if (hit.GetAttacker() == Player.m_localPlayer)
      DPSMeter.AddStructureDamage(hit, __instance);
  }
}
[HarmonyPatch(typeof(WearNTear), nameof(WearNTear.RPC_Damage))]
public class WearNTear_RPC_Damage {
  static void Postfix(WearNTear __instance, HitData hit) {
    if (hit.GetAttacker() == Player.m_localPlayer)
      DPSMeter.AddStructureDamage(hit, __instance);
  }
}
[HarmonyPatch(typeof(MineRock5), nameof(MineRock5.DamageArea))]
public class MineRock5_DamageArea {
  static void Postfix(MineRock5 __instance, HitData hit) {
    if (hit.GetAttacker() == Player.m_localPlayer)
      DPSMeter.AddStructureDamage(hit, __instance);
  }
}
[HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))]
public class TreeBase_RPC_Damage {
  static void Postfix(TreeBase __instance, HitData hit) {
    if (hit.GetAttacker() == Player.m_localPlayer)
      DPSMeter.AddStructureDamage(hit, __instance);
  }
}
[HarmonyPatch(typeof(TreeLog), nameof(TreeLog.RPC_Damage))]
public class TreeLog_RPC_Damage {
  static void Postfix(TreeLog __instance, HitData hit) {
    if (hit.GetAttacker() == Player.m_localPlayer)
      DPSMeter.AddStructureDamage(hit, __instance);
  }
}
#endregion
#region Tracking creature damage for DPS meter
[HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
public class Character_ApplyDamage {
  static void Prefix(Character __instance, HitData hit) {
    if (hit.GetAttacker() == Player.m_localPlayer)
      DPSMeter.AddDamage(hit, __instance);
    if (hit.GetAttacker() == null)
      DPSMeter.AddDot(hit);
    if (__instance == Player.m_localPlayer)
      DPSMeter.AddDamageTaken(hit);
  }
}
#endregion
#region Tracking other things for DPS meter.
[HarmonyPatch(typeof(Player), nameof(Player.UseStamina))]
public class Player_UseStamina {
  static void Prefix(Player __instance, float v) {
    if (__instance == Player.m_localPlayer)
      DPSMeter.AddTotalStamina(v);
  }
}
#endregion
