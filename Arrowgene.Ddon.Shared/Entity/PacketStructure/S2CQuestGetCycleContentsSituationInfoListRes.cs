using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestGetCycleContentsSituationInfoListRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_QUEST_GET_CYCLE_CONTENTS_SITUATION_INFO_LIST_RES;

        public uint CycleContentsScheduleId { get; set; }
        public List<CDataQuestContentsSituationInfo> QuestContentsSituationInfoList { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestGetCycleContentsSituationInfoListRes>
        {
            public override void Write(IBuffer buffer, S2CQuestGetCycleContentsSituationInfoListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteEntityList(buffer, obj.QuestContentsSituationInfoList);
            }

            public override S2CQuestGetCycleContentsSituationInfoListRes Read(IBuffer buffer)
            {
                S2CQuestGetCycleContentsSituationInfoListRes obj = new S2CQuestGetCycleContentsSituationInfoListRes();
                ReadServerResponse(buffer, obj);
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.QuestContentsSituationInfoList = ReadEntityList<CDataQuestContentsSituationInfo>(buffer);
                return obj;
            }
        }
    }
}
