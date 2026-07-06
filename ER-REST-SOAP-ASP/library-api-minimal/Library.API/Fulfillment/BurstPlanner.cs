using Library.Data.Entities;
using Microsoft.AspNetCore.Components.Web;

namespace Library.API.Fulfillment;

public class BurstPlanner
{
    //Method to plan fulfillment order
    public IReadOnlyList<int> OrderByPriority(IEnumerable<Order> orders)
    {
        //we could make our own custom implementation on this - we won't
        //In this case we can use a PriorityQueue - allows for FIFO processing with priority taken into account
        //First int = orderId, Second int = priority
        //lower number = higher priority in this case.
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();

        foreach (Order o in orders)
        {
            pq.Enqueue(o.Id, o.Priority == Priority.Expedited ? 0 : 1);
        }

        var orderedByPriority = new List<int>();

        while (pq.TryDequeue(out int id, out _))
        {
            orderedByPriority.Add(id);

        }

        return orderedByPriority;
    }
}