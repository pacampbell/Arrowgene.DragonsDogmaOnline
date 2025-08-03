using Arrowgene.Buffers;
using System;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataCycleContentsNews
    {
        public uint CycleContentsScheduleId { get; set; }
        public DateTimeOffset Begin { get; set; }
        public DateTimeOffset End { get; set; }
        public byte Category { get; set; }
        public uint CategoryType { get; set; }
        public List<CDataRewardItem> RewardItemList { get; set; } = [];
        public List<CDataCycleContentsNewsDetail> DetailList { get; set; } = [];
        public List<CDataCycleContentsRank> CycleContentsRankList { get; set; } = [];
        public uint TotalPoint { get; set; }
        public uint PlayNum { get; set; }
        public bool IsCreateRanking { get; set; }
        public List<CDataQuestEnemyInfo> QuestEnemyInfoList { get; set; } = new();
        public List<CDataCycleContentsNewsUnk> Unk0List { get; set; } = new();
        public DateTimeOffset ProgressStart { get; set; } // Progress Start
        public DateTimeOffset ProgressEnd { get; set; } // Progress end
        public DateTimeOffset ResultAnalysisStart { get; set; } // Result Analysis Start
        public DateTimeOffset ResultAnalysisEnd { get; set; } // Result Analysis End
        public DateTimeOffset RewardDistributionStart { get; set; } // Reward Distribution Start
        public DateTimeOffset RewardDistributionEnd { get; set; } // Reward Distribution End

        public class Serializer : EntitySerializer<CDataCycleContentsNews>
        {
            public override void Write(IBuffer buffer, CDataCycleContentsNews obj)
            {
                WriteUInt32(buffer, obj.CycleContentsScheduleId);
                WriteInt64(buffer, obj.Begin.ToUnixTimeSeconds());
                WriteInt64(buffer, obj.End.ToUnixTimeSeconds());
                WriteByte(buffer, obj.Category);
                WriteUInt32(buffer, obj.CategoryType);
                WriteEntityList(buffer, obj.RewardItemList);
                WriteEntityList(buffer, obj.DetailList);
                WriteEntityList(buffer, obj.CycleContentsRankList);
                WriteUInt32(buffer, obj.TotalPoint);
                WriteUInt32(buffer, obj.PlayNum);
                WriteBool(buffer, obj.IsCreateRanking);
                WriteEntityList(buffer, obj.QuestEnemyInfoList);
                WriteEntityList(buffer, obj.Unk0List);
                WriteInt64(buffer, obj.ProgressStart.ToUnixTimeSeconds());
                WriteInt64(buffer, obj.ProgressEnd.ToUnixTimeSeconds());
                WriteInt64(buffer, obj.ResultAnalysisStart.ToUnixTimeSeconds());
                WriteInt64(buffer, obj.ResultAnalysisEnd.ToUnixTimeSeconds());
                WriteInt64(buffer, obj.RewardDistributionStart.ToUnixTimeSeconds());
                WriteInt64(buffer, obj.RewardDistributionEnd.ToUnixTimeSeconds());
            }

            public override CDataCycleContentsNews Read(IBuffer buffer)
            {
                CDataCycleContentsNews obj = new CDataCycleContentsNews();
                obj.CycleContentsScheduleId = ReadUInt32(buffer);
                obj.Begin = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.End = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.Category = ReadByte(buffer);
                obj.CategoryType = ReadUInt32(buffer);
                obj.RewardItemList = ReadEntityList<CDataRewardItem>(buffer);
                obj.DetailList = ReadEntityList<CDataCycleContentsNewsDetail>(buffer);
                obj.CycleContentsRankList = ReadEntityList<CDataCycleContentsRank>(buffer);
                obj.TotalPoint = ReadUInt32(buffer);
                obj.PlayNum = ReadUInt32(buffer);
                obj.IsCreateRanking = ReadBool(buffer);
                obj.QuestEnemyInfoList = ReadEntityList<CDataQuestEnemyInfo>(buffer);
                obj.Unk0List = ReadEntityList<CDataCycleContentsNewsUnk>(buffer);
                obj.ProgressStart = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.ProgressEnd = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.ResultAnalysisStart = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.ResultAnalysisEnd = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.RewardDistributionStart = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.RewardDistributionEnd = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                return obj;
            }
        }
    }
}
