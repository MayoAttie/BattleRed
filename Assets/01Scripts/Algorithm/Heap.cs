using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem <T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapInedx = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapInedx = 0;

        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapInedx], item);
    }

    public int Count
    {
        get { return currentItemCount; }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    void SortDown(T item)
    {
        while(true)
        {
            int child_indexLeft = item.HeapInedx * 2 + 1;
            int child_IndexRight = item.HeapInedx * 2 + 2;
            int swapIndex = 0;

            if (child_indexLeft < currentItemCount)
            {
                swapIndex = child_indexLeft;

                if (child_IndexRight < currentItemCount)
                {
                    if (items[child_indexLeft].CompareTo(items[child_IndexRight]) < 0)
                    {
                        swapIndex = child_IndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
                return;

        }
    }

    void SortUp(T item)
    {
        int parentIndx = (item.HeapInedx - 1) / 2;
        while(true)
        {
            T parentItem = items[parentIndx];
            if(item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndx = (item.HeapInedx - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapInedx] = itemB;
        items[itemB.HeapInedx] = itemA;

        int itemA_index = itemA.HeapInedx;
        itemA.HeapInedx = itemB.HeapInedx;
        itemB.HeapInedx = itemA_index;
    }
}


public interface IHeapItem<T> : IComparable<T>
{
    int HeapInedx
    {
        get;
        set;
    }
}