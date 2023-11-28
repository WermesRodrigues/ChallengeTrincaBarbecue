using Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class BbqShopCart : AggregateRoot
    {
        
        public string VeganFoodQuantityInKG
        {
            get
            {
                return $"{VeganFoodQuantity / 1000} KG";
            }
        }

        public string MeatFoodQuantityInKG
        {
            get
            {
                return $"{MeatFoodQuantity / 1000} KG";
            }
        }

        public double VeganFoodQuantity { get; set; }
        public double MeatFoodQuantity { get; set; }

        public void When(NewBbqShopCart @event)
        {
            Id = @event.Id.ToString();
            VeganFoodQuantity = 0;
            MeatFoodQuantity = 0;
        }

        public void ResetShopCart()
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
