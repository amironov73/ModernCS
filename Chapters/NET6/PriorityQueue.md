### PriorityQueue

`PriorityQueue<TElement, TPriority>` (`System.Collections.Generic`) is a new collection that enables adding new items with a value and a priority. On dequeue the PriorityQueue returns the element with the lowest priority value. You can think of this new collection as similar to Queue<T> but that each enqueued element has a priority value that affects the behavior of dequeue.

The following sample demonstrates the behavior of `PriorityQueue<string, int>`.

```c#
// creates a priority queue of strings with integer priorities
var pq = new PriorityQueue<string, int>();

// enqueue elements with associated priorities
pq.Enqueue("A", 3);
pq.Enqueue("B", 1);
pq.Enqueue("C", 2);
pq.Enqueue("D", 3);

pq.Dequeue(); // returns "B"
pq.Dequeue(); // returns "C"
pq.Dequeue(); // either "A" or "D", stability is not guaranteed.
```

Credit to community member Patryk Golebiowski for contributing the implementation.
