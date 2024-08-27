using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    public class ModifyOrder : IOrderCore
    {

        public ModifyOrder(IOrderCore orderCore, 
            long modifyPrice, uint modifyQuanity, bool isBuySide)
        {
            _orderCore = orderCore;
            Price = modifyPrice;
            Quanity = modifyQuanity;
            IsBuySide = isBuySide;
        }
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;

        // METHODS
        public CancelOrder ToCancelOrder()
        {
            return new CancelOrder(this);
        }

        public Order ToNewOrder()
        {
            return new Order(this);
        }

        public long Price { get; private set; }
        public uint Quanity { get; private set; }
        public bool IsBuySide { get; private set; }

        private readonly IOrderCore _orderCore;
    }
}
