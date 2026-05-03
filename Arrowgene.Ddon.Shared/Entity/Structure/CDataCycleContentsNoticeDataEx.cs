using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model.Quest;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataCycleContentsNoticeDataEx
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
        public uint Unk0 { get; set; }
        public uint Unk1 { get; set; }

        public class Serializer : EntitySerializer<CDataCycleContentsNoticeDataEx>
        {
            public override void Write(IBuffer buffer, CDataCycleContentsNoticeDataEx obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteUInt32(buffer, obj.QuestScheduleId);
                WriteUInt32(buffer, (uint)obj.QuestId);
                WriteUInt32(buffer, obj.CategoryType);
                WriteUInt64(buffer, obj.PeriodStart);
                WriteUInt64(buffer, obj.PeriodEnd);
                WriteByte(buffer, obj.NoticeType);
                WriteByte(buffer, obj.IsSystemNotice);
                WriteByte(buffer, obj.PartyMemberNum);
                WriteBool(buffer, obj.IsPlay);
                WriteBool(buffer, obj.IsGetReward);
                WriteEntityList(buffer, obj.QuestProcessStateList);
                WriteUInt32(buffer, obj.Unk0);
                WriteUInt32(buffer, obj.Unk1);
            }

            public override CDataCycleContentsNoticeDataEx Read(IBuffer buffer)
            {
                CDataCycleContentsNoticeDataEx obj = new CDataCycleContentsNoticeDataEx();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.QuestScheduleId = ReadUInt32(buffer);
                obj.QuestId = (QuestId)ReadUInt32(buffer);
                obj.CategoryType = ReadUInt32(buffer);
                obj.PeriodStart = ReadUInt64(buffer);
                obj.PeriodEnd = ReadUInt64(buffer);
                obj.NoticeType = ReadByte(buffer);
                obj.IsSystemNotice = ReadByte(buffer);
                obj.PartyMemberNum = ReadByte(buffer);
                obj.IsPlay = ReadBool(buffer);
                obj.IsGetReward = ReadBool(buffer);
                obj.QuestProcessStateList = ReadEntityList<CDataQuestProcessState>(buffer);
                obj.Unk0 = ReadUInt32(buffer);
                obj.Unk1 = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
