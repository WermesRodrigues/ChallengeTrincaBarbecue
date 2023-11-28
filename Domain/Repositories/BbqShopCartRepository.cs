using Domain.Entities;
using System;

namespace Domain.Repositories
{
    internal class BbqShopCartRepository : StreamRepository<BbqShopCart>, IBbqShopCartRepository
    {
        public BbqShopCartRepository(IEventStore<BbqShopCart> eventStore) : base(eventStore) { }
    }
}
