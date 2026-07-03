using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LeaguePackets.Game
{
    /// <summary>
    /// S2C CastSpell - Simplified broadcast packet for spell casting (LeagueOfLegendsV2 style)
    /// 0x9B - matches modern client protocol
    /// </summary>
    public class S2C_CastSpell : GamePacket
    {
        public override GamePacketID ID => GamePacketID.S2C_CastSpell;
        public byte SpellSlot { get; set; }
        public float TargetPositionX { get; set; }
        public float TargetPositionY { get; set; }
        public float TargetPositionZ { get; set; }
        public uint TargetNetID { get; set; }

        protected override void ReadBody(ByteReader reader)
        {
            SpellSlot = reader.ReadByte();
            TargetPositionX = reader.ReadFloat();
            TargetPositionY = reader.ReadFloat();
            TargetPositionZ = reader.ReadFloat();
            TargetNetID = reader.ReadUInt32();
        }

        protected override void WriteBody(ByteWriter writer)
        {
            writer.WriteByte(SpellSlot);
            writer.WriteFloat(TargetPositionX);
            writer.WriteFloat(TargetPositionY);
            writer.WriteFloat(TargetPositionZ);
            writer.WriteUInt32(TargetNetID);
        }
    }
}
