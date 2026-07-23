using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System;

namespace Spells
{
    public class AhriTumble : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        Spell Spell;
        float ticks;
        AttackableUnit datarget;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
        }
        int proc = 0;
        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            if (!owner.HasBuff("AhriTumble"))
            {
                AddBuff("AhriTumble", 8f, 3, spell, owner, owner);
            }
            else
            {
                var buff = owner.GetBuffWithName("AhriTumble");
                if (buff != null && buff.StackCount > 0)
                {
                    buff.DeactivateBuff();
                    AddBuff("AhriTumble", 8f, (byte)(buff.StackCount - 1), spell, owner, owner);
                }
                else
                {
                    return;
                }
            }

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var trueCoords = GetPointFromUnit(owner, 450f);
            TeleportTo(owner, trueCoords.X, trueCoords.Y);

            SpellCast(owner, 1, SpellSlotType.ExtraSlots, true, null, targetPos);

            AddParticle(owner, owner, "Ahri_Base_R_Cas.troy", owner.Position);
            AddParticleTarget(owner, owner, "Ahri_Base_R_Flash.troy", owner);
        }

        public void SendMisTarget(AttackableUnit target)
        {
        }

        public void OnSpellChannel(Spell spell)
        {
        }

        public void OnSpellChannelCancel(Spell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(Spell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }

    public class AhriTumbleMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle,
                CanHitSameTargetConsecutively = true,
            },
            IsDamagingSpell = true,
            TriggersSpellCasts = true,
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
        }
        public void OnSpellPostCast(Spell spell)
        {
        }

        public void OnSpellChannel(Spell spell)
        {
        }

        public void OnSpellChannelCancel(Spell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(Spell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}