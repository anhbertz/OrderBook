using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingEngineServer.Orders;
using TradingEngingeServer.Instrument;

namespace TradingEngineServer.OrderBook
{
    public class OrderBook : IRetrievalOrderBook
    {



        // PRIVATE FIELDS 
        private readonly Security _instrument;
        private readonly Dictionary<long, OrderBookEntry> _orders = new Dictionary<long, OrderBookEntry>();
        private readonly SortedSet<Limit> _askLimits = new SortedSet<Limit>(AskLimitComparer.Comparer);
        private readonly SortedSet<Limit> _bidLimits = new SortedSet<Limit>(BidLimitComparer.Comparer);

        public OrderBook(Security instrument) { }
        public int Count => _orders.Count;

        public void AddOrder(Order order)
        {
            var baseLimit = new Limit(order.Price);
            AddOrder(order, baseLimit, order.IsBuySide ? _bidLimits : _askLimits, _orders);
        }

        private static void AddOrder(Order order, Limit baseLimit, 
            SortedSet<Limit> limitLevels, Dictionary<long, OrderBookEntry> interalBook)
        {
            if(limitLevels.TryGetValue(baseLimit, out Limit limit))
            {
                OrderBookEntry orderBookEntry = new OrderBookEntry(order, baseLimit);
                if(limit.Head == null)
                {
                    limit.Head = orderBookEntry;
                    limit.Tail = orderBookEntry;
                }
                else
                {
                    OrderBookEntry tailPointer = limit.Tail;
                    tailPointer.Next = orderBookEntry;
                    orderBookEntry.Previous = tailPointer;
                    limit.Tail = orderBookEntry;
                }
                interalBook.Add(order.OrderId, orderBookEntry);
            }
            else
            {
                limitLevels.Add(baseLimit);
                OrderBookEntry orderBookEntry = new OrderBookEntry(order, baseLimit);
                baseLimit.Head = orderBookEntry;
                baseLimit.Tail = orderBookEntry;
                interalBook.Add(order.OrderId, orderBookEntry);
            }
        }

        public void ChangeOrder(ModifyOrder modifyOrder)
        {
            if(_orders.TryGetValue(modifyOrder.OrderId, out OrderBookEntry obe)) 
            {
                RemoveOrder(modifyOrder.ToCancelOrder());
                AddOrder(modifyOrder.ToNewOrder(), obe.ParentLimit, modifyOrder.IsBuySide ? _bidLimits : _askLimits, _orders);
            }
        }

        public bool ContainsOrder(long orderId)
        {
            return _orders.ContainsKey(orderId);
        }

        public List<OrderBookEntry> GetAskOrders()
        {
            List<OrderBookEntry> orderBookEntires = new List<OrderBookEntry>();
            foreach(var askLimit in _askLimits)
            {
                if(askLimit.IsEmpty)
                    continue;
                else
                {
                    OrderBookEntry askLimitPointer = askLimit.Head;
                    while(askLimitPointer != null)
                    {
                        orderBookEntires.Add(askLimitPointer);
                        askLimitPointer = askLimitPointer.Next;
                    }
                }
            }
            return orderBookEntires;
        }

        public List<OrderBookEntry> GetBidOrders()
        {
            List<OrderBookEntry> orderBookEntires = new List<OrderBookEntry>();
            foreach (var bidLimit in _bidLimits)
            {
                if (bidLimit.IsEmpty)
                    continue;
                else
                {
                    OrderBookEntry bidLimitPointer = bidLimit.Head;
                    while (bidLimitPointer != null)
                    {
                        orderBookEntires.Add(bidLimitPointer);
                        bidLimitPointer = bidLimitPointer.Next;
                    }
                }
            }
            return orderBookEntires;
        }

        public OrderBookSpread GetSpread()
        {
            long? bestAsk = null, bestBid = null;
            if (_askLimits.Count != 0 && _askLimits.Min.IsEmpty)
                bestAsk = _askLimits.Min.Price;
            if(_bidLimits.Count != 0 && _bidLimits.Max.IsEmpty)
                bestBid = _bidLimits.Max.Price;
            return new OrderBookSpread(bestBid, bestAsk);
        }

        public void RemoveOrder(CancelOrder cancelOrder)
        {
            if(_orders.TryGetValue(cancelOrder.OrderId, out var obe))
            {
                RemoveOrder(cancelOrder.OrderId, obe, _orders);
            }
        }

        private static void RemoveOrder(long orderId, OrderBookEntry obe, Dictionary<long, OrderBookEntry> internalBook)
        {
            // Location of OrderBookEntry within the Linked List
            if(obe.Previous != null && obe.Next != null)
            {
                obe.Next.Previous = obe.Previous;
                obe.Previous.Next = obe.Next;
            }
            else if(obe.Previous != null)
            {
                obe.Previous.Next = null;
            }
            else if(obe.Next != null)
            {
                obe.Next.Previous = null;
            }

            // OrderBookEntry on limit levels
            if(obe.ParentLimit.Head == obe && obe.ParentLimit.Tail == obe)
            {
                // Only one order on this level
                obe.ParentLimit.Head = null;
                obe.ParentLimit.Tail = null;
            }
            else if(obe.ParentLimit.Head == obe)
            {
                // More than one order, but obe is the first order on level
                obe.ParentLimit.Head = obe.Next;
            }
            else if(obe.ParentLimit.Tail == obe)
            {
                // More than one order, but obe is the last order on level
                obe.ParentLimit.Tail = obe.Previous;
            }

            internalBook.Remove(orderId);
        }
    }
}
