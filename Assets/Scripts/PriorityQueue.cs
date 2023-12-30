using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<KeyValuePair<int, T>> elements = new List<KeyValuePair<int, T>>();

    public void Enqueue(T item, int priority)
    {
        elements.Add(new KeyValuePair<int, T>(priority, item));
        elements.Sort((x, y) => x.Key.CompareTo(y.Key));
    }

    public T Dequeue()
    {
        var item = elements[0].Value;
        elements.RemoveAt(0);
        return item;
    }

    public int Count
    {
        get { return elements.Count; }
    }
};