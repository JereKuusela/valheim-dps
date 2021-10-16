using HarmonyLib;
using UnityEngine;

namespace Service {
  [HarmonyPatch]
  public class GetValue {
    private static T Get<T>(object obj, string field) => Traverse.Create(obj).Field<T>(field).Value;
    public static bool Cheat(Terminal obj) => Get<bool>(obj, "m_cheat");
    public static ZNetView Nview(MonoBehaviour obj) => Get<ZNetView>(obj, "m_nview");
    public static SEMan Seman(Player obj) => Get<SEMan>(obj, "m_seman");
    public static Skills Skills(Player obj) => Get<Skills>(obj, "m_skills");
    public static int NextAttackChainLevel(Attack obj) => Get<int>(obj, "m_nextAttackChainLevel");
  }
}