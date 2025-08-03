using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestCycleContentsPlayStartRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_QUEST_CYCLE_CONTENTS_PLAY_START_RES;

        public class Serializer : PacketEntitySerializer<S2CQuestCycleContentsPlayStartRes>
        {
            public override void Write(IBuffer buffer, S2CQuestCycleContentsPlayStartRes obj)
            {
                WriteServerResponse(buffer, obj);
            }

            public override S2CQuestCycleContentsPlayStartRes Read(IBuffer buffer)
            {
                S2CQuestCycleContentsPlayStartRes obj = new S2CQuestCycleContentsPlayStartRes();
                ReadServerResponse(buffer, obj);
                return obj;
            }
        }
    }
}
