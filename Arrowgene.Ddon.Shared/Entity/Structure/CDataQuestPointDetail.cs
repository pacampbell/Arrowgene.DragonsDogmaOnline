using Arrowgene.Buffers;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataQuestPointDetail
    {
        public List<CDataQuestPointDetailRecord> QuestPointList { get; set; } = new();
        public List<CDataQuestPointDetailRecord> QuestEnemyPointList { get; set; } = new();
        public List<CDataQuestPointDetailRecord> QuestRegionBreakPointList { get; set; } = new();
        public List<CDataQuestPointDetailRecord> QuestDeliveryPointList { get; set; } = new();

        public class Serializer : EntitySerializer<CDataQuestPointDetail>
        {
            public override void Write(IBuffer buffer, CDataQuestPointDetail obj)
            {
                WriteEntityList(buffer, obj.QuestPointList);
                WriteEntityList(buffer, obj.QuestEnemyPointList);
                WriteEntityList(buffer, obj.QuestRegionBreakPointList);
                WriteEntityList(buffer, obj.QuestDeliveryPointList);
            }

            public override CDataQuestPointDetail Read(IBuffer buffer)
            {
                CDataQuestPointDetail obj = new CDataQuestPointDetail();
                obj.QuestPointList = ReadEntityList<CDataQuestPointDetailRecord>(buffer);
                obj.QuestEnemyPointList = ReadEntityList<CDataQuestPointDetailRecord>(buffer);
                obj.QuestRegionBreakPointList = ReadEntityList<CDataQuestPointDetailRecord>(buffer);
                obj.QuestDeliveryPointList = ReadEntityList<CDataQuestPointDetailRecord>(buffer);
                return obj;
            }
        }
    }
}
