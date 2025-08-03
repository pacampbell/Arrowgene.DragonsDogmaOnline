using System;
using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.GameServer.Dump;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuestGetCycleContentsStateListHandler : GameRequestPacketHandler<C2SQuestGetCycleContentsStateListReq, S2CQuestGetCycleContentsStateListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuestGetCycleContentsStateListHandler));

        public QuestGetCycleContentsStateListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CQuestGetCycleContentsStateListRes Handle(GameClient client, C2SQuestGetCycleContentsStateListReq request)
        {
            S2CQuestJoinLobbyQuestInfoNtc pcap = EntitySerializer.Get<S2CQuestJoinLobbyQuestInfoNtc>().Read(InGameDump.data_Dump_20B);
            S2CQuestJoinLobbyQuestInfoNtc ntc = new S2CQuestJoinLobbyQuestInfoNtc();

            ntc.WorldManageQuestOrderList = pcap.WorldManageQuestOrderList; // Recover paths + change vocation

            // TODO: Eventually populate all flags based player state for q7* quests
            foreach (var quest in ntc.WorldManageQuestOrderList)
            {
                quest.Param.QuestFlagList.Clear();
                quest.Param.QuestLayoutFlagList.Clear();
                quest.Param.QuestFlagList = client.Character.GetWorldManageQuestUnlocks((QuestId) quest.Param.QuestId);
                quest.Param.QuestLayoutFlagList = client.Character.GetWorldManageLayoutUnlocks((QuestId)quest.Param.QuestId);
            }

            ntc.QuestDefine = pcap.QuestDefine; // Recover quest log data to be able to accept quests
            ntc.QuestDefine.OrderMaxNum = Server.GameSettings.GameServerSettings.QuestOrderMax;
            ntc.QuestDefine.RewardBoxMaxNum = Server.GameSettings.GameServerSettings.RewardBoxMax;

            // pcap.MainQuestIdList; (this will add back all missing functionality which depends on complete MSQ)
            var completedMsq = client.Character.CompletedQuests.Values.Where(x => x.QuestType == QuestType.Main);
            foreach (var msq in completedMsq)
            {
                ntc.MainQuestIdList.Add(new CDataQuestId() { QuestId = (uint)msq.QuestId });
            }

            var completedTutorials = client.Character.CompletedQuests.Values.Where(x => x.QuestType == QuestType.Tutorial);
            foreach (var tut in completedTutorials)
            {
                ntc.TutorialQuestIdList.Add(new CDataQuestId() { QuestId = (uint)tut.QuestId });
            }

            // Add special quests not normally part of DDON
            foreach (var questId in new List<QuestId>() {QuestId.WorldManageMonsterCaution , QuestId.WorldManageJobTutorial, QuestId.WorldManageDebug})
            {
                var customWorldManageQuest = QuestManager.GetQuestByQuestId(questId);
                ntc.WorldManageQuestOrderList.Add(customWorldManageQuest.ToCDataWorldManageQuestOrderList(0));
            }

            List<QuestProgress> allQuestsInProgress = new();
            List<uint> priorityQuests = new();
            List<uint> decayedQuests = new();
            Server.Database.ExecuteInTransaction(connection =>
            {
                allQuestsInProgress = Server.Database.GetQuestProgressByType(client.Character.CommonId, QuestType.All, connection);
                priorityQuests = client.Party is not null ? Server.Database.GetPriorityQuestScheduleIds(client.Party.Leader.Client.Character.CommonId, connection) : new();

                decayedQuests = Server.LightQuestManager.HandleQuestDecay(client.Character, allQuestsInProgress, priorityQuests, connection).ToList();
            });

            foreach(var decayedScheduleId in decayedQuests)
            {
                var quest = QuestManager.GetQuestByScheduleId(decayedScheduleId);
                if (quest is null)
                {
                    continue;
                }
                ntc.ExpiredQuestList.Add(new()
                {
                    QuestId = quest.QuestId,
                    QuestScheduleId = quest.QuestScheduleId,
                });
            }

            foreach (var questProgress in allQuestsInProgress)
            {
                var quest = QuestManager.GetQuestByScheduleId(questProgress.QuestScheduleId);
                if (quest == null || !quest.IsActive(client))
                {
                    continue;
                }

                switch (questProgress.QuestType)
                {
                    case QuestType.Tutorial:
                        var tutorialQuest = quest.ToCDataTutorialQuestOrderList(questProgress.Step);
                        ntc.TutorialQuestOrderList.Add(tutorialQuest);
                        break;
                    case QuestType.WildHunt:
                        var mobHuntQuest = quest.ToCDataMobHuntQuestOrderList(questProgress.Step);
                        ntc.MobHuntQuestOrderList.Add(mobHuntQuest);
                        break;
                    case QuestType.Light:
                        var lightQuest = quest.ToCDataLightQuestOrderList(questProgress.Step);
                        if (lightQuest.Detail.BoardType == 1 && lightQuest.Detail.GetAp == 0)
                        {
                            lightQuest.Detail.GetAp = AreaRankManager.GetAreaPointReward(quest);
                        }
                        ntc.LightQuestOrderList.Add(lightQuest);
                        break;
                }
            }

            Dictionary<QuestSubstoryGroupId, Dictionary<uint, List<CDataQuestOrderList>>> substoryGroups = new();
            foreach (var questProgress in allQuestsInProgress)
            {
                if (questProgress.QuestType != QuestType.Substory) continue;

                var quest = QuestManager.GetQuestByScheduleId(questProgress.QuestScheduleId);
                if (quest == null || !quest.IsActive(client)) continue;

                var props = QuestManager.GetSubstoryQuestProperties(Server, quest.QuestId);
                if (props.SubstoryGroupId == QuestSubstoryGroupId.Invalid) continue;

                if (!substoryGroups.ContainsKey(props.SubstoryGroupId))
                    substoryGroups[props.SubstoryGroupId] = new();
                if (!substoryGroups[props.SubstoryGroupId].ContainsKey(props.SeqNo))
                    substoryGroups[props.SubstoryGroupId][props.SeqNo] = new();

                substoryGroups[props.SubstoryGroupId][props.SeqNo].Add(quest.ToCDataQuestOrderList(questProgress.Step));
            }

            foreach (var (substoryGroupId, seqData) in substoryGroups)
            {
                var entry = new CDataSubstoryQuestOrderList() { SubstoryGroupId = substoryGroupId };
                foreach (var (seqNo, questList) in seqData)
                {
                    entry.Details.Add(new CDataS2CQuestJoinLobbyQuestInfoNtcUnk0Unk1()
                    {
                        SequenceNo = seqNo,
                        Unk1 = 0,
                        Unk2 = 0,
                        Unk3 = [],
                        Unk4 = false,
                        QuestList = questList
                    });
                }
                ntc.SubstoryQuestOrderList.Add(entry);
            }

            if (client.Party != null)
            {
                foreach (var questScheduleId in priorityQuests)
                {
                    var quest = QuestManager.GetQuestByScheduleId(questScheduleId);
                    if (quest == null || !quest.IsActive(client))
                    {
                        continue;
                    }
                    
                    ntc.PriorityQuestList.Add(new CDataPriorityQuest()
                    {
                        QuestId = (uint)quest.QuestId,
                        QuestScheduleId = (uint)quest.QuestScheduleId
                    });
                }
            }

            if (decayedQuests.Count > 0)
            {
                client.Send(new S2CLobbyChatMsgNotice()
                {
                    Type = LobbyChatMsgType.ManagementAlertC,
                    Message = "A quest has been canceled because the\ndelivery time period has ended."
                });
            }

            client.Send(ntc);

            // {"Structure": {"CycleContentsStateList": [
            // {"CycleContentsScheduleId": 20642, "Category": 1, "CategoryType": 1008, "State": 2},
            // {"CycleContentsScheduleId": 20641, "Category": 1, "CategoryType": 1005, "State": 2},
            // {"CycleContentsScheduleId": 20640, "Category": 1, "CategoryType": 1006, "State": 2},
            // {"CycleContentsScheduleId": 20639, "Category": 1, "CategoryType": 1007, "State": 2}], "Error": 0, "Result": 0}}
            //
            // {"Structure":
            // {"CycleContentsStateList": [
            // {"CycleContentsScheduleId": 20654, "Category": 1, "CategoryType": 1008, "State": 4},
            // {"CycleContentsScheduleId": 20653, "Category": 1, "CategoryType": 1005, "State": 4},
            // {"CycleContentsScheduleId": 20655, "Category": 1, "CategoryType": 1006, "State": 4},
            // {"CycleContentsScheduleId": 20651, "Category": 1, "CategoryType": 1007, "State": 4}], "Error": 0, "Result": 0}}


            // The Category type appears to correspond to 
            // Dragon's Dogma Online\nativePC\rom\quest\qci_xx_yyyy.arc
            // xx = Category
            // yyyy = CategoryType
            //
            // qci_01_1001 - Battle For Gritten Fort
            // qci_01_1002 - The Crucible of Demons
            // qci_01_1003 - The Demon of Darkness
            // qci_01_1004 - The Deathly Battle of the Ancient Temple
            // qci_01_1005 - Dacreim Fortress Recapture Battle
            // qci_01_1006 - Jifule Fortress Capture Battle
            // qci_01_1007 - The Deathly Battle of the Forest of Mist
            // qci_01_1008 - Resurrection of the Flame of Despair
            // qci_01_1009 - Acre Selund War Chronicles
            //
            // qci_03_3002 - Ancient Warrior
            // qci_03_3003 - The Dazzling Gold
            // qci_03_3004 - The Lost Order
            // qci_03_3006 - Bloodbane Isle's Feast of Madness
            // qci_03_3007 - The Dragon Awakened
            var warMissionSchedule = Server.GameSettings.Get<Dictionary<uint, (byte Category, uint CategoryType, List<uint> QuestIds)>>("warmission", "WarMissionSchedule") ??
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_SERVER_CONFIG_ERROR);

            var warMissionControl = Server.GameSettings.Get<Dictionary<uint, List<(uint QuestId, byte NoticeType)>>>("warmission", "WarMissionControl") ??
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_SERVER_CONFIG_ERROR);

            // Only activate theaters that have at least one registered quest.
            var activeScheduleIds = warMissionSchedule
                .Where(kv => kv.Value.QuestIds.Any(qid => QuestManager.GetQuestByQuestId((QuestId)qid) != null))
                .Select(kv => kv.Key)
                .ToList();

            var (progressStart, progressEnd, resultEnd, rewardEnd) = QuestGetCycleContentsNewsListHandler.GetWarMissionDates();
            var now = DateTimeOffset.UtcNow;
            byte state = (now >= progressStart && now < progressEnd) ? (byte)2 : (byte)4;

            var result = new S2CQuestGetCycleContentsStateListRes();
            foreach (var scheduleId in activeScheduleIds)
            {
                var (category, categoryType, _) = warMissionSchedule[scheduleId];
                result.CycleContentsStateList.Add(new CDataCycleContentsStateList()
                {
                    CycleContentsScheduleId = scheduleId,
                    Category = (byte)category,
                    CategoryType = categoryType,
                    State = state,
                });
            }

            var winningPcap = EntitySerializer.Get<S2CQuestCycleContentsUnkNtc>().Read(situation_data0);

            foreach (var cycleContentScheduleId in activeScheduleIds)
            {
                var packet = new S2CQuestCycleContentsUnkNtc();
                // pcap: NoticeType=4 → Reward quest (index 3), NoticeType=5 → Management quest (index 0)
                // pcap: IsPlay=false, QuestProcessStateList=empty for both notice entries
                foreach (var index in new List<int> { 1, 0 })
                {
                    var warmissionMgmtQuestInfo = warMissionControl[cycleContentScheduleId][index];

                    var quest = QuestManager.GetQuestByQuestId((QuestId)warmissionMgmtQuestInfo.QuestId) ??
                        throw new ResponseErrorException(ErrorCode.ERROR_CODE_SERVER_CONFIG_ERROR);

                    CDataCycleContentsNoticeData data = new()
                    {
                        CycleContentsScheduleId = cycleContentScheduleId,
                        QuestScheduleId = quest.QuestScheduleId,
                        QuestId = quest.QuestId,
                        CategoryType = warMissionSchedule[cycleContentScheduleId].CategoryType,
                        NoticeType = warmissionMgmtQuestInfo.NoticeType,
                        PeriodStart = (ulong) progressStart.ToUnixTimeSeconds(),
                        PeriodEnd = (ulong) progressEnd.ToUnixTimeSeconds(),
                        IsPlay = false,
                        QuestProcessStateList = quest.ToCDataQuestList(0).QuestProcessStateList,
                        PartyMemberNum = 8,
                    };
                    packet.CycleContentsNoitceDataList.Add(data);
                }

                byte noticeType = 6;
                foreach (var questId in warMissionSchedule[cycleContentScheduleId].QuestIds)
                {
                    var quest = QuestManager.GetQuestByQuestId((QuestId)questId);
                    if (quest == null)
                    {
                        continue;
                    }

                    var detail = quest.ToCDataQuestContentsSituationInfoDetail();
                    detail.IsEnable = true;
                    detail.NoticeType = noticeType++;
                    packet.QuestContentsSituationInfoDetailList.Add(detail);
                }

                client.Send(packet);
            }

            return result;
        }

        private static readonly byte[] situation_data0 = [0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x50, 0xB2, 0x00, 0x05, 0xF6, 0xF1, 0x05, 0x5D, 0xC0, 0x15, 0x00, 0x00, 0x03, 0xED, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5D, 0xC3, 0x6C, 0x90, 0x02, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0x00, 0x00, 0x01, 0xBB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x10, 0x00, 0x00, 0x18, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x18, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x34, 0x00, 0x00, 0x0C, 0x9B, 0x04, 0x2C, 0x92, 0xB1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x18, 0xBA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x34, 0x00, 0x00, 0x0C, 0x9D, 0x04, 0x2C, 0x92, 0xB1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50, 0xB2, 0x00, 0x05, 0xF6, 0xF0, 0x05, 0x5D, 0xC0, 0x14, 0x00, 0x00, 0x03, 0xED, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5D, 0xCC, 0xA7, 0x10, 0x05, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x05, 0xF6, 0xF4, 0x05, 0x5D, 0xC0, 0x18, 0x00, 0x00, 0x00, 0x55, 0x00, 0x56, 0x06, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x75, 0x58, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x96, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x78, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x05, 0xF6, 0xF5, 0x05, 0x5D, 0xC0, 0x19, 0x00, 0x00, 0x00, 0x64, 0x00, 0x87, 0x07, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x76, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x05, 0xDC, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x05, 0x78, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x05, 0x46, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x05, 0x14, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0xF6, 0xF6, 0x05, 0x5D, 0xC0, 0x1A, 0x00, 0x00, 0x00, 0x64, 0x00, 0x87, 0x08, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x76, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x04, 0xB0, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x04, 0x4C, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x1A, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x03, 0xE8, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x30, 0x54, 0x07, 0x12, 0xF7];
    }
}
