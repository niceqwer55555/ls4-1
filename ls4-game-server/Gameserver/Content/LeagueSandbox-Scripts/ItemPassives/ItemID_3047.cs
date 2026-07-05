using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace ItemPassives
{
    public class ItemID_3047 : IItemScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
    }
}