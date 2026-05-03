using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestCycleContentsPointNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_11_99_16_NTC;

        public uint CycleContentsScheduleId { get; set; }
        public List<CDataCommonU32> QuestPointList { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestCycleContentsPointNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestCycleContentsPointNtc obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteEntityList(buffer, obj.QuestPointList);
            }

            public override S2CQuestCycleContentsPointNtc Read(IBuffer buffer)
            {
                S2CQuestCycleContentsPointNtc obj = new S2CQuestCycleContentsPointNtc();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.QuestPointList = ReadEntityList<CDataCommonU32>(buffer);
                return obj;
            }
        }
    }
}
