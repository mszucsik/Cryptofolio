using System.Threading.Tasks;
using Cryptofolio.Data;
using Cryptofolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Cryptofolio.Authorization
{
    public class IsOwnerController
                : AuthorizationHandler<OperationAuthorizationRequirement, Portfolio>
    {
        UserManager<IdentityUser> _userManager;

        public IsOwnerController(UserManager<IdentityUser>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Portfolio resource)
        {
            if (context.User == null || resource == null)
            {
                // Return Task.FromResult(0) if targeting a version of
                // .NET Framework older than 4.6:
                return Task.CompletedTask;
            }


            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}