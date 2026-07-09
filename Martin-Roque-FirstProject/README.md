# Videogames Store API (Domain)

> A REST API that simulates a videogames store with inventory management, buying processing, concurrency testing, benchmarking, and reporting.

---

#  Endpoint Map

| Endpoint | Description |
|----------|-------------|
| `GET /inventory` | Show all inventory items |
| `GET /videogames` | Show all videogames stored (available for sale) |
| `GET /customers` | Show all registered customers |
| `POST /inventory/seed` | Restock the inventory |
| `POST /buyings/burst` | Execute a burst of buying requests |
| `GET /benchmark` | Compare sequential and concurrent execution |
| `GET /reports/by-fulfillment` | Show fulfilled buying statistics |
| `GET /reports/videogames` | Show videogames included in buying lines |
| `GET /reports/customers` | Show the customers who purchased the most games |
| `GET /verify/no-oversell` | Verify that no videogame was oversold |

---

# Algorithm Complexity

## Priority Queue

```text
Time Complexity: O(n log n)
```

The priority queue processes all buying requests by inserting and removing elements. Since both operations are logarithmic, the overall complexity is:

> **O(n log n)**

---

#  Concurrency Strategy

## Optimistic Concurrency

**Advantages**

- Better throughput
- Allows multiple operations simultaneously
- Reduces blocking

**Disadvantages**

- May produce retries due to conflicts
- Slightly less consistent during heavy contention

---

## Lock-Based Synchronization

**Advantages**

- Guarantees full consistency
- Prevents concurrent modifications

**Disadvantages**

- Higher contention
- Can lead to starvation
- Possibility of deadlocks

---

# ACID Principles

The application follows the **ACID** principles to ensure data integrity.

-  Atomicity
-  Consistency
-  Isolation
-  Durability

This helps prevent problems such as:

- Dirty reads
- Phantom reads
- Inconsistent transactions

---

# Database Indexes

The following non-key indexes are used to improve query performance.

| Index | Purpose |
|-------|---------|
| `SpecIden` | Quickly identify a videogame |
| `Status` | Speed up buying status filtering |
| `Priority` | Optimize priority-based buying queries |

---

# Benchmark Results

| Orders | Sequential (ms) | Concurrent (ms) |
|--------:|----------------:|----------------:|
| 300 | 16,922 | 22,268 |
| 1000 | 33,137 | 105,206 |

### Conclusion

> Concurrency becomes more beneficial as the number of requests increases, provided contention remains low and conflicts are minimized.

---

# HTTP Status Codes

| Code | Meaning |
|------|---------|
|  `200 OK` | Request completed successfully |
|  `204 No Content` | Resource created or processed successfully without returning content |
|  `400 Bad Request` | Invalid request or input |
|  `404 Not Found` | Requested resource does not exist |
|  `500 Internal Server Error` | Unexpected server error |

---