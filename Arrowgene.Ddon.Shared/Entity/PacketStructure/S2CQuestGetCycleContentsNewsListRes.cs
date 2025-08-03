using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestGetCycleContentsNewsListRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_QUEST_GET_CYCLE_CONTENTS_NEWS_LIST_RES;

        public List<CDataCycleContentsNews> CycleContentsNewsList { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestGetCycleContentsNewsListRes>
        {
            public override void Write(IBuffer buffer, S2CQuestGetCycleContentsNewsListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList(buffer, obj.CycleContentsNewsList);
            }

            public override S2CQuestGetCycleContentsNewsListRes Read(IBuffer buffer)
            {
                S2CQuestGetCycleContentsNewsListRes obj = new S2CQuestGetCycleContentsNewsListRes();
                ReadServerResponse(buffer, obj);
                obj.CycleContentsNewsList = ReadEntityList<CDataCycleContentsNews>(buffer);
                return obj;
            }
        }
    }
}
