/**
 * Settings file for the Substory feature in warmission.csx
 * 
 *  CYCLE_CONTENTS_PERIOD_NONE = 0x0,
 *  CYCLE_CONTENTS_PERIOD_PREPARATION = 0x1,
 *  CYCLE_CONTENTS_PERIOD_HOLDING = 0x2,
 *  CYCLE_CONTENTS_PERIOD_RANKING = 0x3,
 *  CYCLE_CONTENTS_PERIOD_RECEIVING = 0x4,
 */

/// <summary>
/// Quests assigned for the Cycle Contents Schedule Id
/// </summary>
/* CycleContentsScheduleId = {Category, CategoryType, QuestIds = []} */
var WarMissionSchedule = new Dictionary<uint, (byte Category, uint CategoryType, List<uint> QuestIds)>
{
    [20639] = (1, 1007, [90030304, 90030305, 90030306]), // Abbas
    [20640] = (1, 1006, [90030204, 90030205, 90030206]), // Walt
    [20641] = (1, 1005, [90030104, 90030105, 90030106]), // Ashton
    [20642] = (1, 1008, [90030404, 90030405, 90030406]), // Zachary
    // [20643] = (1, 1009, [90040004, 90040005, 90040006, 90040007, 90040008]) // Raymond 
};

/// <summary>
/// Defines the control quests for the War Mission
/// </summary>
/* CycleContentsScheduleId = [(QuestId, NoticeType)] */
var WarMissionControl = new Dictionary<uint, List<(uint QuestId, byte NoticeType)>>
{
    //          Managment,     Order,         End,           Reward
    [20639] = [(90030300, 5), (90030301, 2), (90030302, 3), (90030303, 4)],
    [20640] = [(90030200, 5), (90030201, 2), (90030202, 3), (90030203, 4)],
    [20641] = [(90030100, 5), (90030101, 2), (90030102, 3), (90030103, 4)],
    [20642] = [(90030400, 5), (90030401, 2), (90030402, 3), (90030403, 4)],
    // [20643] = [90040000, 90040001, 90040002, q90040003]
};
