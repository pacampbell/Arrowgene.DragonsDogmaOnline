using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;
using System.Collections.Generic;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class JobOrbTreeGetJobOrbTreeGetAllJobOrbElementListHandler : GameRequestPacketHandler<C2SJobOrbTreeGetJobOrbTreeGetAllJobOrbElementListReq, S2CJobOrbTreeGetJobOrbTreeGetAllJobOrbElementListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(BazaarCancelHandler));

        public JobOrbTreeGetJobOrbTreeGetAllJobOrbElementListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CJobOrbTreeGetJobOrbTreeGetAllJobOrbElementListRes Handle(GameClient client, C2SJobOrbTreeGetJobOrbTreeGetAllJobOrbElementListReq request)
        {
            S2CJobOrbTreeGetJobOrbTreeGetAllJobOrbElementListRes response = new S2CJobOrbTreeGetJobOrbTreeGetAllJobOrbElementListRes();

            response.ElementList = Server.JobOrbUnlockManager.GetUpgradeList(request.JobId);

            return response;
        }
    }
}
