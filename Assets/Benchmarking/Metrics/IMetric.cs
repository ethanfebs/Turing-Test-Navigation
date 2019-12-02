using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IMetric<T, U>
{
    protected Func<U, U, U> accumulator;
    protected U total;
    protected int totalCount = 0;

    protected bool hasNext = false;
    protected U next;

    public IMetric(Func<U, U, U> a, U i)
    {
        accumulator = a;
        total = i;
    }

    public abstract bool Update(T data);

    public bool HasNext()
    {
        return hasNext;
    }

    public U Next()
    {
        return next;
    }

    public U GetTotal()
    {
        return total;
    }

    public int GetTotalCount()
    {
        return totalCount == 0 ? 1 : totalCount;
    }

}