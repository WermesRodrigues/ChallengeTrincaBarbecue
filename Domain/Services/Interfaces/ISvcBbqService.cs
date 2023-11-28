
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface ISvcBbqService : ISvcBaseService<Bbq>
    {
        Task<BbqEventsResponses> GetBarbecueCart(string personId, string bbqId);

        Task<BbqEventsResponses> CreateNewBarbecue(DateTime date, string reason, bool isTrincaPaying);

        Task<BbqEventsResponses> AcceptAndDeclineBarbecueInvite(string inviteId, string personId, bool isVeg, BarbecueInvitationType barbecueInvitationType);

        Task<BbqEventsResponses> ModerateBarbecue(string bbqId, bool gonnaHappen, bool isTrincaPaying);        

        Task<BbqEventsResponses> GetProposedBarbecues(string personId);
    }
}
