using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAvg
{
    private CircularArray<float> buffer;
    private int size;
    private float avg;

    public MovingAvg(int s)
    {
        size = s;
        buffer = new CircularArray<float>(s);
    }

    public void Add(float val)
    {
        if (buffer.IsFull())
        {
            avg -= buffer.GetAt(0);
        }

        var newVal = val / size;
        buffer.Add(newVal);
        avg += newVal;
    }

    public float GetAvg()
    {
        return avg;
    }

    public bool IsFull()
    {
        return buffer.IsFull();
    }
}
