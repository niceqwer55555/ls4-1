using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace ItemPassives
{
    public class ItemID_1036 : IItemScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
    }
}