using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestRaidBossPointNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_RAID_BOSS_POINT_NTC;

        public uint CycleContentsScheduleId { get; set; }
        public CDataQuestPointDetail QuestPointDetail { get; set; } = new();

        public class Serializer : PacketEntitySerializer<S2CQuestRaidBossPointNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestRaidBossPointNtc obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteEntity(buffer, obj.QuestPointDetail);
            }

            public override S2CQuestRaidBossPointNtc Read(IBuffer buffer)
            {
                S2CQuestRaidBossPointNtc obj = new S2CQuestRaidBossPointNtc();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.QuestPointDetail = ReadEntity<CDataQuestPointDetail>(buffer);
                return obj;
            }
        }
    }
}
