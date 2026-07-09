using VideoGameStore.Data.Entities;

namespace VideoGameStore.API.Fulfillment;

public class BurstPlanner
{
    public IReadOnlyList<int> BuyingByPriority(IEnumerable<Buying> buyings)
    {
        //Creating a queue to store priorities buyings
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();

        foreach (Buying b in buyings)
            pq.Enqueue(b.Id, b.Priority == Priority.Expedited ? 0 : 1);

        var orderedByPriority = new List<int>();

        //Returnig a queue with correct priority
        while (pq.TryDequeue(out int id, out _))
        {
            orderedByPriority.Add(id);
        }

        return orderedByPriority;
    }
}