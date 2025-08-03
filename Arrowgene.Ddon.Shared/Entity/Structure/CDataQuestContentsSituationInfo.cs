using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model.Quest;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataQuestContentsSituationInfo
    {
        public uint QuestScheduleId { get; set; }
        public QuestId QuestId { get; set; }

        public class Serializer : EntitySerializer<CDataQuestContentsSituationInfo>
        {
            public override void Write(IBuffer buffer, CDataQuestContentsSituationInfo obj)
            {
                WriteUInt32(buffer, obj.QuestScheduleId);
                WriteUInt32(buffer, (uint)obj.QuestId);
            }

            public override CDataQuestContentsSituationInfo Read(IBuffer buffer)
            {
                CDataQuestContentsSituationInfo obj = new CDataQuestContentsSituationInfo();
                obj.QuestScheduleId = ReadUInt32(buffer);
                obj.QuestId = (QuestId)ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
