using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using EndlessConfiguration;

namespace BotCore.Preconditions
{
    public class RequireAllowedRoleAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var allowedRoles = Config.Admins;
            var user = context.User as SocketGuildUser;
            return allowedRoles.Any(role => user != null && user.Roles.Any(userRole => userRole.Name == role))
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("You are not allowed to use this command.");
        }
    }
}