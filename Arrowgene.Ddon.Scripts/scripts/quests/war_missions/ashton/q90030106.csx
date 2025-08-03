/**
 * @brief War Mission - Dacreim Fortress Recapture Battle: Stirring General (ダクレイム砦奪還戦　戦況：大将奮起)
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    public override QuestType QuestType => QuestType.WarMission;
    public override QuestId QuestId => (QuestId)90030106;
    public override ushort RecommendedLevel => 100;
    public override byte MinimumItemRank => 135;
    public override bool IsDiscoverable => false;
    public override byte SituationLevel => 3;

	protected override void InitializeState()
    {
        // AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted((QuestId)30440));

        AddRankTier(1, 1200);
        AddRankTier(2, 1100);
        AddRankTier(3, 1050);
        AddRankTier(4, 1000);
        AddRankTier(5, 0);
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);
		process0.AddIsGatherPartyInStageBlock(QuestAnnounceType.None, Stage.DacreimFortress0);
        process0.AddNoProgressBlock();
        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
