using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.OrderBook
{
    public interface IMatchingOrderBook : IRetrievalOrderBook
    {
        MatchResult Match();
    }
}
