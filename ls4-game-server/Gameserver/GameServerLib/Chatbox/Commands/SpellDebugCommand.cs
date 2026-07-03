using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace LeagueSandbox.GameServer.Chatbox.Commands
{
    /// <summary>
    /// /spelldebug - 切换技能调试日志
    /// 开启后在控制台打印所有技能的加载和施放详情
    /// </summary>
    public class SpellDebugCommand : ChatCommandBase
    {
        public override string Command => "spelldebug";
        public override string Syntax => $"{Command}";

        public SpellDebugCommand(ChatCommandManager chatCommandManager, Game game)
            : base(chatCommandManager, game) { }

        public override void Execute(int userId, bool hasReceivedArguments, string arguments = "")
        {
            Spell.DebugSpells = !Spell.DebugSpells;
            var status = Spell.DebugSpells ? "ON" : "OFF";
            ChatCommandManager.SendDebugMsgFormatted(DebugMsgType.INFO,
                $"Spell debug logging: <b>{status}</b>. Check server console for output.");
        }
    }
}
