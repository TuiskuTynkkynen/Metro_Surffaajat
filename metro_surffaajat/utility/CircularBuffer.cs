using System;
using System.Collections;
using System.Collections.Generic;

namespace Metro_Surffaajat.utility;

/// @author Tuisku Tynkkynen
/// @version 16.10.2025
/// <summary>
/// 
/// </summary>
/// <param name="capacity"></param>
/// <typeparam name="T"></typeparam>
public class CircularBuffer<T>(int capacity) : 
   IEnumerable<T>
{
    /// <summary>
    /// Number of elements internal buffer can hold.
    /// </summary>
    public readonly int Capacity = capacity;
    /// <summary>
    /// Current number of elements.
    /// </summary>
    public int Count { get; private set; }
    /// <summary>
    /// Hold maximum number of elements.
    /// </summary>
    public bool IsFull => Count == Capacity;
    /// <summary>
    /// Holds no elements.
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Get or set element at specified index.
    /// </summary>
    /// <param name="index">Index of the element</param>
    /// <exception cref="ArgumentOutOfRangeException">Specified index must be less than Count</exception>
    public T this[uint index] { 
        get => _data[CalculateIndex(index)];
        set => _data[CalculateIndex(index)] = value;
    }
    /// <summary>
    /// Get or set element at specified index.
    /// </summary>
    /// <param name="index">Index of the element</param>
    /// <exception cref="ArgumentOutOfRangeException">Specified index must be greater than 0 and less than Count</exception>
    public T this[int index] {
        get => index < 0 ? throw new ArgumentOutOfRangeException(nameof(index), index, "must be greater than 0") : _data[CalculateIndex((uint)index)];
        set
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), index, "must be greater than 0");
            _data[CalculateIndex((uint)index)] = value;
        } 
    }

    /// <summary>
    /// Internal buffer.
    /// </summary>
    private readonly T[] _data = new T[capacity];
    /// <summary>
    /// Index of the most recently added element. -1, if IsEmpty.
    /// </summary>
    private int _head = -1;

    
    /// <summary>
    /// Adds element to the end of the buffer. If IsFull, overwrites earliest element.
    /// </summary>
    /// <param name="value">Value to be added</param>
    public void Add(T value)
    {
        if (++_head >= Capacity)
        {
            _head = 0;
        }
        
        if(!IsFull)
        {
            Count++;
        }

        _data[_head] = value;
    }

    
    /// <summary>
    /// Get reference to most recently added element.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Buffer must not be empty</exception>
    public ref T Back()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException();
        }
        
        return ref _data[_head];
    }
    

    /// <summary>
    /// Clears the internal buffer.
    /// </summary>
    public void Clear()
    {
        _data.Initialize();
        _head = -1;
        Count = 0;
    }


    /// <summary>
    /// Get enumerator that iterates through internal buffer.
    /// </summary>
    /// <returns>Enumerator that iterates over buffer</returns>
    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }
    
    
    /// <summary>
    /// Get enumerator that iterates through internal buffer.
    /// </summary>
    /// <returns>Enumerator that iterates over buffer</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    /// <summary>
    /// Removes most recent element from buffer.
    /// </summary>
    public void Remove()
    {
        if (IsEmpty)
        {
            return;
        }

        Count--;
        if (--_head < 0)
        {
            _head = Capacity - 1;
        }
    }

    
    /// <summary>
    /// Calculates the position of specified index in internal buffer.
    /// </summary>
    /// <param name="index">Index of the element inside CircularBuffer</param>
    /// <returns>Index of the element inside internal buffer</returns>
    /// <exception cref="ArgumentOutOfRangeException">Specified index must be less than Count</exception>
    private int CalculateIndex(uint index)
    {
        if (index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"was not valid index of CircularBuffer<{nameof(T)}> with Count {Count}");
        }
        
        int tail = _head - Count + 1;
        int internalIndex = tail + (int)index;
        
        // Could be calculated with modulus, but this should be faster  
        internalIndex += (internalIndex < 0) ? Capacity : 0;
        internalIndex -= (internalIndex >= Capacity) ? Capacity : 0;
        
        return internalIndex;
    }
}