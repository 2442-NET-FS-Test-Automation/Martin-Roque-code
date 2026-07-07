## Domain: 
#### Videogame Store.

## Endpoint Map
#### /inventory - Show all inventory items
#### /videogames - Show all videogames stored (to now what I am selling)
#### /inventory/seed - Restock inventory
#### /buyings/burst - Calling a burst of buyings
#### /benchmark - comparing sequential with concurrent
#### /reports/by-fulfillment - how many buyings were fulfilled
#### /reports/top-videogames - how many videogames are in buyinglines
#### /reports/top-customers - which customer bought more games
#### /verify/no-oversell - confirm the program do not oversell any videogame in the buyings

## Big-O
#### The algorithm for priority queue is O(n log n) because both of the parts increase in that way with the buying's total.

## Optimistic concurrency vs Lock
#### In this case concurrency give security about execute code without problems but with little inconsisty
#### vs Lock it is 100% consistent but can lead to starve or deadlocks easier.

## ACID
#### We use ACID's principles to make sure the data will be consistent and avoid phantom and dirty lectures for the system.

## non-key index used:
#### SpeIden: Identify easier a Videogame.
#### Status and Priority (Buying): easier to filter in endpoints.

## Benchmark time
#### Sequential:
#### Concurrency:
#### Conclusion: Concurrency is better when more and more orders are comming

## Status codes
#### 200: to notify user about it action was done
#### 204: to nofity creation of an object
#### 400: Something go wrong during execution
#### 404: wrong Url
#### 500: server is down