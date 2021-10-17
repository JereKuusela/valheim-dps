using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using Service;

namespace DPS {
  [BepInPlugin("valheim.jerekuusela.dps", "DPS", "1.0.0.0")]
  public class DPS : BaseUnityPlugin {
    public void Awake() {
      Settings.Init(Config);
      var harmony = new Harmony("valheim.jerekuusela.dps");
      harmony.PatchAll();
      InitCommands();
      MessageHud_UpdateMessage.GetMessage = GetMessage;
    }
    private static List<string> GetMessage() {
      var lines = new List<string>();
      var dps = DPSMeter.Get();
      var eps = ExperienceMeter.Get();
      if (dps != null || eps != null) {
        if (Admin.Checking)
          lines.Add(Format.String("Authenticating..."));
        else if (Settings.AdminRequired())
          lines.Add(Format.String("Authentication failed, some features won't work.", "red"));
        else {
          if (Settings.CreatureDamageRange >= 0)
            lines.Add("Creature damage range: " + Format.Percent(Settings.CreatureDamageRange));
          if (Settings.MaxAttackChainLevels >= 0)
            lines.Add("Max attack chain: " + Format.Int(Settings.MaxAttackChainLevels));
          if (Settings.PlayerDamageRange >= 0)
            lines.Add("Player damage range: " + Format.Percent(Settings.PlayerDamageRange));
          if (Settings.NoStaminaUsage)
            lines.Add("No stamina usage");
          if (Settings.SetSkills >= 0)
            lines.Add("Override skills: " + Format.Int(Settings.SetSkills * 100));
        }
      }
      if (dps != null) {
        lines.Add(" ");
        lines.AddRange(dps);
      }
      if (eps != null) {
        lines.Add(" ");
        lines.AddRange(eps);
      }
      return lines;
    }


    private static void InitCommands() {
      new Terminal.ConsoleCommand("dps", "Toggles DPS tool.", delegate (Terminal.ConsoleEventArgs args) {
        if (Settings.ShowDPS && DPSMeter.Running) {
          DPSMeter.Reset();
          args.Context.AddString("DPS tool reseted.");
        } else {
          Settings.configShowDPS.Value = !Settings.ShowDPS;
          args.Context.AddString("DPS tool " + (Settings.ShowDPS ? "enabled" : "disabled") + ".");
        }
      });
      new Terminal.ConsoleCommand("exp", "Toggles experience tool.", delegate (Terminal.ConsoleEventArgs args) {
        if (Settings.ShowExperience && ExperienceMeter.Running) {
          ExperienceMeter.Reset();
          args.Context.AddString("Experience tool reseted.");
        } else {
          Settings.configShowExperience.Value = !Settings.ShowExperience;
          args.Context.AddString("Experience tool " + (Settings.ShowExperience ? "enabled" : "disabled") + ".");
        }
      });
      new Terminal.ConsoleCommand("dummy_spawn", "[resistance1=modifier1] [resistance2=modifier2]... - Spawns a training dummy.", delegate (Terminal.ConsoleEventArgs args) {
        if (!Admin.Enabled) args.Context.AddString("Unauthorized to spawn dummies.");
        Dummy.Spawn(args.Args);
        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawned a training dummy", 0, null);
      });
      new Terminal.ConsoleCommand("dummy_kill", "Kills all training dummies.", delegate (Terminal.ConsoleEventArgs args) {
        if (!Admin.Enabled) args.Context.AddString("Unauthorized to kill dummies.");
        var killed = Dummy.Kill();
        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Killing all training dummies:" + killed, 0, null);
      });
      new Terminal.ConsoleCommand("dummy_reset", "[resistance1=modifier1] [resistance2=modifier2]... - Kills all training dummies and spawns a new one.", delegate (Terminal.ConsoleEventArgs args) {
        if (!Admin.Enabled) args.Context.AddString("Unauthorized to spawn dummies.");
        Dummy.Kill();
        Dummy.Spawn(args.Args);
        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawned a training dummy", 0, null);
      });
    }
  }
}
