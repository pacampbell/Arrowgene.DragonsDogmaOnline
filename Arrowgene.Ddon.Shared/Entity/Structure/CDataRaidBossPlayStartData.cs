using Arrowgene.Buffers;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure;

public class CDataRaidBossPlayStartData
{
    public CDataCycleContentsPlayStartData RaidBossPlayStartData { get; set; } = new();
    public List<CDataClearTimePointBonus> ClearTimePointBonusList { get; set; } = new();
    public CDataRaidBossEnemyParam RaidBossEnemyParam { get; set; } = new();
    public List<CDataHasRegionBreakReward> RegionBreakRewardList { get; set; } = new();

    public class Serializer : EntitySerializer<CDataRaidBossPlayStartData>
    {
        public override void Write(IBuffer buffer, CDataRaidBossPlayStartData obj)
        {
            WriteEntity(buffer, obj.RaidBossPlayStartData);
            WriteEntityList(buffer, obj.ClearTimePointBonusList);
            WriteEntity(buffer, obj.RaidBossEnemyParam);
            WriteEntityList(buffer, obj.RegionBreakRewardList);
        }

        public override CDataRaidBossPlayStartData Read(IBuffer buffer)
        {
            CDataRaidBossPlayStartData obj = new CDataRaidBossPlayStartData();
            obj.RaidBossPlayStartData = ReadEntity<CDataCycleContentsPlayStartData>(buffer);
            obj.ClearTimePointBonusList = ReadEntityList<CDataClearTimePointBonus>(buffer);
            obj.RaidBossEnemyParam = ReadEntity<CDataRaidBossEnemyParam>(buffer);
            obj.RegionBreakRewardList = ReadEntityList<CDataHasRegionBreakReward>(buffer);
            return obj;
        }
    }
}
