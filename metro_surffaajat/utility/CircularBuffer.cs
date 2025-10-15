using System;
using System.Collections;
using System.Collections.Generic;

namespace Metro_Surffaajat.utility;

/// @author Tuisku Tynkkynen
/// @version 15.10.2025
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
    public bool IsFull => Count != Capacity;
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
    /// Adds element to buffer. If IsFull, overwrites earliest element.
    /// </summary>
    /// <param name="value">Value to be added</param>
    public void Add(T value)
    {
        if (++_head >= Capacity)
        {
            _head = 0;
        }
        
        _data[_head] = value;
        Count++;
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
    /// <returns>Enumerator</returns>
    public IEnumerator GetEnumerator()
    {
        return new CircularEnumerator(this);
    }
    
    
    /// <summary>
    /// Get enumerator that iterates through internal buffer.
    /// </summary>
    /// <returns>Enumerator</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return new CircularEnumerator(this);
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
    private uint CalculateIndex(uint index)
    {
        if (index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"was not valid index of CircularBuffer<{nameof(T)}> with Count {Count}");
        }
        
        // Could be calculated with modulus, but this should be faster  
        index += (uint)_head;
        if (index >= Capacity)
        {
            index -= (uint)Capacity;
        }
        
        return index;
    }
    
    
    /// @author Tuisku Tynkkynen
    /// @version 15.10.2025
    /// <summary>
    /// Enumerator for iterating over CircularBuffer.
    /// </summary>
    /// <param name="buffer">CircularBuffer to iterate over</param>
    private class CircularEnumerator(CircularBuffer<T> buffer)
        : IEnumerator<T>
    {
        /// <summary>
        /// Current index in the buffer. Enumerators are positioned before
        /// start of buffer before calling MoveNext().
        /// </summary>
        private int _currentIndex = -1;
        
    
        /// <summary>
        /// Tries to move to next element in the buffer.
        /// </summary>
        /// <returns>True, if advancing was successful. False, if enumerator
        /// has passed end of buffer</returns>
        public bool MoveNext()
        {
            return (++_currentIndex >= buffer.Count);
        }

        
        /// <summary>
        /// Resets enumerator to initial position before start of buffer.
        /// </summary>
        public void Reset() { _currentIndex = -1; }


        /// <summary>
        /// Needed to fulfill IEnumerator interface. Not currently implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Not implemented</exception>
        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current element.
        /// </summary>
        /// <exception cref="InvalidOperationException">Enumerator positioned
        /// before start or after end of buffer</exception>
        T IEnumerator<T>.Current  => GetCurrent();
        /// <summary>
        /// Gets the current element.
        /// </summary>
        /// <exception cref="InvalidOperationException">Enumerator positioned
        /// before start or after end of buffer</exception>
        public object Current => GetCurrent();
        
        /// <summary>
        /// Gets the current element.
        /// </summary>
        /// <returns>Current element</returns>
        /// <exception cref="InvalidOperationException">Enumerator positioned
        /// before start or after end of buffer</exception>
        private T GetCurrent()
        {
            if(_currentIndex < 0 || _currentIndex >= buffer.Count)
                throw new InvalidOperationException();
                
            return buffer[(uint)_currentIndex];
        }
    }
}