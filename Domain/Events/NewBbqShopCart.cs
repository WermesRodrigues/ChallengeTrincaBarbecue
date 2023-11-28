using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Events
{
    public class NewBbqShopCart : IEvent
    {
        public NewBbqShopCart(string id, double veganFoodQuantity, double meatFoodQuantity)
        {
            Id = id;
            VeganFoodQuantity = veganFoodQuantity;
            MeatFoodQuantity = meatFoodQuantity;
        }

        public string Id { get; set; }
        public double VeganFoodQuantity { get; set; }
        public double MeatFoodQuantity { get; set; }
    }
}
