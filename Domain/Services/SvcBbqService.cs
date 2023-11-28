using CrossCutting;
using Domain;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class SvcBbqService : ISvcBbqService
    {
        private readonly IBbqRepository _bbqRepository;
        private readonly ISvcPersonService _svcpersonService;
        private readonly SnapshotStore _snapshots;
        public async Task<Bbq> GetAsync(string id) => await _bbqRepository.GetAsync(id);

        public SvcBbqService(IBbqRepository bbqRepository, ISvcPersonService svcpersonService, SnapshotStore snapshots)
        {
            _bbqRepository = bbqRepository;
            _svcpersonService = svcpersonService;
            _snapshots = snapshots;
        }

        public async Task<BbqEventsResponses> GetBarbecueCart(string personId, string bbqId)
        {
            try
            {
                var isModerator = await _svcpersonService.IsModerator(personId);

                if (!isModerator)
                    return new BbqEventsResponses("Failed the personId is not a moderator.", false);

                var bbq = await _bbqRepository.GetAsync(bbqId);

                if (bbq == null)
                    return new BbqEventsResponses(string.Format("Barbecue not found Id {0}.", bbqId), false);

                if (bbq.BbqShopCart == null)
                    return new BbqEventsResponses($"The Cart of Barbecue is empty!.", false);

                return new BbqEventsResponses("Successfully fetched.", true, bbq.BbqShopCart.TakeSnapshot());
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to get a Barbecue Cart " + ex.Message, false);
            }
        }

        public async Task<BbqEventsResponses> CreateNewBarbecue(DateTime date, string reason, bool isTrincaPaying)
        {
            try
            {
                var churras = new Bbq();

                churras.Apply(new ThereIsSomeoneElseInTheMood(Guid.NewGuid(), date, reason, isTrincaPaying));

                await _bbqRepository.SaveAsync(churras);

                var Lookups = await _snapshots.SingleOrDefaultCollection<Lookups>("Lookups");

                //include ModeratorIds to invitations....
                foreach (var personId in Lookups.ModeratorIds)
                {
                    await _svcpersonService.InvitePersonToBarbecue(personId, churras);
                }

                return new BbqEventsResponses(string.Format("Hi, your Barbecue has been created with Id {0}", churras.Id), true, churras.TakeSnapshot());
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to create a Barbecue " + ex.Message, false);
            }
        }

        public async Task<BbqEventsResponses> GetProposedBarbecues(string personId)
        {
            try
            {
                var snapshots = new List<object>();

                var person = await _svcpersonService.GetAsync(personId);

                if (person == null)
                    return new BbqEventsResponses(string.Format("The user was not found Id {0}.", personId), false);

                //exclude hour from date....
                var bbqInvites = person.Invites.Where(i => i.Date.Date > DateTime.Now.Date && i.Status != InviteStatus.Declined)
                                               .Select(o => o.Id)
                                               .ToList();

                foreach (var bbqId in bbqInvites)
                {
                    var bbq = await _bbqRepository.GetAsync(bbqId);

                    if (bbq == null || bbq.Status == BbqStatus.ItsNotGonnaHappen)
                        continue;

                    snapshots.Add(bbq.TakeSnapshot());
                }

                return new BbqEventsResponses("Successfully fetched Proposed Barbecue.", true, snapshots);
            }
            catch (Exception ex)
            {
                //Improvements Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to get proposed barbecues " + ex.Message, false);
            }
        }

        public async Task<BbqEventsResponses> ModerateBarbecue(string bbqId, bool gonnaHappen, bool isTrincaPaying)
        {
            try
            {
                var bbq = await _bbqRepository.GetAsync(bbqId);

                if (bbq == null)
                    return new BbqEventsResponses(string.Format("Barbecue not found Id {0}.", bbqId), false);

                bbq.Apply(new BbqStatusUpdated(gonnaHappen, isTrincaPaying));

                await _bbqRepository.SaveAsync(bbq);

                var lookups = await _snapshots.SingleOrDefaultCollection<Lookups>("Lookups");

                //by default get all people include moderators, becuase if gonnaHappen==false will decline all invitations...
                var peopleFilter = lookups.PeopleIds.ToList();

                //filter PeopleIds Except ModeratorIds if gonnaHappen=true
                if (gonnaHappen)
                    peopleFilter = peopleFilter.Except(lookups.ModeratorIds).ToList();

                foreach (var personId in peopleFilter)
                {
                    //check if will gonna happen then invite the people...
                    if (gonnaHappen)
                    {
                        await _svcpersonService.InvitePersonToBarbecue(personId, bbq);
                    }
                    //gonnaHappen==false will decline all invitations include moderators...
                    else
                    {
                        await _svcpersonService.DeclinedInvitationPersonToBarbecue(personId, bbq);
                    }
                }

                if (!gonnaHappen)
                    return new BbqEventsResponses($"Barbecue was successfully canceled.", true, bbq.TakeSnapshot());

                return new BbqEventsResponses($"People were successfully invited to barbecue.", true, bbq.TakeSnapshot());
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to invite the moderators " + ex.Message, false);
            }
        }

        public async Task<BbqEventsResponses> AcceptAndDeclineBarbecueInvite(string inviteId, string personId, bool isVeg, BarbecueInvitationType barbecueInvitationType = BarbecueInvitationType.Peding)
        {
            try
            {
                var bbq = await _bbqRepository.GetAsync(inviteId);

                if (bbq == null)
                    return new BbqEventsResponses(string.Format("Barbecue not found Id {0}.", inviteId), false);

                if (bbq != null && bbq.Status != BbqStatus.PendingConfirmations)
                    return new BbqEventsResponses(string.Format("The current status of Barbecue is PendingConfirmations yet.", inviteId), false);

                //get Event handle to update the person and the next update the Cart.....
                var @event = GetEventHandle(inviteId, personId, isVeg, barbecueInvitationType);

                //send envent by parameters barbecueInvitationType
                var responseBarbecue = await _svcpersonService.HandleBarbecuePeopleInvitation(personId, @event);

                if (!responseBarbecue.IsOk)
                    return new BbqEventsResponses(responseBarbecue.MessageEvent, false);

                bbq.Apply(@event);

                await _bbqRepository.SaveAsync(bbq);

                return new BbqEventsResponses(responseBarbecue.MessageEvent, true, responseBarbecue.SnapshotObj);
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to Accept And DeclineBarbecue Invitations " + ex.Message, false);
            }
        }

        private IEvent GetEventHandle(string inviteId, string personId, bool isVeg, BarbecueInvitationType barbecueInvitationType)
        {
            switch (barbecueInvitationType)
            {
                case BarbecueInvitationType.Decline:
                    return new InviteWasDeclined(personId, inviteId, isVeg);
                case BarbecueInvitationType.Accept:
                    return new InviteWasAccepted(personId, inviteId, isVeg);
            }

            throw new NotImplementedException();
        }
    }
}
