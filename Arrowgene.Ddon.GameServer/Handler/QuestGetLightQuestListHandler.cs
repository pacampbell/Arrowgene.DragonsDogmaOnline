using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Ddon.Shared.Network;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuestGetLightQuestList : GameStructurePacketHandler<C2SQuestGetLightQuestListReq>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuestGetLightQuestList));
        
        public QuestGetLightQuestList(DdonGameServer server) : base(server)
        {
        }

        public override void Handle(GameClient client, StructurePacket<C2SQuestGetLightQuestListReq> packet)
        {
            S2CQuestGetLightQuestListRes res = new S2CQuestGetLightQuestListRes();

            var activeQuests = client.Party.QuestState.GetActiveQuestIds();
            var quests = QuestManager.GetQuestsByType(QuestType.Light);
            foreach (var quest in quests)
            {
                if (activeQuests.Contains(quest.Key))
                {
                    continue;
                }

                if (!QuestManager.IsBoardQuest(quest.Key))
                {
                    continue;
                }

                res.LightQuestList.Add(quest.Value.ToCDataLightQuestList(0));
            }

            client.Send(res);
        }
    }
}
