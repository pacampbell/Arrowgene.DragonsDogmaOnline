using Arrowgene.Buffers;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataQuestPointDetailRecord
    {
        public uint QuestPointRecordId { get; set; }
        public uint Point { get; set; }

        public class Serializer : EntitySerializer<CDataQuestPointDetailRecord>
        {
            public override void Write(IBuffer buffer, CDataQuestPointDetailRecord obj)
            {
                WriteUInt32(buffer, obj.QuestPointRecordId);
                WriteUInt32(buffer, obj.Point);
            }

            public override CDataQuestPointDetailRecord Read(IBuffer buffer)
            {
                CDataQuestPointDetailRecord obj = new CDataQuestPointDetailRecord();
                obj.QuestPointRecordId = ReadUInt32(buffer);
                obj.Point = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
