using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestFortDefensePlayStartNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_FORT_DEFENSE_PLAY_START_NTC;

        public CDataCycleContentsPlayStartData FortDefensePlayStartData { get; set; } = new();
        public uint WarSituationLevel { get; set; }


        public class Serializer : PacketEntitySerializer<S2CQuestFortDefensePlayStartNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestFortDefensePlayStartNtc obj)
            {
                WriteEntity(buffer, obj.FortDefensePlayStartData);
                WriteUInt32(buffer, obj.WarSituationLevel);
            }

            public override S2CQuestFortDefensePlayStartNtc Read(IBuffer buffer)
            {
                S2CQuestFortDefensePlayStartNtc obj = new S2CQuestFortDefensePlayStartNtc();
                obj.FortDefensePlayStartData = ReadEntity<CDataCycleContentsPlayStartData>(buffer);
                obj.WarSituationLevel = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}

