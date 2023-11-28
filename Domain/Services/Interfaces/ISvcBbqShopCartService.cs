using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface ISvcBbqShopCartService : ISvcBaseService<BbqShopCart>    
    {
        Task<BbqEventsResponses> CreateNewShopCartBarbecue(string bbqId);
        Task<BbqEventsResponses> GetBarbecueCartShopCart(string bbqId);
    }
}
