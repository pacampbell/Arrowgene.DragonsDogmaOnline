using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestGetCycleContentsPointListRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_QUEST_GET_CYCLE_CONTENTS_POINT_LIST_RES;

        public uint CycleContentsScheduleId { get; set; }
        public CDataQuestPointDetail QuestPointDetail { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestGetCycleContentsPointListRes>
        {
            public override void Write(IBuffer buffer, S2CQuestGetCycleContentsPointListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteEntity(buffer, obj.QuestPointDetail);
            }

            public override S2CQuestGetCycleContentsPointListRes Read(IBuffer buffer)
            {
                S2CQuestGetCycleContentsPointListRes obj = new S2CQuestGetCycleContentsPointListRes();
                ReadServerResponse(buffer, obj);
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.QuestPointDetail = ReadEntity<CDataQuestPointDetail>(buffer);
                return obj;
            }
        }
    }
}
