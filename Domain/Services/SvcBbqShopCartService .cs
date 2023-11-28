using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Services.Interfaces;
using Eveneum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class SvcBbqShopCartService : ISvcBbqShopCartService
    {
        private readonly IBbqShopCartRepository _bbqShopCartRepository;
        public async Task<BbqShopCart> GetAsync(string id) => await _bbqShopCartRepository.GetAsync(id);

        public SvcBbqShopCartService(IBbqShopCartRepository bbqShopCartRepository)
        {
            _bbqShopCartRepository = bbqShopCartRepository;
        }

        public async Task<BbqEventsResponses> CreateNewShopCartBarbecue(string bbqId)
        {
            try
            {
                #region create new Cart for the barbecue                
                var churrasCart = new BbqShopCart();

                churrasCart.Apply(new NewBbqShopCart(bbqId, 0, 0));

                await _bbqShopCartRepository.SaveAsync(churrasCart);
                #endregion create new Cart for the barbecue                   

                return new BbqEventsResponses(string.Format("Hi, your Shop Cart Barbecue has been created with Id {0}", bbqId), true, churrasCart.TakeSnapshot());
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to create a Shop cart Barbecue " + ex.Message, false);
            }
        }

        public async Task<BbqEventsResponses> GetBarbecueCartShopCart(string bbqId)
        {
            try
            {
                var bbqShopCart = await _bbqShopCartRepository.GetAsync(bbqId);

                if (bbqShopCart == null)
                    return new BbqEventsResponses(string.Format("Shop Cart Barbecue not found Id {0}.", bbqId), false);

                return new BbqEventsResponses("Successfully fetched.", true, bbqShopCart);
            }
            catch (Exception ex)
            {
                //Improvements  Later
                //We can create log on table and create automatically ticket on DevOps with input and output...

                return new BbqEventsResponses("Somenthing is wrong to get a Barbecue Shop Cart " + ex.Message, false);
            }
        }
    }
}
