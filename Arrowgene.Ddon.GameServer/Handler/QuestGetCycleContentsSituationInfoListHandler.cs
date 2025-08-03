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

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuestGetCycleContentsSituationInfoListHandler : GameRequestPacketHandler<C2SQuestGetCycleContentsSituationInfoListReq, S2CQuestGetCycleContentsSituationInfoListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuestGetCycleContentsSituationInfoListHandler));

        public QuestGetCycleContentsSituationInfoListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CQuestGetCycleContentsSituationInfoListRes Handle(GameClient client, C2SQuestGetCycleContentsSituationInfoListReq request)
        {
            var res = new S2CQuestGetCycleContentsSituationInfoListRes()
            {
                CycleContentsScheduleId = request.CycleContentsScheduleId
            };

            S2CQuestGetCycleContentsSituationInfoListRes pcap = EntitySerializer.Get<S2CQuestGetCycleContentsSituationInfoListRes>().Read(pcap_data);

            var warMissionSchedule = Server.GameSettings.Get<Dictionary<uint, (byte Category, uint CategoryType, List<uint> QuestIds)>>("warmission", "WarMissionSchedule") ??
                 throw new ResponseErrorException(ErrorCode.ERROR_CODE_SERVER_CONFIG_ERROR);

            if (!warMissionSchedule.ContainsKey(request.CycleContentsScheduleId))
            {
                Logger.Debug($"No quest type mapping for CycleContentsScheduleId={request.CycleContentsScheduleId}");
                return res;
            }

            foreach (var questId in warMissionSchedule[request.CycleContentsScheduleId].QuestIds)
            {
                var quest = QuestManager.GetQuestByQuestId((QuestId)questId);
                if (quest == null)
                {
                    Logger.Debug($"No quest defined for QuestId={questId}");
                    continue;
                }

                res.QuestContentsSituationInfoList.Add(new CDataQuestContentsSituationInfo()
                {
                    QuestScheduleId = quest.QuestScheduleId,
                    QuestId = quest.QuestId
                });
            }

            return res;
        }

