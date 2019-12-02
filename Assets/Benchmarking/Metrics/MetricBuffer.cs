using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// MetricBuffer buffers values of type T in a circular array. When the buffer is full, Func p processes it and outputs type U. Func a then accumulates the output into a total, which takes an initial value of i.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="U"></typeparam>
public class MetricBuffer<T, U> : IMetric<T, U>
{
    private CircularArray<T> buffer;
    private Func<CircularArray<T>, U> processor;

    public MetricBuffer(int bufferSize, Func<CircularArray<T>, U> p, Func<U, U, U> a, U i) : base(a, i)
    {
        buffer = new CircularArray<T>(bufferSize);
        processor = p;
    }
    
    public override bool Update(T data)
    {
        buffer.Add(data);

        hasNext = buffer.IsFull();
        if (hasNext)
        {
            //dynamic temp = processor.Invoke(buffer);
            //total = (U)(total + temp);
            var temp = processor.Invoke(buffer);
            total = accumulator.Invoke(total, temp);

            totalCount++;
            next = temp;
        }

        return hasNext;
    }
    
}

