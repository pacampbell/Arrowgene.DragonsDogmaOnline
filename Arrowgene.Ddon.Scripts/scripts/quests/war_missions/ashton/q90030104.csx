/**
 * @brief War Mission - Dacreim Fortress Recapture Battle: Enemy Annihilation (ダクレイム砦奪還戦　戦況：敵軍殲滅)
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    public override QuestType QuestType => QuestType.WarMission;
    public override QuestId QuestId => (QuestId)90030104;
    public override ushort RecommendedLevel => 85;
    public override byte MinimumItemRank => 86;
    public override bool IsDiscoverable => false;
    public override byte SituationLevel => 1;

    private static class Purpose
    {
        public const int MainObjectiveSecure4Min = 0; // [Main Objective] Secure a route to advance into the fort (4 min left) 
        public const int MainObjectiveConquerFortCenter5Min = 1; // [Main Objective] Conquer fort center (5 min left)
        public const int MainObjectiveConquerFortCenter4Min = 2; // [Main Objective] Conquer fort center(4 min left)
        public const int MainObjectiveConquerFortCenter3Min = 3; // [Main Objective] Conquer fort center (3 min left)
        public const int MainObjectiveConquerFortCenter2Min = 4; // [Main Objective] Conquer fort center (2 min left)
        public const int MainObjectiveConquerFortCenter1Min = 5; // [Main Objective] Conquer fort center (1 min left)

        public const int MainObjectiveConquerFortSides5Min = 6; // [Main Objective] Conquer fort sides (5 min left)
        public const int MainObjectiveConquerFortSides4Min = 7; // [Main Objective] Conquer fort sides (4 min left)
        public const int MainObjectiveConquerFortSides3Min = 8; // [Main Objective] Conquer fort sides (3 min left)
        public const int MainObjectiveConquerFortSides2Min = 9; // [Main Objective] Conquer fort sides (2 min left)
        public const int MainObjectiveConquerFortSides1Min = 10; // [Main Objective] Conquer fort sides (1 min left)

        public const int UrgentMessageArtilleryReinforcement2Min = 11; // [Urgent Message] Artillery reinforcement "Weapon Reinforcement Unit" arrives! (2 min left)
        public const int UrgentMessageArtilleryReinforcement1Min = 12; // [Urgent Message] Artillery reinforcement "Weapon Reinforcement Unit" arrives! (1 min left)
        public const int UrgentMessageArtilleryGuard2Min = 13; //  [Urgent Message] "Guard" with key to artillery battery arrives!
        public const int UrgentMessageArtilleryGuard1Min = 14; //  [Urgent Message] "Guard" with key to artillery battery arrives!

        public const int MainObjectiveReconquerForCenter = 15; // [Main Objective] Reconquer central enemy camp

        public const int UrgentOrderDefeatBeastCommander10min = 16; // [Urgent Order] Bring down "Beast Commander" that appeared (10 min left)
        public const int UrgentOrderDefeatBeastCommander9min = 22; // [Urgent Order] Bring down "Beast Commander" that appeared (9 min left)
        public const int UrgentOrderDefeatBeastCommander8min = 23; // [Urgent Order] Bring down "Beast Commander" that appeared (8 min left)
        public const int UrgentOrderDefeatBeastCommander7min = 24; // [Urgent Order] Bring down "Beast Commander" that appeared (7 min left)
        public const int UrgentOrderDefeatBeastCommander6min = 25; // [Urgent Order] Bring down "Beast Commander" that appeared (6 min left)
        public const int UrgentOrderDefeatBeastCommander5min = 26; // [Urgent Order] Bring down "Beast Commander" that appeared (5 min left)
        public const int UrgentOrderDefeatBeastCommander4min = 27; // [Urgent Order] Bring down "Beast Commander" that appeared (4 min left)
        public const int UrgentOrderDefeatBeastCommander3min = 28; // [Urgent Order] Bring down "Beast Commander" that appeared (3 min left)
        public const int UrgentOrderDefeatBeastCommander2min = 29; // [Urgent Order] Bring down "Beast Commander" that appeared (2 min left)
        public const int UrgentOrderDefeatBeastCommander1min = 30; // [Urgent Order] Bring down "Beast Commander" that appeared (1 min left)

        public const int ConquestCompleteRewardsTreasury = 17; // [Conquest Complete] Obtain rewards in underground treasury

        public const int MainObjectiveSecure3Min = 18;
        public const int MainObjectiveSecure2Min = 19;
        public const int MainObjectiveSecure1Min = 20;
        // 21; Unused
    }

    private enum ObjectiveId
    {
        MainObjectiveSecure = 0,
        MainObjectiveConquerFortCenter = 1,
        MainObjectiveConquerFortSides = 2,
        MainObjectiveReconquerForCenter = 3,
        UrgentMessageArtilleryReinforcement = 4,
        UrgentMessageArtilleryGuard = 5,
        UrgentOrderDefeatBeastCommander = 6,
        ConquestCompleteRewardsTreasury = 7
    }

    private static readonly Dictionary<ObjectiveId, List<int>> ObjectiveStates = new()
    {
        [ObjectiveId.MainObjectiveSecure] = [
            Purpose.MainObjectiveSecure4Min,
            Purpose.MainObjectiveSecure3Min,
            Purpose.MainObjectiveSecure2Min,
            Purpose.MainObjectiveSecure1Min,
        ],
        [ObjectiveId.MainObjectiveConquerFortCenter] = [
            Purpose.MainObjectiveConquerFortCenter5Min,
            Purpose.MainObjectiveConquerFortCenter4Min,
            Purpose.MainObjectiveConquerFortCenter3Min,
            Purpose.MainObjectiveConquerFortCenter2Min,
            Purpose.MainObjectiveConquerFortCenter1Min,
        ],
        [ObjectiveId.MainObjectiveConquerFortSides] = [
            Purpose.MainObjectiveConquerFortSides5Min,
            Purpose.MainObjectiveConquerFortSides4Min,
            Purpose.MainObjectiveConquerFortSides3Min,
            Purpose.MainObjectiveConquerFortSides2Min,
            Purpose.MainObjectiveConquerFortSides1Min,
        ],
        [ObjectiveId.MainObjectiveReconquerForCenter] = [
            Purpose.MainObjectiveReconquerForCenter
        ],
        [ObjectiveId.UrgentMessageArtilleryReinforcement] = [
            Purpose.UrgentMessageArtilleryReinforcement2Min,
            Purpose.UrgentMessageArtilleryReinforcement1Min,
        ],
        [ObjectiveId.UrgentMessageArtilleryGuard] = [
            Purpose.UrgentMessageArtilleryGuard2Min,
            Purpose.UrgentMessageArtilleryGuard1Min,
        ],
        [ObjectiveId.UrgentOrderDefeatBeastCommander] = [
            Purpose.UrgentOrderDefeatBeastCommander10min,
            Purpose.UrgentOrderDefeatBeastCommander9min,
            Purpose.UrgentOrderDefeatBeastCommander8min,
            Purpose.UrgentOrderDefeatBeastCommander7min,
            Purpose.UrgentOrderDefeatBeastCommander6min,
            Purpose.UrgentOrderDefeatBeastCommander5min,
            Purpose.UrgentOrderDefeatBeastCommander4min,
            Purpose.UrgentOrderDefeatBeastCommander3min,
            Purpose.UrgentOrderDefeatBeastCommander2min,
            Purpose.UrgentOrderDefeatBeastCommander1min,
        ],
        [ObjectiveId.ConquestCompleteRewardsTreasury] = [
            Purpose.ConquestCompleteRewardsTreasury
        ]
    };

    private class InstanceData
    {
        public static void SetObjective(QuestState questState, ObjectiveId objectiveId, int objectiveValue)
        {
            questState.InstanceVars.SetData<int>("objective_id", (int)objectiveId);
            questState.InstanceVars.SetData<int>("objective_value", objectiveValue);
        }

        public static ObjectiveId GetObjectiveId(QuestState questState)
        {
            return (ObjectiveId)questState.InstanceVars.GetData<int>("objective_id");
        }

        public static int GetObjectiveValue(QuestState questState)
        {
            return questState.InstanceVars.GetData<int>("objective_value");
        }
    }

    protected override void InitializeState()
    {
        MissionParams.MinimumMembers = 1;
        MissionParams.MaximumMembers = 8;
        MissionParams.IsSolo = false;
        MissionParams.PlaytimeInSeconds = 1200;
        MissionParams.ContentType = QuestContentType.FortDefense;

        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted((QuestId)30040));

        AddRankTier(1, 250);
        AddRankTier(2, 150);
        AddRankTier(3, 120);
        AddRankTier(4, 50);
        AddRankTier(5, 0);
    }

    public override void InitializeInstanceState(QuestState questState)
    {
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);
        process0.AddIsGatherPartyInStageBlock(QuestAnnounceType.Start, Stage.DacreimFortress1);
        process0.AddNoProgressBlock()
            .AddCallback((param) =>
            {
                var purpose = ObjectiveStates[ObjectiveId.MainObjectiveSecure][0];
                param.ResultCommands.AddResultCmdCyclePurpose(purpose, QuestEndContentsAnnounceType.Purpose);
                // param.ResultCommands.AddResultCmdRemoveCyclePurpose(purpose, QuestEndContentsAnnounceType.Purpose);
                InstanceData.SetObjective(param.QuestState, ObjectiveId.MainObjectiveSecure, purpose);
            });
        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
