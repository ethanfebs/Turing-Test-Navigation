using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This class is a MetricBuffer with buffer size = 1, i.e., a single-element queue (SEQ). The only difference is Func p, which processes a single value instead of a buffer.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="U"></typeparam>
public class MetricSEQ<T, U> : IMetric<T, U>
{
    private Func<T, U> processor;
    
    public MetricSEQ(Func<T, U> p, Func<U, U, U> a, U i) : base(a, i)
    {
        processor = p;
    }

    public override bool Update(T data)
    {
        var temp = processor.Invoke(data);
        total = accumulator.Invoke(total, temp);

        totalCount++;
        next = temp;

        return true;
    }
    
}
