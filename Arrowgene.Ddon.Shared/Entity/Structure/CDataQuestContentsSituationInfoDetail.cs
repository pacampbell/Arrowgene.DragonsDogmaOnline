using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model.Quest;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataQuestContentsSituationInfoDetail
    {
        public uint QuestScheduleId { get; set; }
        public QuestId QuestId { get; set; }
        public uint BaseLevel { get; set; }
        public ushort ContentJoinItemRank { get; set; }
        public byte NoticeType { get; set; }
        public List<CDataQuestOrderConditionParam> QuestOrderConditionParamList { get; set; } = new();
        public List<CDataRewardItem> RewardItemList { get; set; } = new();
        public uint ClearTimePointBonus { get; set; }
        public List<CDataCommonPair<uint>> DpRanksList { get; set; } = new();
        public bool IsEnable { get; set; }


        public class Serializer : EntitySerializer<CDataQuestContentsSituationInfoDetail>
        {
            public override void Write(IBuffer buffer, CDataQuestContentsSituationInfoDetail obj)
            {
                WriteUInt32(buffer, obj.QuestScheduleId);
                WriteUInt32(buffer, (uint)obj.QuestId);
                WriteUInt32(buffer, obj.BaseLevel);
                WriteUInt16(buffer, obj.ContentJoinItemRank);
                WriteByte(buffer, obj.NoticeType);
                WriteEntityList(buffer, obj.QuestOrderConditionParamList);
                WriteEntityList(buffer, obj.RewardItemList);
                WriteUInt32(buffer, obj.ClearTimePointBonus);
                WriteEntityList(buffer, obj.DpRanksList);
                WriteBool(buffer, obj.IsEnable);
            }

            public override CDataQuestContentsSituationInfoDetail Read(IBuffer buffer)
            {
                CDataQuestContentsSituationInfoDetail obj = new CDataQuestContentsSituationInfoDetail();
                obj.QuestScheduleId = ReadUInt32(buffer);
                obj.QuestId = (QuestId)ReadUInt32(buffer);
                obj.BaseLevel = ReadUInt32(buffer);
                obj.ContentJoinItemRank = ReadUInt16(buffer);
                obj.NoticeType = ReadByte(buffer);
                obj.QuestOrderConditionParamList = ReadEntityList<CDataQuestOrderConditionParam>(buffer);
                obj.RewardItemList = ReadEntityList<CDataRewardItem>(buffer);
                obj.ClearTimePointBonus = ReadUInt32(buffer);
                obj.DpRanksList = ReadEntityList<CDataCommonPair<uint>>(buffer);
                obj.IsEnable = ReadBool(buffer);
                return obj;
            }
        }
    }
}
