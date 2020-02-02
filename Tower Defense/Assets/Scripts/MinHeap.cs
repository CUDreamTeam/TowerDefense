using System.Collections.Generic;
using System;
using UnityEngine;

public class MinHeap<T> where T : IComparable<T>
{
    int capacity = 1;
    public int size = 0;
    T[] heap;

    public MinHeap()
    {
        heap = new T[capacity];
    }

    public MinHeap(List<T> lst)
    {
        heap = new T[lst.Count];
        addItems(lst);
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= size)
            {
                throw new IndexOutOfRangeException("Minheap index: " + index);
            }
            if (index == 0)
            {
                T item = heap[0];
                heap[0] = heap[size - 1];
                heapifyDown();
                size--;
                return item;
            }
            else
            {
                return heap[index];
            }
        }
    }

    public void addItems(List<T> items)
    {
        foreach (T t in items)
        {
            addItem(t);
        }
    }

    /// <summary>
    /// Returns the first object and removes it from the heap
    /// </summary>
    /// <returns></returns>
    public T getFront()
    {
        T item = heap[0];
        heap[0] = heap[size - 1];
        heapifyDown();
        size--;
        return item;
    }

    public void addItem(T item)
    {
        if (item == null)
            Debug.Log("Null item");
        resize();
        heap[size] = item;
        size++;
        heapifyUp();
    }

    int getLeftChildIndex(int pIndex)
    {
        return (2 * pIndex) + 1;
    }

    int getRightChildIndex(int pIndex)
    {
        return (2 * pIndex) + 2;
    }

    int getParentIndex(int cIndex)
    {
        return (cIndex - 1) / 2;
    }

    bool hasLeftChild(int pIndex)
    {
        if (getLeftChildIndex(pIndex) < size)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool hasRightChild(int pIndex)
    {
        if (getRightChildIndex(pIndex) < size)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool hasParent(int cIndex)
    {
        if (cIndex == 0)
        {
            return false;
        }
        if (getParentIndex(cIndex) < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    T getLeftChild(int pIndex)
    {
        return heap[getLeftChildIndex(pIndex)];
    }

    T getRightChild(int pIndex)
    {
        return heap[getRightChildIndex(pIndex)];
    }

    T getParent(int cIndex)
    {
        return heap[getParentIndex(cIndex)];
    }

    void swap(int indexA, int indexB)
    {
        T temp = heap[indexA];
        heap[indexA] = heap[indexB];
        heap[indexB] = temp;
    }

    void resize()
    {
        if (size == capacity)
        {
            T[] temp = new T[capacity * 2];
            for (int i = 0; i < capacity; ++i)
            {
                temp[i] = heap[i];
            }
            heap = temp;
            capacity *= 2;
        }
    }

    void heapifyUp()
    {
        int ind = size - 1;
        while (hasParent(ind) && getParent(ind).CompareTo(heap[ind]) > 0)
        {
            swap(getParentIndex(ind), ind);
            ind = getParentIndex(ind);
        }
    }

    void heapifyDown()
    {
        int ind = 0;
        while (hasLeftChild(ind))
        {
            int smallerChildIndex = getLeftChildIndex(ind);
            if (hasRightChild(ind) && getRightChild(ind).CompareTo(getLeftChild(ind)) < 0)
            {
                smallerChildIndex = getRightChildIndex(ind);
            }
            else if (hasRightChild(ind) && getRightChild(ind).CompareTo(getLeftChild(ind)) == 0)
            {
                smallerChildIndex = getRightChildIndex(ind);
            }

            if (heap[ind].CompareTo(heap[smallerChildIndex]) < 0)
            {
                break;
            }
            else
            {
                swap(ind, smallerChildIndex);
            }
            ind = smallerChildIndex;
        }
    }

    public int indexOf(T item)
    {
        for (int i = 0; i < size; i++)
        {
            if (heap[i].Equals(item))
            {
                return i;
            }
        }
        return -1;
    }

    public T[] items()
    {
        T[] it = new T[size];
        for (int i = 0; i < size; i++)
        {
            it[i] = heap[i];
        }
        return it;
    }

    //Designed to be used with the unit selection
    public List<T> getAll(MinHeap<T> minHeap, List<T> all)
    {
        if (minHeap.size > 0)
        {
            all.Add(minHeap.getFront());
            return getAll(minHeap, all);
        }
        else
        {
            return all;
        }
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < size; i++)
        {
            if (heap[i].Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool UpdateItem(T item)
    {
        foreach (T it in heap)
        {
            if (it.Equals(item))
            {
                return true;
            }
        }
        return false;
    }
}
