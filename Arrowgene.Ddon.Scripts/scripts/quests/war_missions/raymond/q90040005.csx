/**
 * @brief War Mission - Battle with the Beast Commander: Dacreim Fortress (獣の将との戦い：ダクレイム砦)
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    public override QuestType QuestType => QuestType.WarMission;
    public override QuestId QuestId => (QuestId)90040005;
    public override ushort RecommendedLevel => 105;
    public override byte MinimumItemRank => 135;
    public override bool IsDiscoverable => false;

	protected override void InitializeState()
    {
        // AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted((QuestId)30440));
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
