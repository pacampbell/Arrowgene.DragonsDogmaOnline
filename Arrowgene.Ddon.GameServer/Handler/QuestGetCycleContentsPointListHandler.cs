using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;
using System.Collections.Generic;

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
            var ntc = QuestGetCycleContentsStateListHandler.BuildFortDefenseNtc(Server, request.CycleContentsScheduleId);
            if (ntc != null) client.Send(ntc);

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
