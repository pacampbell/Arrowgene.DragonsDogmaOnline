using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model.Quest;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataCycleContentsNoticeData
    {
        public uint CycleContentsScheduleId { get; set; }
        public uint QuestScheduleId { get; set; }
        public QuestId QuestId { get; set; }
        public uint CategoryType { get; set; }
        public ulong PeriodStart { get; set; }
        public ulong PeriodEnd { get; set; }
        public byte NoticeType { get; set; }
        public byte IsSystemNotice { get; set; }
        public byte PartyMemberNum { get; set; }
        public bool IsPlay { get; set; }
        public bool IsGetReward { get; set; }
        public List<CDataQuestProcessState> QuestProcessStateList { get; set; } = new();


        public class Serializer : EntitySerializer<CDataCycleContentsNoticeData>
        {
            public override void Write(IBuffer buffer, CDataCycleContentsNoticeData obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteUInt32(buffer, obj.QuestScheduleId);
                WriteUInt32(buffer, (uint) obj.QuestId);
                WriteUInt32(buffer, obj.CategoryType);
                WriteUInt64(buffer, obj.PeriodStart);
                WriteUInt64(buffer, obj.PeriodEnd);
                WriteByte(buffer, obj.NoticeType);
                WriteByte(buffer, obj.IsSystemNotice);
                WriteByte(buffer, obj.PartyMemberNum);
                WriteBool(buffer, obj.IsPlay);
                WriteBool(buffer, obj.IsGetReward);
                WriteEntityList(buffer, obj.QuestProcessStateList);
            }

            public override CDataCycleContentsNoticeData Read(IBuffer buffer)
            {
                CDataCycleContentsNoticeData obj = new CDataCycleContentsNoticeData();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.QuestScheduleId = ReadUInt32(buffer);
                obj.QuestId = (QuestId) ReadUInt32(buffer);
                obj.CategoryType = ReadUInt32(buffer);
                obj.PeriodStart = ReadUInt64(buffer);
                obj.PeriodEnd = ReadUInt64(buffer); ;
                obj.NoticeType = ReadByte(buffer);
                obj.IsSystemNotice = ReadByte(buffer);
                obj.PartyMemberNum = ReadByte(buffer);
                obj.IsPlay = ReadBool(buffer);
                obj.IsGetReward = ReadBool(buffer);
                obj.QuestProcessStateList = ReadEntityList<CDataQuestProcessState>(buffer);
                return obj;
            }
        }
    }
}
