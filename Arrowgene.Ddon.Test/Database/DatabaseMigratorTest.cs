using System;
using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database;
using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Database.Sql.Core.Migration;
using Arrowgene.Ddon.Shared.Entity;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Model.Quest;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.Ddon.Test.Database
{
    public class DatabaseMigratorTest : TestBase
    {
        private IDatabase db = new MockDatabase();

        public DatabaseMigratorTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TestStraightforwardMigration()
        {
            MockMigrationStrategy.CALL_COUNT = 0;
            var from0to1 = new MockMigrationStrategy() { From = 0, To = 1 };
            var from1to2 = new MockMigrationStrategy() { From = 1, To = 2 };
            var from2to3 = new MockMigrationStrategy() { From = 2, To = 3 };
            var strategies = new List<IMigrationStrategy>() {
                from0to1,
                from1to2,
                from2to3
            };
            var migrator = new DatabaseMigrator(strategies);
            bool result = migrator.MigrateDatabase(db, 0, 3);

            Assert.True(result);

            Assert.True(from0to1.Called);
            Assert.True(from0to1.CallOrder == 0);
            Assert.True(from1to2.Called);
            Assert.True(from1to2.CallOrder == 1);
            Assert.True(from2to3.Called);
            Assert.True(from2to3.CallOrder == 2);
        }

        [Fact]
        public void TestShortestMigrationPath()
        {
            MockMigrationStrategy.CALL_COUNT = 0;
            var from0to1 = new MockMigrationStrategy() { From = 0, To = 1 };
            var from1to2 = new MockMigrationStrategy() { From = 1, To = 2 };
            var from2to3 = new MockMigrationStrategy() { From = 2, To = 3 };
            var from0to3 = new MockMigrationStrategy() { From = 0, To = 3 };
            var strategies = new List<IMigrationStrategy>() {
                from0to1,
                from1to2,
                from2to3,
                from0to3,
            };
            var migrator = new DatabaseMigrator(strategies);
            bool result = migrator.MigrateDatabase(db, 0, 3);

            Assert.True(result);

            Assert.False(from0to1.Called);
            Assert.False(from1to2.Called);
            Assert.False(from2to3.Called);
            Assert.True(from0to3.Called);
            Assert.True(from2to3.CallOrder == 0);
        }

        [Fact]
        public void TestConvolutedMigrationPath()
        {
            MockMigrationStrategy.CALL_COUNT = 0;
            var from0to2 = new MockMigrationStrategy() { From = 0, To = 2 };
            var from2to1 = new MockMigrationStrategy() { From = 2, To = 1 };
            var from1to3 = new MockMigrationStrategy() { From = 1, To = 3 };
            var strategies = new List<IMigrationStrategy>() {
                from1to3,
                from2to1,
                from0to2,
            };
            var migrator = new DatabaseMigrator(strategies);
            bool result = migrator.MigrateDatabase(db, 0, 3);

            Assert.True(result);

            Assert.True(from0to2.Called);
            Assert.True(from0to2.CallOrder == 0);
            Assert.True(from2to1.Called);
            Assert.True(from2to1.CallOrder == 1);
            Assert.True(from1to3.Called);
            Assert.True(from1to3.CallOrder == 2);
        }

        [Fact]
        public void TestShortestConvolutedMigrationPath()
        {
            MockMigrationStrategy.CALL_COUNT = 0;
            var from0to2 = new MockMigrationStrategy() { From = 0, To = 2 };
            var from2to3 = new MockMigrationStrategy() { From = 2, To = 3 };
            var from3to1 = new MockMigrationStrategy() { From = 3, To = 1 };
            var from1to4 = new MockMigrationStrategy() { From = 1, To = 4 };
            var from0to1 = new MockMigrationStrategy() { From  = 0, To  = 1 };
            var strategies = new List<IMigrationStrategy>() {
                from0to2,
                from2to3,
                from3to1,
                from1to4,
                from0to1
            };
            var migrator = new DatabaseMigrator(strategies);
            bool result = migrator.MigrateDatabase(db, 0, 4);

            Assert.True(result);

            Assert.True(from0to1.Called);
            Assert.True(from0to1.CallOrder == 0);
            Assert.True(from1to4.Called);
            Assert.True(from1to4.CallOrder == 1);
            Assert.False(from0to2.Called);
            Assert.False(from2to3.Called);
            Assert.False(from3to1.Called);
        }

        [Fact]
        public void TestNoMigrationPath()
        {
            MockMigrationStrategy.CALL_COUNT = 0;
            var from0to1 = new MockMigrationStrategy() { From = 0, To = 1 };
            var from1to2 = new MockMigrationStrategy() { From = 1, To = 2 };
            var from2to3 = new MockMigrationStrategy() { From = 2, To = 3 };
            var strategies = new List<IMigrationStrategy>() {
                from0to1,
                from1to2,
                from2to3
            };
            var migrator = new DatabaseMigrator(strategies);

            Assert.Throws<Exception>(() => migrator.MigrateDatabase(db, 0, 4));

            Assert.False(from0to1.Called);
            Assert.False(from1to2.Called);
            Assert.False(from2to3.Called);
        }

        [Fact]
        public void TestNoMigrationNeeded()
        {
            MockMigrationStrategy.CALL_COUNT = 0;
            var from0to1 = new MockMigrationStrategy() { From = 0, To = 1 };
            var from1to2 = new MockMigrationStrategy() { From = 1, To = 2 };
            var from2to3 = new MockMigrationStrategy() { From = 2, To = 3 };
            var strategies = new List<IMigrationStrategy>() {
                from0to1,
                from1to2,
                from2to3
            };
            var migrator = new DatabaseMigrator(strategies);
            bool result = migrator.MigrateDatabase(db, 3, 3);

            Assert.True(result);

            Assert.False(from0to1.Called);
            Assert.False(from1to2.Called);
            Assert.False(from2to3.Called);
        }
    }

    class MockDatabase : IDatabase
    {
        public bool ExecuteInTransaction(Action<DbConnection> action) { 
            action.Invoke(null); return true; 
        }

        public Account CreateAccount(string name, string mail, string hash) { return new Account(); }
        public bool CreateCharacter(Character character) { return true; }
        public bool CreateDatabase() { return true; }
        public bool CreatePawn(Pawn pawn) { return true; }
        public bool DeleteAccount(int accountId) { return true; }
        public int DeleteBazaarExhibition(ulong bazaarId) { return 1; }
        public bool DeleteBoxRewardItem(uint commonId, uint uniqId) { return true; }
        public bool DeleteCharacter(uint characterId) { return true; }
        public bool DeleteCommunicationShortcut(uint characterId, uint pageNo, uint buttonNo) { return true; }
        public bool DeleteConnection(int serverId, int accountId) { return true; }
        public bool DeleteConnectionsByAccountId(int accountId) { return true; }
        public bool DeleteConnectionsByServerId(int serverId) { return true; }
        public int DeleteContact(uint requestingCharacterId, uint requestedCharacterId) { return 1; }
        public int DeleteContactById(uint id) { return 1; }
        public bool DeleteEquipItem(uint commonId, JobId job, EquipType equipType, byte equipSlot, string itemUId) { return true; }
        public bool DeleteEquipJobItem(uint commonId, JobId job, ushort slotNo) { return true; }
        public bool DeleteEquippedAbilities(uint commonId, JobId equippedToJob) { return true; }
        public bool DeleteEquippedAbility(uint commonId, JobId equippedToJob, byte slotNo) { return true; }
        public bool DeleteEquippedCustomSkill(uint commonId, JobId job, byte slotNo) { return true; }
        public bool DeleteNormalSkillParam(uint commonId, JobId job, uint skillNo) { return true; }
        public bool DeletePawn(uint pawnId) { return true; }
        public bool DeletePriorityQuest(uint characterCommonId, QuestId questId) { return true; }
        public bool DeleteReleasedWarpPoint(uint characterId, uint warpPointId) { return true; }
        public bool DeleteShortcut(uint characterId, uint pageNo, uint buttonNo) { return true; }
        public bool DeleteSpSkill(uint pawnId, JobId job, byte spSkillId) { return true; }
        public bool DeleteStorage(uint characterId, StorageType storageType) { return true; }
        public bool DeleteStorageItem(uint characterId, StorageType storageType, ushort slotNo) { return true; }
        public bool DeleteToken(string token) { return true; }
        public bool DeleteTokenByAccountId(int accountId) { return true; }
        public bool DeleteWalletPoint(uint characterId, WalletType type) { return true; }
        public void Execute(string sql) {}
        public void Execute(DbConnection conn, string sql) {}
        public List<BazaarExhibition> FetchCharacterBazaarExhibitions(uint characterId) { return new List<BazaarExhibition>(); }
        public CompletedQuest GetCompletedQuestsById(uint characterCommonId, QuestId questId) { return new CompletedQuest(); }
        public List<CompletedQuest> GetCompletedQuestsByType(uint characterCommonId, QuestType questType) { return new List<CompletedQuest>(); }
        public bool CreateMeta(DatabaseMeta meta) { return true; }
        public DatabaseMeta GetMeta() { return new DatabaseMeta(); }
        public List<QuestId> GetPriorityQuests(uint characterCommonId) { return new List<QuestId>(); }
        public QuestProgress GetQuestProgressById(uint characterCommonId, QuestId questId) { return new QuestProgress(); }
        public List<QuestProgress> GetQuestProgressByType(uint characterCommonId, QuestType questType) { return new List<QuestProgress>(); }
        public ulong InsertBazaarExhibition(BazaarExhibition exhibition) { return 1; }
        public bool InsertBoxRewardItems(uint commonId, QuestBoxRewards rewards) { return true; }
        public bool InsertCommunicationShortcut(uint characterId, CDataCommunicationShortCut communicationShortcut) { return true; }
        public bool InsertConnection(Connection connection) { return true; }
        public int InsertContact(uint requestingCharacterId, uint requestedCharacterId, ContactListStatus status, ContactListType type, bool requesterFavorite, bool requestedFavorite) { return 1; }
        public bool InsertEquipItem(uint commonId, JobId job, EquipType equipType, byte equipSlot, string itemUId) { return true; }
        public bool InsertEquipJobItem(string itemUId, uint commonId, JobId job, ushort slotNo) { return true; }
        public bool InsertEquippedAbility(uint commonId, JobId equipptedToJob, byte slotNo, Ability ability) { return true; }
        public bool InsertEquippedCustomSkill(uint commonId, byte slotNo, CustomSkill skill) { return true; }
        public bool InsertGainExtendParam(uint commonId, CDataOrbGainExtendParam Param) { return true; }
        public bool InsertIfNotExistCompletedQuest(uint characterCommonId, QuestId questId, QuestType questType) { return true; }
        public bool InsertIfNotExistsDragonForceAugmentation(uint commonId, uint elementId, uint pageNo, uint groupNo, uint indexNo) { return true; }
        public bool InsertIfNotExistsNormalSkillParam(uint commonId, CDataNormalSkillParam normalSkillParam) { return true; }
        public bool InsertIfNotExistsPawnTrainingStatus(uint pawnId, JobId job, byte[] pawnTrainingStatus) { return true; }
        public bool InsertIfNotExistsReleasedWarpPoint(uint characterId, ReleasedWarpPoint ReleasedWarpPoint) { return true; }
        public bool InsertIfNotExistsReleasedWarpPoints(uint characterId, List<ReleasedWarpPoint> ReleasedWarpPoint) { return true; }
        public bool InsertItem(Item item) { return true; }
        public bool InsertLearnedAbility(uint commonId, Ability ability) { return true; }
        public bool InsertLearnedCustomSkill(uint commonId, CustomSkill skill) { return true; }
        public bool InsertNormalSkillParam(uint commonId, CDataNormalSkillParam normalSkillParam) { return true; }
        public bool InsertPawnTrainingStatus(uint pawnId, JobId job, byte[] pawnTrainingStatus) { return true; }
        public bool InsertPriorityQuest(uint characterCommonId, QuestId questId) { return true; }
        public bool InsertQuestProgress(uint characterCommonId, QuestId questId, QuestType questType, uint step) { return true; }
        public bool InsertReleasedWarpPoint(uint characterId, ReleasedWarpPoint ReleasedWarpPoint) { return true; }
        public bool InsertSecretAbilityUnlock(uint commonId, SecretAbility secretAbility) { return true; }
        public bool InsertShortcut(uint characterId, CDataShortCut shortcut) { return true; }
        public bool InsertSpSkill(uint pawnId, JobId job, CDataSpSkill spSkill) { return true; }
        public bool InsertStorage(uint characterId, StorageType storageType, Storage storage) { return true; }
        public bool InsertStorageItem(uint characterId, StorageType storageType, ushort slotNo, string itemUId, uint itemNum) { return true; }
        public bool InsertWalletPoint(uint characterId, CDataWalletPoint walletPoint) { return true; }
        public bool RemoveQuestProgress(uint characterCommonId, QuestId questId, QuestType questType) { return true; }
        public bool ReplaceCharacterJobData(uint commonId, CDataCharacterJobData replacedCharacterJobData) { return true; }
        public bool ReplaceCommunicationShortcut(uint characterId, CDataCommunicationShortCut communicationShortcut) { return true; }
        public bool ReplaceEquipItem(uint commonId, JobId job, EquipType equipType, byte equipSlot, string itemUId) { return true; }
        public bool ReplaceEquipJobItem(string itemUId, uint commonId, JobId job, ushort slotNo) { return true; }
        public bool ReplaceEquippedAbilities(uint commonId, JobId equippedToJob, List<Ability> abilities) { return true; }
        public bool ReplaceEquippedAbility(uint commonId, JobId equipptedToJob, byte slotNo, Ability ability) { return true; }
        public bool ReplaceEquippedCustomSkill(uint commonId, byte slotNo, CustomSkill skill) { return true; }
        public bool ReplaceNormalSkillParam(uint commonId, CDataNormalSkillParam normalSkillParam) { return true; }
        public bool ReplacePawnTrainingStatus(uint pawnId, JobId job, byte[] pawnTrainingStatus) { return true; }
        public bool ReplaceReleasedWarpPoint(uint characterId, ReleasedWarpPoint ReleasedWarpPoint) { return true; }
        public bool ReplaceShortcut(uint characterId, CDataShortCut shortcut) { return true; }
        public bool ReplaceStorageItem(uint characterId, StorageType storageType, ushort slotNo, string itemUId, uint itemNum) { return true; }
        public bool ReplaceWalletPoint(uint characterId, CDataWalletPoint walletPoint) { return true; }
        public Account SelectAccountById(int accountId) { return new Account(); }
        public Account SelectAccountByLoginToken(string loginToken) { return new Account(); }
        public Account SelectAccountByName(string accountName) { return new Account(); }
        public List<BazaarExhibition> SelectActiveBazaarExhibitionsByItemIdExcludingOwn(uint itemId, uint excludedCharacterId) { return new List<BazaarExhibition>(); }
        public List<BazaarExhibition> SelectActiveBazaarExhibitionsByItemIdsExcludingOwn(List<uint> itemIds, uint excludedCharacterId) { return new List<BazaarExhibition>(); }
        public List<SecretAbility> SelectAllUnlockedSecretAbilities(uint commonId) { return new List<SecretAbility>(); }
        public BazaarExhibition SelectBazaarExhibitionByBazaarId(ulong bazaarId) { return new BazaarExhibition(); }
        public List<QuestBoxRewards> SelectBoxRewardItems(uint commonId) { return new List<QuestBoxRewards>(); }
        public Character SelectCharacter(uint characterId) { return new Character(); }
        public List<Character> SelectCharactersByAccountId(int accountId) { return new List<Character>(); }
        public List<Connection> SelectConnectionsByAccountId(int accountId) { return new List<Connection>(); }
        public ContactListEntity SelectContactListById(uint id) { return new ContactListEntity(); }
        public List<ContactListEntity> SelectContactsByCharacterId(uint characterId) { return new List<ContactListEntity>(); }
        public ContactListEntity SelectContactsByCharacterId(uint characterId1, uint characterId2) { return new ContactListEntity(); }
        public Item SelectItem(string uid) { return new Item(); }
        public List<CDataNormalSkillParam> SelectNormalSkillParam(uint commonId, JobId job) { return new List<CDataNormalSkillParam>(); }
        public CDataOrbGainExtendParam SelectOrbGainExtendParam(uint commonId) { return new CDataOrbGainExtendParam(); }
        public List<CDataReleaseOrbElement> SelectOrbReleaseElementFromDragonForceAugmentation(uint commonId) { return new List<CDataReleaseOrbElement>(); }
        public Pawn SelectPawn(uint pawnId) { return new Pawn(); }
        public List<Pawn> SelectPawnsByCharacterId(uint characterId) { return new List<Pawn>(); }
        public List<ReleasedWarpPoint> SelectReleasedWarpPoints(uint characterId) { return new List<ReleasedWarpPoint>(); }
        public GameToken SelectToken(string tokenStr) { return new GameToken(); }
        public GameToken SelectTokenByAccountId(int accountId) { return new GameToken(); }
        public bool SetMeta(DatabaseMeta meta) { return true; }
        public bool SetToken(GameToken token) { return true; }
        public bool UpdateAccount(Account account) { return true; }
        public int UpdateBazaarExhibiton(BazaarExhibition exhibition) { return 1; }
        public bool UpdateCharacterArisenProfile(Character character) { return true; }
        public bool UpdateCharacterBaseInfo(Character character) { return true; }
        public bool UpdateCharacterCommonBaseInfo(CharacterCommon common) { return true; }
        public bool UpdateCharacterJobData(uint commonId, CDataCharacterJobData updatedCharacterJobData) { return true; }
        public bool UpdateCharacterMatchingProfile(Character character) { return true; }
        public bool UpdateCommunicationShortcut(uint characterId, uint oldPageNo, uint oldButtonNo, CDataCommunicationShortCut updatedCommunicationShortcut) { return true; }
        public int UpdateContact(uint requestingCharacterId, uint requestedCharacterId, ContactListStatus status, ContactListType type, bool requesterFavorite, bool requestedFavorite) { return 1; }
        public bool UpdateEditInfo(CharacterCommon character) { return true; }
        public bool UpdateEquipItem(uint commonId, JobId job, EquipType equipType, byte equipSlot, string itemUId) { return true; }
        public bool UpdateEquippedAbility(uint commonId, JobId oldEquippedToJob, byte oldSlotNo, JobId equipptedToJob, byte slotNo, Ability ability) { return true; }
        public bool UpdateEquippedCustomSkill(uint commonId, JobId oldJob, byte oldSlotNo, byte slotNo, CustomSkill skill) { return true; }
        public bool UpdateLearnedAbility(uint commonId, Ability ability) { return true; }
        public bool UpdateLearnedCustomSkill(uint commonId, CustomSkill updatedSkill) { return true; }
        public bool UpdateNormalSkillParam(uint commonId, JobId job, uint skillNo, CDataNormalSkillParam normalSkillParam) { return true; }
        public bool UpdateOrbGainExtendParam(uint commonId, CDataOrbGainExtendParam Param) { return true; }
        public bool UpdatePawnBaseInfo(Pawn pawn) { return true; }
        public bool UpdatePawnTrainingStatus(uint pawnId, JobId job, byte[] pawnTrainingStatus) { return true; }
        public bool UpdateQuestProgress(uint characterCommonId, QuestId questId, QuestType questType, uint step) { return true; }
        public bool UpdateReleasedWarpPoint(uint characterId, ReleasedWarpPoint updatedReleasedWarpPoint) { return true; }
        public bool UpdateShortcut(uint characterId, uint oldPageNo, uint oldButtonNo, CDataShortCut updatedShortcut) { return true; }
        public bool UpdateStatusInfo(CharacterCommon character) { return true; }
        public bool UpdateStorage(uint characterId, StorageType storageType, Storage storage) { return true; }
        public bool UpdateWalletPoint(uint characterId, CDataWalletPoint updatedWalletPoint) { return true; }
        public bool MigrateDatabase(DatabaseMigrator migrator, uint toVersion) { return true; }

        public List<SystemMailMessage> SelectSystemMailMessages(uint characterId) { return new List<SystemMailMessage>(); }
        public SystemMailMessage SelectSystemMailMessage(ulong messageId) { return new SystemMailMessage(); }
        public bool UpdateSystemMailMessageState(ulong messageId, MailState messageState) {  return true; }
        public bool DeleteSystemMailMessage(ulong messageId) { return true; }
        public List<SystemMailAttachment> SelectAttachmentsForSystemMail(ulong messageId) { return new List<SystemMailAttachment>(); }
        public bool UpdateSystemMailAttachmentReceivedStatus(ulong messageId, ulong attachmentId, bool isReceived) {  return true; }
        public bool DeleteSystemMailAttachment(ulong messageId) { return true; }

        public bool InsertCrest(uint characterCommonId, string itemUId, uint slot, uint crestId, uint crestAmount) { return true; }
        public bool UpdateCrest(uint characterCommonId, string itemUId, uint slot, uint crestId, uint crestAmount) { return true; }
        public bool RemoveCrest(uint characterCommonId, string itemUId, uint slot) { return true; }
        public List<Crest> GetCrests(uint characterCommonId, string itemUId) { return new List<Crest>(); }
    }

    class MockMigrationStrategy : IMigrationStrategy
    {
        public static uint CALL_COUNT = 0;

        public uint From { get; set; }

        public uint To { get; set; }

        public bool Called { get; private set; } = false;
        public uint CallOrder { get; private set; }

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            Called = true;
            CallOrder = CALL_COUNT++;
            return true;
        }
    }
}
