using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuestCycleContentsPlayStartHandler : GameRequestPacketHandler<C2SQuestCycleContentsPlayStartReq, S2CQuestCycleContentsPlayStartRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuestCycleContentsPlayStartHandler));

        public QuestCycleContentsPlayStartHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CQuestCycleContentsPlayStartRes Handle(GameClient client, C2SQuestCycleContentsPlayStartReq request)
        {
            var quest = QuestManager.GetQuestByScheduleId(request.CycleContentsScheduleId);
            if (quest == null)
            {
                var questScheduleId = BoardManager.GetQuestScheduleIdFromBoardId(client.Party.ContentId);
                quest = QuestManager.GetQuestByScheduleId(questScheduleId);
            }

            if (quest != null)
            {
                client.Party.ExmInProgress = true;
                client.Party.ExmInitialPartySize = (uint)client.Party.MemberCount();
                client.Party.QuestState.AddNewQuest(quest);

                switch(quest.MissionParams.ContentType)
                {
                    case QuestContentType.ExtremeMission:
                        {
                            var ntc = new S2CQuestTimeGainQuestPlayStartNtc()
                            {
                                TimeGainQuestPlayStartData = quest.ToCDataContentsPlayStartData()
                            };
                            ntc.TimeGainQuestPlayStartData.QuestPhaseGroupIdList = quest.MissionParams.QuestPhaseGroupIdList;
                            client.Party.SendToAll(ntc);
                        }
                        break;
                    case QuestContentType.FortDefense:
                        {
                            var ntc = new S2CQuestFortDefensePlayStartNtc()
                            {
                                FortDefensePlayStartData = quest.ToCDataCycleContentsPlayStartData(request.CycleContentsScheduleId),
                                WarSituationLevel = quest.SituationLevel
                            };
                            client.Party.SendToAll(ntc);
                            client.Party.SendToAll(new S2CQuestFortDefenseExtraSituationNtc() { SituationValue = 0 });
                        }
                        break;
                    case QuestContentType.BossRaid:
                        {
                            var ntc = new S2CQuestRaidBossPlayStartNtc()
                            {
                                RaidBossPlayStartData = new()
                                {
                                    RaidBossPlayStartData = quest.ToCDataCycleContentsPlayStartData(request.CycleContentsScheduleId),
                                    // ClearTimePointBonusList // TODO: Add to quest params
                                    // RaidBossEnemyParam      // TODO: Add to quest params
                                    // RegionBreakRewardList   // TODO: Add to quest params
                                }
                            };
                            client.Party.SendToAll(ntc);
                        }
                        break;
                    default:
                        Logger.Error($"Unexpected Content type {quest.MissionParams.ContentType} for ={request.CycleContentsScheduleId}");
                        break;
                }
            }
            else
            {
                Logger.Error($"No quest found for CycleContentsPlayStart with QuestScheduleId/CycleContentsScheduleId={request.CycleContentsScheduleId}");
            }

            return new S2CQuestCycleContentsPlayStartRes();
        }
    }
}
