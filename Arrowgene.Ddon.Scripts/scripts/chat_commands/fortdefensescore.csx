public class ChatCommand : IChatCommand
{
    public override AccountStateType AccountState => AccountStateType.Admin;
    public override string CommandName            => "fortdefensescore";
    public override string HelpText               => "usage: `/fortdefensescore scheduleId pointValue [situationValue]`";

    public override void Execute(DdonGameServer server, string[] command, GameClient client, ChatMessage message, List<ChatResponse> responses)
    {
        if (command.Length < 2)
        {
            responses.Add(ChatResponse.CommandError(client, "not enough arguments provided"));
            return;
        }

        try
        {
            uint scheduleId  = uint.Parse(command[0]);
            uint pointValue  = uint.Parse(command[1]);
            uint situationValue = command.Length >= 3 ? uint.Parse(command[2]) : 0;

            client.Party.SendToAll(new S2CQuestCycleContentsPointNtc()
            {
                CycleContentsScheduleId = scheduleId,
                QuestPointList = [new CDataCommonU32(pointValue)]
            });

            client.Party.SendToAll(new S2CQuestFortDefenseExtraSituationNtc()
            {
                SituationValue = situationValue
            });
        }
        catch (Exception)
        {
            responses.Add(ChatResponse.CommandError(client, "invalid arguments"));
        }
    }
}

return new ChatCommand();