#if false
{
    "Structure":{
        "CycleContentsNewsList":[
            {
                "CycleContentsScheduleId":20659,
                "Begin":{
                    "DateTime":"10/31/2019 01:00:00",
                    "UtcDateTime":"10/31/2019 02:00:00",
                    "LocalDateTime":"10/31/2019 02:00:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080840000000000,
                    "UtcTicks":637080840000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "End":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "Category":1,
                "CategoryType":1008,
                "RewardItemList":[
                    
                ],
                "DetailList":[
                    {
                        "QuestId":90030404,
                        "BaseLevel":100,
                        "ContentJoinItemrank":116,
                        "SituationLevel":1
                    }
                ],
                "CycleContentsRankList":[
                    {
                        "Type":1,
                        "Rank":0,
                        "Score":0,
                        "Unk1":505,
                        "Unk2":3,
                        "UpdateDate":{
                            "DateTime":"10/31/2019 18:01:01",
                            "UtcDateTime":"10/31/2019 19:01:01",
                            "LocalDateTime":"10/31/2019 19:01:01",
                            "Date":"10/30/2019 23:00:00",
                            "Day":31,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":304,
                            "Hour":19,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":1,
                            "Month":10,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":1,
                            "Ticks":637081452610000000,
                            "UtcTicks":637081452610000000,
                            "TimeOfDay":"19:01:01",
                            "Year":2019
                        }
                    },
                    {
                        "Type":2,
                        "Rank":0,
                        "Score":0,
                        "Unk1":0,
                        "Unk2":0,
                        "UpdateDate":{
                            "DateTime":"12/31/1969 23:00:00",
                            "UtcDateTime":"01/01/1970 00:00:00",
                            "LocalDateTime":"01/01/1970 00:00:00",
                            "Date":"12/31/1969 23:00:00",
                            "Day":1,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":1,
                            "Hour":0,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":0,
                            "Month":1,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":0,
                            "Ticks":621355968000000000,
                            "UtcTicks":621355968000000000,
                            "TimeOfDay":"00:00:00",
                            "Year":1970
                        }
                    }
                ],
                "TotalPoint":0,
                "PlayNum":0,
                "IsCreateRanking":true,
                "EnemyInfo":[
                    {
                        "GroupId":"TheEvilDragon0",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"BlazeGoblin",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"BlazeGrigori",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"BlazeWolf",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"BurnedEnt",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"BlazeChimera",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Hellhound",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":false
                    }
                ],
                "Unk0":[
                    
                ],
                "UnkOffset0":{
                    "DateTime":"10/31/2019 01:00:00",
                    "UtcDateTime":"10/31/2019 02:00:00",
                    "LocalDateTime":"10/31/2019 02:00:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080840000000000,
                    "UtcTicks":637080840000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset1":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset2":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset3":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset4":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset5":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                }
            },
            {
                "CycleContentsScheduleId":20658,
                "Begin":{
                    "DateTime":"10/31/2019 01:01:00",
                    "UtcDateTime":"10/31/2019 02:01:00",
                    "LocalDateTime":"10/31/2019 02:01:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":1,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080840600000000,
                    "UtcTicks":637080840600000000,
                    "TimeOfDay":"02:01:00",
                    "Year":2019
                },
                "End":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "Category":1,
                "CategoryType":1005,
                "RewardItemList":[
                    
                ],
                "DetailList":[
                    {
                        "QuestId":90030104,
                        "BaseLevel":85,
                        "ContentJoinItemrank":86,
                        "SituationLevel":1
                    },
                    {
                        "QuestId":90030106,
                        "BaseLevel":100,
                        "ContentJoinItemrank":135,
                        "SituationLevel":3
                    }
                ],
                "CycleContentsRankList":[
                    {
                        "Type":1,
                        "Rank":0,
                        "Score":0,
                        "Unk1":504,
                        "Unk2":3,
                        "UpdateDate":{
                            "DateTime":"10/31/2019 18:01:01",
                            "UtcDateTime":"10/31/2019 19:01:01",
                            "LocalDateTime":"10/31/2019 19:01:01",
                            "Date":"10/30/2019 23:00:00",
                            "Day":31,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":304,
                            "Hour":19,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":1,
                            "Month":10,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":1,
                            "Ticks":637081452610000000,
                            "UtcTicks":637081452610000000,
                            "TimeOfDay":"19:01:01",
                            "Year":2019
                        }
                    },
                    {
                        "Type":2,
                        "Rank":0,
                        "Score":0,
                        "Unk1":0,
                        "Unk2":0,
                        "UpdateDate":{
                            "DateTime":"12/31/1969 23:00:00",
                            "UtcDateTime":"01/01/1970 00:00:00",
                            "LocalDateTime":"01/01/1970 00:00:00",
                            "Date":"12/31/1969 23:00:00",
                            "Day":1,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":1,
                            "Hour":0,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":0,
                            "Month":1,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":0,
                            "Ticks":621355968000000000,
                            "UtcTicks":621355968000000000,
                            "TimeOfDay":"00:00:00",
                            "Year":1970
                        }
                    }
                ],
                "TotalPoint":0,
                "PlayNum":0,
                "IsCreateRanking":true,
                "EnemyInfo":[
                    {
                        "GroupId":"Cragger",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Gorechimera",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Nightmare",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Bifrest",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGoremanticore",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"SquadLeaderDwarfOrc",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"BeastCommander",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGorecyclops",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGrimwarg",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"RangedSoldierDwarfOrc",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SwordSoldierDwarfOrc",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"GrimGoblinFighter",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"GrimGoblinLeader",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"GoblinShaman",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"GoblinAidShaman",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SlingGrimGoblin",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"HeavySoldierDwarfOrc",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"LittleCrag",
                        "Unk0":0,
                        "Lv":85,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SwordSoldierDwarfOrc",
                        "Unk0":0,
                        "Lv":1,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"WarReadyGorecyclops",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGoremanticore",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"BeastCommander",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Cragger",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Gorechimera",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGrimwarg",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"LittleCrag",
                        "Unk0":0,
                        "Lv":100,
                        "IsPartyRecommend":false
                    }
                ],
                "Unk0":[
                    
                ],
                "UnkOffset0":{
                    "DateTime":"10/31/2019 01:01:00",
                    "UtcDateTime":"10/31/2019 02:01:00",
                    "LocalDateTime":"10/31/2019 02:01:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":1,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080840600000000,
                    "UtcTicks":637080840600000000,
                    "TimeOfDay":"02:01:00",
                    "Year":2019
                },
                "UnkOffset1":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset2":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset3":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset4":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset5":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                }
            },
            {
                "CycleContentsScheduleId":20657,
                "Begin":{
                    "DateTime":"10/31/2019 01:02:00",
                    "UtcDateTime":"10/31/2019 02:02:00",
                    "LocalDateTime":"10/31/2019 02:02:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":2,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080841200000000,
                    "UtcTicks":637080841200000000,
                    "TimeOfDay":"02:02:00",
                    "Year":2019
                },
                "End":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "Category":1,
                "CategoryType":1006,
                "RewardItemList":[
                    
                ],
                "DetailList":[
                    {
                        "QuestId":90030204,
                        "BaseLevel":90,
                        "ContentJoinItemrank":96,
                        "SituationLevel":1
                    },
                    {
                        "QuestId":90030206,
                        "BaseLevel":105,
                        "ContentJoinItemrank":140,
                        "SituationLevel":3
                    }
                ],
                "CycleContentsRankList":[
                    {
                        "Type":1,
                        "Rank":0,
                        "Score":0,
                        "Unk1":503,
                        "Unk2":3,
                        "UpdateDate":{
                            "DateTime":"10/31/2019 18:01:01",
                            "UtcDateTime":"10/31/2019 19:01:01",
                            "LocalDateTime":"10/31/2019 19:01:01",
                            "Date":"10/30/2019 23:00:00",
                            "Day":31,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":304,
                            "Hour":19,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":1,
                            "Month":10,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":1,
                            "Ticks":637081452610000000,
                            "UtcTicks":637081452610000000,
                            "TimeOfDay":"19:01:01",
                            "Year":2019
                        }
                    },
                    {
                        "Type":2,
                        "Rank":0,
                        "Score":0,
                        "Unk1":0,
                        "Unk2":0,
                        "UpdateDate":{
                            "DateTime":"12/31/1969 23:00:00",
                            "UtcDateTime":"01/01/1970 00:00:00",
                            "LocalDateTime":"01/01/1970 00:00:00",
                            "Date":"12/31/1969 23:00:00",
                            "Day":1,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":1,
                            "Hour":0,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":0,
                            "Month":1,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":0,
                            "Ticks":621355968000000000,
                            "UtcTicks":621355968000000000,
                            "TimeOfDay":"00:00:00",
                            "Year":1970
                        }
                    }
                ],
                "TotalPoint":0,
                "PlayNum":0,
                "IsCreateRanking":true,
                "EnemyInfo":[
                    {
                        "GroupId":"Wight",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkeletonCyclops",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyOgre",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Golem",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"NecroMaster",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"SkeletonMage",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"BoltSkeletonBrute",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"WarReadySaurian",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"WarReadyGiantSaurian",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkeletonWarg",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkullLord",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkeletonBrute",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkeletonSorcerer",
                        "Unk0":0,
                        "Lv":90,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"WarReadyOgre",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"NecroMaster",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"SkeletonCyclops",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Golem",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGiantSaurian",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkeletonSorcerer",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkeletonWarg",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"Wight",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"SkullLord",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"BoltSkeletonBrute",
                        "Unk0":0,
                        "Lv":105,
                        "IsPartyRecommend":false
                    }
                ],
                "Unk0":[
                    
                ],
                "UnkOffset0":{
                    "DateTime":"10/31/2019 01:02:00",
                    "UtcDateTime":"10/31/2019 02:02:00",
                    "LocalDateTime":"10/31/2019 02:02:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":2,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080841200000000,
                    "UtcTicks":637080841200000000,
                    "TimeOfDay":"02:02:00",
                    "Year":2019
                },
                "UnkOffset1":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset2":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset3":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset4":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset5":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                }
            },
            {
                "CycleContentsScheduleId":20656,
                "Begin":{
                    "DateTime":"10/31/2019 01:03:00",
                    "UtcDateTime":"10/31/2019 02:03:00",
                    "LocalDateTime":"10/31/2019 02:03:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":3,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080841800000000,
                    "UtcTicks":637080841800000000,
                    "TimeOfDay":"02:03:00",
                    "Year":2019
                },
                "End":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "Category":1,
                "CategoryType":1007,
                "RewardItemList":[
                    
                ],
                "DetailList":[
                    {
                        "QuestId":90030304,
                        "BaseLevel":95,
                        "ContentJoinItemrank":106,
                        "SituationLevel":1
                    },
                    {
                        "QuestId":90030306,
                        "BaseLevel":110,
                        "ContentJoinItemrank":145,
                        "SituationLevel":3
                    }
                ],
                "CycleContentsRankList":[
                    {
                        "Type":1,
                        "Rank":0,
                        "Score":0,
                        "Unk1":502,
                        "Unk2":3,
                        "UpdateDate":{
                            "DateTime":"10/31/2019 18:01:01",
                            "UtcDateTime":"10/31/2019 19:01:01",
                            "LocalDateTime":"10/31/2019 19:01:01",
                            "Date":"10/30/2019 23:00:00",
                            "Day":31,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":304,
                            "Hour":19,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":1,
                            "Month":10,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":1,
                            "Ticks":637081452610000000,
                            "UtcTicks":637081452610000000,
                            "TimeOfDay":"19:01:01",
                            "Year":2019
                        }
                    },
                    {
                        "Type":2,
                        "Rank":0,
                        "Score":0,
                        "Unk1":0,
                        "Unk2":0,
                        "UpdateDate":{
                            "DateTime":"12/31/1969 23:00:00",
                            "UtcDateTime":"01/01/1970 00:00:00",
                            "LocalDateTime":"01/01/1970 00:00:00",
                            "Date":"12/31/1969 23:00:00",
                            "Day":1,
                            "DayOfWeek":"Thursday",
                            "DayOfYear":1,
                            "Hour":0,
                            "Millisecond":0,
                            "Microsecond":0,
                            "Nanosecond":0,
                            "Minute":0,
                            "Month":1,
                            "Offset":"00:00:00",
                            "TotalOffsetMinutes":0,
                            "Second":0,
                            "Ticks":621355968000000000,
                            "UtcTicks":621355968000000000,
                            "TimeOfDay":"00:00:00",
                            "Year":1970
                        }
                    }
                ],
                "TotalPoint":0,
                "PlayNum":0,
                "IsCreateRanking":true,
                "EnemyInfo":[
                    {
                        "GroupId":"EmpressGhost",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Witch",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"ShadowChimera",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"EliminatorSlay",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"Medusa",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"DeathKnight",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"WarReadyNightmare",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGorecyclops",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyGoremanticore",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyOgre",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"CursedDragon",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"AncestorOrigin",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Fodden",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"MistWyrm",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"MistDrake",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Goremanticore",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Nightmare",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"BlackGriffin",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Gorecyclops",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"ShadowMaster",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"GrudgeGhost",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"ShadowWolf",
                        "Unk0":0,
                        "Lv":95,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"EmpressGhost",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"WarReadyNightmare",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"ShadowMaster",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"Witch",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"GrudgeGhost",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"ShadowWolf",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":false
                    },
                    {
                        "GroupId":"ShadowChimera",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":true
                    },
                    {
                        "GroupId":"DeathKnight",
                        "Unk0":0,
                        "Lv":110,
                        "IsPartyRecommend":false
                    }
                ],
                "Unk0":[
                    
                ],
                "UnkOffset0":{
                    "DateTime":"10/31/2019 01:03:00",
                    "UtcDateTime":"10/31/2019 02:03:00",
                    "LocalDateTime":"10/31/2019 02:03:00",
                    "Date":"10/30/2019 23:00:00",
                    "Day":31,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":304,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":3,
                    "Month":10,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637080841800000000,
                    "UtcTicks":637080841800000000,
                    "TimeOfDay":"02:03:00",
                    "Year":2019
                },
                "UnkOffset1":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset2":{
                    "DateTime":"11/07/2019 00:00:00",
                    "UtcDateTime":"11/07/2019 01:00:00",
                    "LocalDateTime":"11/07/2019 01:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086852000000000,
                    "UtcTicks":637086852000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                },
                "UnkOffset3":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset4":{
                    "DateTime":"11/07/2019 01:00:00",
                    "UtcDateTime":"11/07/2019 02:00:00",
                    "LocalDateTime":"11/07/2019 02:00:00",
                    "Date":"11/06/2019 23:00:00",
                    "Day":7,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":311,
                    "Hour":2,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637086888000000000,
                    "UtcTicks":637086888000000000,
                    "TimeOfDay":"02:00:00",
                    "Year":2019
                },
                "UnkOffset5":{
                    "DateTime":"11/14/2019 00:00:00",
                    "UtcDateTime":"11/14/2019 01:00:00",
                    "LocalDateTime":"11/14/2019 01:00:00",
                    "Date":"11/13/2019 23:00:00",
                    "Day":14,
                    "DayOfWeek":"Thursday",
                    "DayOfYear":318,
                    "Hour":1,
                    "Millisecond":0,
                    "Microsecond":0,
                    "Nanosecond":0,
                    "Minute":0,
                    "Month":11,
                    "Offset":"00:00:00",
                    "TotalOffsetMinutes":0,
                    "Second":0,
                    "Ticks":637092900000000000,
                    "UtcTicks":637092900000000000,
                    "TimeOfDay":"01:00:00",
                    "Year":2019
                }
            }
        ],
        "Error":0,
        "Result":0
    }
}
#endif

        private static byte[] pcap_data = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50, 0xB1, 0x00, 0x00, 0x00, 0x02, 0x00, 0x05, 0xF6, 0xED, 0x05, 0x5D, 0xC0, 0x7C, 0x00, 0x05, 0xF6, 0xEF, 0x05, 0x5D, 0xC0, 0x7E, 0x00, 0x00, 0x00, 0x81, 0x00, 0x01, 0x01];
    }
}
