using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SQuestGetCycleContentsPointListReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_QUEST_GET_CYCLE_CONTENTS_POINT_LIST_REQ;

        public uint CycleContentsScheduleId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SQuestGetCycleContentsPointListReq>
        {
            public override void Write(IBuffer buffer, C2SQuestGetCycleContentsPointListReq obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
            }

            public override C2SQuestGetCycleContentsPointListReq Read(IBuffer buffer)
            {
                C2SQuestGetCycleContentsPointListReq obj = new C2SQuestGetCycleContentsPointListReq();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
