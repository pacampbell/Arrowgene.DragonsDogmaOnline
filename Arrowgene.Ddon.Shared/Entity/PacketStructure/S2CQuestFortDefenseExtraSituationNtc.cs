using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestFortDefenseExtraSituationNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_11_100_16_NTC;

        public uint SituationValue { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuestFortDefenseExtraSituationNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestFortDefenseExtraSituationNtc obj)
            {
                WriteUInt32(buffer, obj.SituationValue);
            }

            public override S2CQuestFortDefenseExtraSituationNtc Read(IBuffer buffer)
            {
                S2CQuestFortDefenseExtraSituationNtc obj = new S2CQuestFortDefenseExtraSituationNtc();
                obj.SituationValue = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
