using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Ddon.Shared.Network;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuestQuestOrderHandler : GameStructurePacketHandler<C2SQuestQuestOrderReq>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuestQuestOrderHandler));

        public QuestQuestOrderHandler(DdonGameServer server) : base(server)
        {
        }

        public override void Handle(GameClient client, StructurePacket<C2SQuestQuestOrderReq> packet)
        {
            var res = new S2CQuestQuestOrderRes();

            QuestId questId = (QuestId)packet.Structure.QuestScheduleId;
            if (client.Party.QuestState.GetActiveQuestIds().Contains(questId))
            {
                // return an error?
                client.Send(new S2CQuestQuestOrderRes()
                {
                    Error = (uint) ErrorCode.ERROR_CODE_QUEST_ADD_ITEM_FAILED
                });
                return;
            }

            var quest = QuestManager.GetQuest(questId);
            res.QuestProcessStateList = quest.ToCDataQuestList(0, false).QuestProcessStateList;
            client.Party.QuestState.AddNewQuest(quest, 0);
            Server.Database.InsertQuestProgress(client.Character.CommonId, quest.QuestId, quest.QuestType, 0);

            client.Send(res);
        }
    }
}
