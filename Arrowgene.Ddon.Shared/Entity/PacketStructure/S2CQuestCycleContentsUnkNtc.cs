using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestCycleContentsUnkNtc : IPacketStructure
    {
        // Potentially CPacket_S2C_FORT_DEFENSE_NOTICE?
        public PacketId Id => PacketId.S2C_QUEST_11_95_16_NTC;

        public List<CDataCycleContentsNoticeData> CycleContentsNoitceDataList { get; set; } = new();
        public List<CDataQuestContentsSituationInfoDetail> QuestContentsSituationInfoDetailList { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestCycleContentsUnkNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestCycleContentsUnkNtc obj)
            {
                WriteEntityList(buffer, obj.CycleContentsNoitceDataList);
                WriteEntityList(buffer, obj.QuestContentsSituationInfoDetailList);
            }

            public override S2CQuestCycleContentsUnkNtc Read(IBuffer buffer)
            {
                S2CQuestCycleContentsUnkNtc obj = new S2CQuestCycleContentsUnkNtc();
                obj.CycleContentsNoitceDataList = ReadEntityList<CDataCycleContentsNoticeData>(buffer);
                obj.QuestContentsSituationInfoDetailList = ReadEntityList<CDataQuestContentsSituationInfoDetail>(buffer);
                return obj;
            }
        }
    }
}
