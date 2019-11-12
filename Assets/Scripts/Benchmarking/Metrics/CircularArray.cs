using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularArray<V>
{
    private int startIndex = 0;
    private int count = 0;

    public V[] array;

    public CircularArray(int size)
    {
        array = new V[size];
    }

    public void Add(V value)
    {
        array[(startIndex + count) % array.Length] = value;

        count++;
        if (count > array.Length)
        {
            count--;
            startIndex = (startIndex + 1) % array.Length;
        }
    }

    public V GetAt(int normalizedIndex)
    {
        return array[(startIndex + normalizedIndex) % array.Length];
    }

    public bool IsFull()
    {
        return count == array.Length;
    }

    public bool IsRefreshed()
    {
        return startIndex == 0 && IsFull();
    }

    public bool IsEmpty()
    {
        return count == 0;
    }

    public void Clear()
    {
        startIndex = 0;
        count = 0;
    }
}