using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class SvcPersonService : ISvcPersonService
    {
        private readonly IPersonRepository _personRepository;
        public async Task<Person> GetAsync(string id) => await _personRepository.GetAsync(id);

        public SvcPersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<bool> IsModerator(string personId)
        {
            try
            {
                var person = await _personRepository.GetAsync(personId);

                return person != null && person.IsCoOwner;
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return false;
            }
        }

        public async Task<BbqEventsResponses> HandleBarbecuePeopleInvitation(string personId, IEvent @event)
        {
            try
            {
                var person = await _personRepository.GetAsync(personId);

                if (person == null)
                    return new BbqEventsResponses(string.Format("Person not found personId {0}.", personId), false);

                person.Apply(@event);

                await _personRepository.SaveAsync(person);

                return new BbqEventsResponses($"Event handled applied successfully.", true, person.TakeSnapshot());
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to Handle Barbecue People answer invitations " + ex.Message, false);
            }
        }


        public async Task InvitePersonToBarbecue(string personId, Bbq bbq)
        {
            try
            {
                var person = await _personRepository.GetAsync(personId);

                person.Apply(new PersonHasBeenInvitedToBbq(bbq.Id, bbq.Date, bbq.Reason));

                await _personRepository.SaveAsync(person);
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...
            }
        }

        public async Task DeclinedInvitationPersonToBarbecue(string personId, Bbq bbq)
        {
            try
            {
                var person = await _personRepository.GetAsync(personId);

                person.Apply(new InviteWasDeclined(personId, bbq.Id, false));

                await _personRepository.SaveAsync(person);
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...
            }
        }
    }
}
