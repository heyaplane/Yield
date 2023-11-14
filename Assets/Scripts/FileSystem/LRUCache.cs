using System.Collections.Generic;
using UnityEngine;

public class LRUCache
{
    readonly int capacity;
    Dictionary<string, LRUNode> cache;
    LRUNode head, tail;

    public LRUCache(int capacity)
    {
        this.capacity = capacity;
        cache = new Dictionary<string, LRUNode>();
        head = new LRUNode();
        tail = new LRUNode();
        head.Next = tail;
        tail.Prev = head;
    }

    void AddNode(LRUNode node)
    {
        node.Prev = head;
        node.Next = head.Next;

        head.Next.Prev = node;
        head.Next = node;
    }

    void RemoveNode(LRUNode node)
    {
        var prev = node.Prev;
        var next = node.Next;

        prev.Next = next;
        next.Prev = prev;

        if (node.Value != null)
        {
            Object.Destroy(node.Value as Texture2D);
            node.Value = null;
        }
    }

    void MoveToHead(LRUNode node)
    {
        RemoveNode(node);
        AddNode(node);
    }

    public bool TryGet<T>(string key, out T value) where T : class
    {
        if (cache.TryGetValue(key, out var node))
        {
            MoveToHead(node);
            value = node as T;
            return true;
        }

        value = null;
        return false;
    }

    public void Put(string key, object value)
    {
        if (!cache.TryGetValue(key, out var node))
        {
            node = new LRUNode {Key = key, Value = value};
            cache.Add(key, node);
            AddNode(node);

            if (cache.Count > capacity)
            {
                var newTail = tail.Prev;
                RemoveNode(newTail);
                cache.Remove(newTail.Key);
            }
        }

        else
        {
            node.Value = value;
            MoveToHead(node);
        }
    }
}
