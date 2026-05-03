using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestWarMissionGaugeZeroNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_11_106_16_NTC;

        // The gauge value at the moment it reached zero (defeat condition)
        public uint GaugeValue { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuestWarMissionGaugeZeroNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestWarMissionGaugeZeroNtc obj)
            {
                WriteUInt32(buffer, obj.GaugeValue);
            }

            public override S2CQuestWarMissionGaugeZeroNtc Read(IBuffer buffer)
            {
                S2CQuestWarMissionGaugeZeroNtc obj = new S2CQuestWarMissionGaugeZeroNtc();
                obj.GaugeValue = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
