using Arrowgene.Buffers;
using System;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataCycleContentsNewsUnk
    {
        public uint QuestScheduleId { get; set; }
        public DateTimeOffset Begin { get; set; }
        public DateTimeOffset End { get; set; }

        public class Serializer : EntitySerializer<CDataCycleContentsNewsUnk>
        {
            public override void Write(IBuffer buffer, CDataCycleContentsNewsUnk obj)
            {
                WriteUInt32(buffer, obj.QuestScheduleId);
                WriteInt64(buffer, obj.Begin.ToUnixTimeSeconds());
                WriteInt64(buffer, obj.End.ToUnixTimeSeconds());
            }

            public override CDataCycleContentsNewsUnk Read(IBuffer buffer)
            {
                CDataCycleContentsNewsUnk obj = new CDataCycleContentsNewsUnk();
                obj.QuestScheduleId = ReadUInt32(buffer);
                obj.Begin = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                obj.End = DateTimeOffset.FromUnixTimeSeconds(ReadInt64(buffer));
                return obj;
            }
        }
    }
}
