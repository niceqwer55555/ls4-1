using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace ItemPassives
{
    public class ItemID_1500 : IItemScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
    }
}