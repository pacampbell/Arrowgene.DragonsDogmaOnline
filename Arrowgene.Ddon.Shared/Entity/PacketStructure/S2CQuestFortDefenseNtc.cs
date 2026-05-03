using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestFortDefenseNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_FORT_DEFENSE_NTC;

        public List<CDataCycleContentsNoticeData> FortDefenseNoticeDataList { get; set; } = new();
        public List<CDataQuestContentsSituationInfoDetail> QuestContentsSituationInfoDetailList { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestFortDefenseNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestFortDefenseNtc obj)
            {
                WriteEntityList(buffer, obj.FortDefenseNoticeDataList);
                WriteEntityList(buffer, obj.QuestContentsSituationInfoDetailList);
            }

            public override S2CQuestFortDefenseNtc Read(IBuffer buffer)
            {
                S2CQuestFortDefenseNtc obj = new S2CQuestFortDefenseNtc();
                obj.FortDefenseNoticeDataList = ReadEntityList<CDataCycleContentsNoticeData>(buffer);
                obj.QuestContentsSituationInfoDetailList = ReadEntityList<CDataQuestContentsSituationInfoDetail>(buffer);
                return obj;
            }
        }
    }
}
