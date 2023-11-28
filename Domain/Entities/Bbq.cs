using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Events;

namespace Domain.Entities
{
    public class Bbq : AggregateRoot
    {
        public string? Reason { get; set; }
        public BbqStatus Status { get; set; }
        public DateTime Date { get; set; }
        public bool IsTrincasPaying { get; set; }
        private int _totalAccepted => InvitationsAccepted.Count;
        public BbqShopCart? BbqShopCart { get; set; }
        public List<string> InvitationsAccepted { get; set; } = new List<string>();

        public void When(ThereIsSomeoneElseInTheMood @event)
        {
            Id = @event.Id.ToString();
            Date = @event.Date;
            Reason = @event.Reason;
            Status = BbqStatus.New;
        }


        internal void When(BbqStatusUpdated @event)
        {
            if (@event.GonnaHappen)
                Status = BbqStatus.PendingConfirmations;
            else
                Status = BbqStatus.ItsNotGonnaHappen;

            if (@event.TrincaWillPay)
                IsTrincasPaying = true;
        }


        internal void When(InviteWasAccepted @event)
        {
            //check if is new 
            if (BbqShopCart == null)
                BbqShopCart = new BbqShopCart(@event.InviteId);
            
            ///check if the person not accepet to not duplicate....
            if (!InvitationsAccepted.Any(w => w.Equals(@event.PersonId)))
            {
                BbqShopCart.IncreaseQuantitiesByFoodType(@event.IsVeg);

                InvitationsAccepted.Add(@event.PersonId);
            }

            //update status like Confirmed
            if (_totalAccepted >= 7)
                Status = BbqStatus.Confirmed;
        }

        internal void When(InviteWasDeclined @event)
        {
            //TODO:Deve ser possível rejeitar um convite já aceito antes.
            //Se este for o caso, a quantidade de comida calculada pelo aceite anterior do convite
            //deve ser retirado da lista de compras do churrasco.
            //Se ao rejeitar, o número de pessoas confirmadas no churrasco for menor que sete,
            //o churrasco deverá ter seu status atualizado para “Pendente de confirmações”. 

            //check if is new 
            if (BbqShopCart == null)
                BbqShopCart = new BbqShopCart(@event.InviteId);

            //check if the person was accepted to decline....
            if (InvitationsAccepted.Any(w => w.Equals(@event.PersonId)))
            {
                InvitationsAccepted.Remove(@event.PersonId);

                //decrease food....
                BbqShopCart.DecreaseQuantitiesByFoodType(@event.IsVeg);
            }

            if (_totalAccepted < 7)
                Status = BbqStatus.PendingConfirmations;
        }

        public object TakeSnapshot()
        {
            return new
            {
                Id,
                Date,
                IsTrincasPaying,
                Status = Status.ToString()
            };
        }
    }

    public enum BarbecueInvitationType
    {
        Accept = 0,
        Decline = 1,
        Peding = 2,
    }
}
