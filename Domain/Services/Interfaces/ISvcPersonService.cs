using Domain.Entities;
using Domain;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface ISvcPersonService : ISvcBaseService<Person>
    {
        Task<BbqEventsResponses> HandleBarbecuePeopleInvitation(string personId, IEvent @event);
        Task InvitePersonToBarbecue(string personId, Bbq bbq);
        Task DeclinedInvitationPersonToBarbecue(string personId, Bbq bbq);
        Task<bool> IsModerator(string personId);
    }
}
