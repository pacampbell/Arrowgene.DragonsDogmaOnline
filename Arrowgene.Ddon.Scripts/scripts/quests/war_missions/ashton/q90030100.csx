/**
 * @brief War Mission - Dacreim Fortress (Control Quest)
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    public override QuestType QuestType => QuestType.WarMission;
    public override QuestId QuestId => (QuestId)90030100;
    public override ushort RecommendedLevel => 0;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => false;

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);
        process0.AddNoProgressBlock();
        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
