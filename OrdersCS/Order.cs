using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    public class Order : IOrderCore
    {
        public Order(IOrderCore orderCore, long price, uint quantity, bool isBuySide ) 
        {
            // PROPERTIES
            Price = price;
            IsBuySide = isBuySide;
            InitialQuantity = quantity;
            CurrentQuantity = quantity;

            // FIELDS
            _orderCore = orderCore;

        }

        public Order(ModifyOrder modifyOrder) : 
            this(modifyOrder, modifyOrder.Price, modifyOrder.Quanity, modifyOrder.IsBuySide)
        {

        }

        public long Price { get; private set; }
        public uint InitialQuantity { get; private set; }
        public uint CurrentQuantity { get; private set; }
        public bool IsBuySide { get; private set; }

        //METHODS 

        public void IncreaseQuantity(uint quatityDelta)
        {
            CurrentQuantity += quatityDelta;
        }

        public void DecreaseQuantity(uint quatityDelta)
        {
            if (quatityDelta > CurrentQuantity)
                throw new InvalidOperationException($"Quantity Delta > CurrentQuantity Quanity for OrderId ={OrderId}");
            CurrentQuantity -= quatityDelta;
        }
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;

        private readonly IOrderCore _orderCore;
    }
}
