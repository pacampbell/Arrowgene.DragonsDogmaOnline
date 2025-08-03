using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SQuestGetCycleContentsSituationInfoListReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_QUEST_GET_CYCLE_CONTENTS_SITUATION_INFO_LIST_REQ;

        public uint CycleContentsScheduleId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SQuestGetCycleContentsSituationInfoListReq>
        {
            public override void Write(IBuffer buffer, C2SQuestGetCycleContentsSituationInfoListReq obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
            }

            public override C2SQuestGetCycleContentsSituationInfoListReq Read(IBuffer buffer)
            {
                C2SQuestGetCycleContentsSituationInfoListReq obj = new C2SQuestGetCycleContentsSituationInfoListReq();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
