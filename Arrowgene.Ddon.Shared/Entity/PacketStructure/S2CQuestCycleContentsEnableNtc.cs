using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuestCycleContentsEnableNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUEST_11_86_16_NTC;

        public uint CycleContentsScheduleId { get; set; }
        public bool IsEnable { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuestCycleContentsEnableNtc>
        {
            public override void Write(IBuffer buffer, S2CQuestCycleContentsEnableNtc obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteBool(buffer, obj.IsEnable);
            }

            public override S2CQuestCycleContentsEnableNtc Read(IBuffer buffer)
            {
                S2CQuestCycleContentsEnableNtc obj = new S2CQuestCycleContentsEnableNtc();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.IsEnable = ReadBool(buffer);
                return obj;
            }
        }
    }
}
