using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.OrderBook
{
    public interface IRetrievalOrderBook : IOrderEntryOrderBook
    {
        List<OrderBookEntry> GetAskOrders();
        List<OrderBookEntry> GetBidOrders();
    }
}
