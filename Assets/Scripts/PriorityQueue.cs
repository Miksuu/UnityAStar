using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    // List of elements in the priority queue, each element is a key-value pair where key is the priority and value is the item
    private List<KeyValuePair<int, T>> elements = new List<KeyValuePair<int, T>>();

    // Method to add an item to the priority queue with a given priority
    public void Enqueue(T item, int priority)
    {
        elements.Add(new KeyValuePair<int, T>(priority, item));
        elements.Sort((x, y) => x.Key.CompareTo(y.Key));
    }

    // Method to remove and return the item with the highest priority (lowest priority number) from the priority queue
    public T Dequeue()
    {
        var item = elements[0].Value;
        elements.RemoveAt(0);
        return item;
    }

    // Property to get the number of elements in the priority queue
    public int Count
    {
        get { return elements.Count; }
    }
};