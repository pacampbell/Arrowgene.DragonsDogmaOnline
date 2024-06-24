using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrowgene.Ddon.Shared.Model.Quest
{
    public enum QuestContentType : int
    {
        None = 0,
        Normal = 1,
        WorldQuest = 2,
        Cycle = 3,
        End = 4,
        Debug = 5,
        QuickPartyMainQuest = 6,
        QuickPartyArea = 7,
        Large = 8
    }
}
