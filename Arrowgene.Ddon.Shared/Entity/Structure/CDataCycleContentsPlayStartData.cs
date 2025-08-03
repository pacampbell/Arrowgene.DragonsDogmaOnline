using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model.Quest;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataCycleContentsPlayStartData
    {
        public uint CycleContentsScheduleId { get; set; }
        public uint KeyId { get; set; }
        public uint QuestScheduleId { get; set; }
        public QuestId QuestId { get; set; }
        public uint BaseLevel { get; set; }
        public byte StartPos { get; set; }
        public List<CDataQuestProcessState> QuestProcessStateList { get; set; } = new();
        public List<CDataQuestEnemyInfo> QuestEnemyInfoList { get; set; } = new();
        public List<CDataQuestLayoutFlagSetInfo> QuestLayoutFlagSetInfoList { get; set; } = new();
        public List<CDataCommonPair<uint>> Unk0List { get; set; } = new();

        public class Serializer : EntitySerializer<CDataCycleContentsPlayStartData>
        {
            public override void Write(IBuffer buffer, CDataCycleContentsPlayStartData obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteUInt32(buffer, obj.KeyId);
                WriteUInt32(buffer, obj.QuestScheduleId);
                WriteUInt32(buffer, (uint) obj.QuestId);
                WriteUInt32(buffer, obj.BaseLevel);
                WriteByte(buffer, obj.StartPos);
                WriteEntityList(buffer, obj.QuestProcessStateList);
                WriteEntityList(buffer, obj.QuestEnemyInfoList);
                WriteEntityList(buffer, obj.QuestLayoutFlagSetInfoList);
                WriteEntityList(buffer, obj.Unk0List);
            }

            public override CDataCycleContentsPlayStartData Read(IBuffer buffer)
            {
                CDataCycleContentsPlayStartData obj = new CDataCycleContentsPlayStartData();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.KeyId = ReadUInt32(buffer);
                obj.QuestScheduleId = ReadUInt32(buffer);
                obj.QuestId = (QuestId) ReadUInt32(buffer);
                obj.BaseLevel = ReadUInt32(buffer);
                obj.StartPos = ReadByte(buffer);
                obj.QuestProcessStateList = ReadEntityList<CDataQuestProcessState>(buffer);
                obj.QuestEnemyInfoList = ReadEntityList<CDataQuestEnemyInfo>(buffer);
                obj.QuestLayoutFlagSetInfoList = ReadEntityList<CDataQuestLayoutFlagSetInfo>(buffer);
                obj.Unk0List = ReadEntityList<CDataCommonPair<uint>>(buffer);
                return obj;
            }
        }
    }
}
