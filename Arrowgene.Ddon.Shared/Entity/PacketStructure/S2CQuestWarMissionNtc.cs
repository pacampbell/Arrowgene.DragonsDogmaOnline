using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestWarMissionNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_11_97_16_NTC;

        public List<CDataCycleContentsNoticeDataEx> WarMissionNoticeDataList { get; set; } = new();
        public List<CDataQuestContentsSituationInfoDetail> QuestContentsSituationInfoDetailList { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestWarMissionNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestWarMissionNtc obj)
            {
                WriteEntityList(buffer, obj.WarMissionNoticeDataList);
                WriteEntityList(buffer, obj.QuestContentsSituationInfoDetailList);
            }

            public override S2CQuestWarMissionNtc Read(IBuffer buffer)
            {
                S2CQuestWarMissionNtc obj = new S2CQuestWarMissionNtc();
                obj.WarMissionNoticeDataList = ReadEntityList<CDataCycleContentsNoticeDataEx>(buffer);
                obj.QuestContentsSituationInfoDetailList = ReadEntityList<CDataQuestContentsSituationInfoDetail>(buffer);
                return obj;
            }
        }
    }
}
