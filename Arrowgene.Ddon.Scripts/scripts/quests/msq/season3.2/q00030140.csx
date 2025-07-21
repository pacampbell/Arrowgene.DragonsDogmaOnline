/**
 * @brief The Road to the Royal Capital
 */

#load "C:\Users\Paul\Git\Arrowgene.DragonsDogmaOnline\Arrowgene.Ddon.Cli\bin\Debug\net9.0\Files\Assets\scripts\libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheRoadToTheRoyalCapital;
    public override ushort RecommendedLevel => 89;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.None;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint LookoutCastleGuardTroop0 = 1774; // Lookout Castle Guard Troop
        public const uint LookoutCastleGuardTroop1 = 1775; // Lookout Castle Guard Troop

        public const uint NecroMaster = 1772; // Necro Master
        public const uint NecroFollower = 1773; // Necro Follower
    }

    private class QstLayoutFlag
    {
        // Megadosys Plateau (st0133)
        public const uint MegadosysPlateauNpcs0 = 7215; // Gillian, Gurdolin, Lise, Elliot
        public const uint MegadosysPlateauNpcs1 = 7244; // Gillian, Gurdolin
        public const uint MegadosysPlateauNpcs2 = 7243; // Nedo, Gurdolin, Lise, Elliot, Gillian, Meirova, Yuri, Ross
        public const uint MegadosysPlateauNpcs3 = 7436; // Gillian

        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs0 = 7002; // Nedo, Meirova
        public const uint LookoutCastleNpcs1 = 7214; // Gurdolin, Lise, Elliot, Gillian
        public const uint LookoutCastleBertha = 7655; // Bertha
    }

    private class MyQstFlag
    {
        public const uint StartAdds = 1;
        public const uint EndAdds = 2;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(89));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheBattleOfLookoutCastle));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 800000);
        AddWalletReward(WalletType.Gold, 90000);
        AddWalletReward(WalletType.RiftPoints, 9000);

        AddFixedItemReward(ItemId.RoyalCrestMedalMegadosysDistrict, 5);
        AddFixedItemReward(ItemId.UnappraisedWaterTrinketGeneral, 2);
        AddFixedItemReward(ItemId.ApMegadosysPlateau, 50);
    }

    protected override void InitializeEnemyGroups()
    {
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.MegadosysPlateau, 1, QuestEnemyPlacementType.Manual, new()
        {
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);
        process0.AddNpcTouchAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 0)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.AudienceChamber, 205, 0);
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1, 0, 1, NpcId.Meirova0, 21893)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleBertha);
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1, -9153, -83, -9090)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs1)
            .AddResultCmdReleaseAnnounce(ContentsRelease.None, flagInfo: QuestFlags.LookoutCastle.LookoutHarbor);
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.LookoutCastle1, 0, 13, QuestJumpType.After, Stage.MegadosysPlateau);
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.MegadosysPlateau, 0, 0);
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadosysPlateau, 0, 13, NpcId.Gillian0, 21912)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.LookoutCastleNpcs1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadosysPlateauNpcs0);
        process0.AddNoProgressBlock();
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 21887); // Return to Lestania and report to Joseph
        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
