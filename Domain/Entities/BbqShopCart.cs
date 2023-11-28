using Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class BbqShopCart : AggregateRoot
    {
        public double VeganFoodQuantity { get; set; }
        public double MeatFoodQuantity { get; set; }

        public void When(NewBbqShopCart @event)
        {
            Id = @event.Id.ToString();
            VeganFoodQuantity = 0;
            MeatFoodQuantity = 0;
        }

        public void ResetshopCart()
        {
            VeganFoodQuantity = 0;
            MeatFoodQuantity = 0;
        }


        public void DecreaseQuantitiesByFoodType(bool isVeg)
        {
            if (isVeg)
            {
                VeganFoodQuantity -= 600;
            }
            else
            {
                MeatFoodQuantity -= 300;
                VeganFoodQuantity -= 300;
            }

            if (VeganFoodQuantity < 0)
                VeganFoodQuantity = 0;

            if (MeatFoodQuantity < 0)
                MeatFoodQuantity = 0;
        }

        public void IncreaseQuantitiesByFoodType(bool isVeg)
        {
            if (isVeg)
            {
                VeganFoodQuantity += 600;
            }
            else
            {
                MeatFoodQuantity += 300;
                VeganFoodQuantity += 300;
            }
        }

        public object? TakeSnapshot()
        {
            return new
            {
                Id,
                VeganFoodQuantity = VeganFoodQuantity / 1000,
                MeatFoodQuantity = MeatFoodQuantity / 1000,
            };
        }
    }
}
