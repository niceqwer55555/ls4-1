using LeagueSandbox.GameServer.Players;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace LeagueSandbox.GameServer.Chatbox.Commands
{
    /// <summary>
    /// /dumpspells - 打印当前英雄所有技能状态
    /// 显示每个技能槽位对应的脚本是否已加载
    /// </summary>
    public class DumpSpellsCommand : ChatCommandBase
    {
        private readonly PlayerManager _playerManager;

        public override string Command => "dumpspells";
        public override string Syntax => $"{Command}";

        public DumpSpellsCommand(ChatCommandManager chatCommandManager, Game game)
            : base(chatCommandManager, game)
        {
            _playerManager = game.PlayerManager;
        }

        public override void Execute(int userId, bool hasReceivedArguments, string arguments = "")
        {
            var champ = _playerManager.GetPeerInfo(userId).Champion;

            ChatCommandManager.SendDebugMsgFormatted(DebugMsgType.INFO,
                $"=== {champ.Model} Spell Status ===");

            foreach (var kv in champ.Spells)
            {
                var slot = kv.Key;
                var spell = kv.Value;
                var scriptOk = !(spell.Script is SpellScriptEmpty);
                var status = scriptOk ? "OK" : "MISSING";
                ChatCommandManager.SendDebugMsgFormatted(scriptOk ? DebugMsgType.INFO : DebugMsgType.ERROR,
                    $"  Slot={slot} Name={spell.SpellName} Script={status}");
            }

            ChatCommandManager.SendDebugMsgFormatted(DebugMsgType.INFO,
                $"=== Buffs: {champ.GetBuffs().Count} ===");
            foreach (var buff in champ.GetBuffs())
            {
                var scriptOk = !(buff.BuffScript is BuffScriptEmpty);
                var status = scriptOk ? "OK" : "MISSING";
                ChatCommandManager.SendDebugMsgFormatted(scriptOk ? DebugMsgType.INFO : DebugMsgType.ERROR,
                    $"  {buff.Name} Script={status}");
            }
        }
    }
}
