using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SQuestCycleContentsPlayStartReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_QUEST_CYCLE_CONTENTS_PLAY_START_REQ;

        public uint CycleContentsScheduleId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SQuestCycleContentsPlayStartReq>
        {
            public override void Write(IBuffer buffer, C2SQuestCycleContentsPlayStartReq obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
            }

            public override C2SQuestCycleContentsPlayStartReq Read(IBuffer buffer)
            {
                C2SQuestCycleContentsPlayStartReq obj = new C2SQuestCycleContentsPlayStartReq();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
