using System;
using System.Linq;
using System.Threading.Tasks;
using BotCore.Configuration;
using Discord.Commands;

namespace BotCore.Preconditions
{
    public class InAllowedUsersAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var allowedUsers = Config.Admins;
            if (allowedUsers.Any(id => ulong.Parse(id) == context.User.Id))
            {
                return PreconditionResult.FromSuccess();
            }
            return PreconditionResult.FromError("You are not allowed to use this command.");
        }
    }
}