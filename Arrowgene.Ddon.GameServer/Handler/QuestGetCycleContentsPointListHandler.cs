using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Logging;
using System.Drawing;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuestGetCycleContentsPointListHandler : GameRequestPacketHandler<C2SQuestGetCycleContentsPointListReq, S2CQuestGetCycleContentsPointListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuestGetCycleContentsPointListHandler));

        public QuestGetCycleContentsPointListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CQuestGetCycleContentsPointListRes Handle(GameClient client, C2SQuestGetCycleContentsPointListReq request)
        {
            return new S2CQuestGetCycleContentsPointListRes()
            {
                CycleContentsScheduleId = request.CycleContentsScheduleId,
                QuestPointDetail = new CDataQuestPointDetail()
                {
                    QuestPointList = [new() {QuestPointRecordId = 1, Point = 10}, new() { QuestPointRecordId = 2, Point = 90 }],
                    QuestEnemyPointList = [new() { QuestPointRecordId = 3, Point = 20 }, new() { QuestPointRecordId = 4, Point = 80 }],
                }
            };
        }
    }
}
