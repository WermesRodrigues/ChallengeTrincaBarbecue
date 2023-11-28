using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class BbqShopCart
    {
        public BbqShopCart(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public double VeganFoodQuantity { get; set; }
        public double MeatFoodQuantity { get; set; }

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
