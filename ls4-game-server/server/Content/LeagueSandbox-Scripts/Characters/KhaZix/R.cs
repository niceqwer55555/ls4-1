using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;

namespace Spells
{
    public class KhazixR : ISpellScript
    {
        ObjAIBase Khazix;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Khazix = spell.CastInfo.Owner as Champion;
            AddParticleTarget(Khazix, Khazix, "khazix_base_r_cas", Khazix);
        }
        public void OnSpellPostCast(Spell spell)
        {
            if (!Khazix.HasBuff("KhazixR"))
            {
                Khazix.Spells[3].SetCooldown(0.5f, true);
                AddBuff("KhazixR", 15.0f, 1, spell, Khazix, Khazix);

                var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
                var trueCoords = GetPointFromUnit(Khazix, 700f);
                TeleportTo(Khazix, trueCoords.X, trueCoords.Y);

                float[] baseDamage = { 100f, 150f, 200f };
                float ad = Khazix.Stats.AttackDamage.Total * 0.6f;
                float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad;

                foreach (var unit in GetUnitsInRange(Khazix.Position, 300f, true))
                {
                    if (unit.Team != Khazix.Team && unit != Khazix)
                    {
                        unit.TakeDamage(Khazix, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    }
                }
            }
            else
            {
                Khazix.RemoveBuffsWithName("KhazixR");
            }
            AddBuff("KhazixRStealth", 1.0f, 1, spell, Khazix, Khazix);
        }
    }
}