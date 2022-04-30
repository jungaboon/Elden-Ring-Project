// Created by Ronis Vision. All rights reserved
// 13.08.2020.

using System;
using System.Collections;
using System.Collections.Generic;

namespace RVModules.RVUtilities
{
    namespace CircularBuffer
    {
        /// <inheritdoc/>
        /// <summary>
        /// Circular buffer.
        /// 
        /// When writing to a full buffer:
        /// PushBack -> removes this[0] / Front()
        /// PushFront -> removes this[Size-1] / Back()
        /// </summary>
        public class RingBuffer<T> : IEnumerable<T>
        {
            #region Fields

            private readonly T[] buffer;

            /// <summary>
            /// The _start. Index of the first element in buffer.
            /// </summary>
            private int start;

            /// <summary>
            /// The _end. Index after the last element in the buffer.
            /// </summary>
            private int end;

            /// <summary>
            /// The _size. Buffer size.
            /// </summary>
            private int size;

            #endregion

            #region Properties

            /// <summary>
            /// Maximum capacity of the buffer. Elements pushed into the buffer after
            /// maximum capacity is reached (IsFull = true), will remove an element.
            /// </summary>
            public int Capacity => buffer.Length;

            public bool IsFull => Size == Capacity;

            public bool IsEmpty => Size == 0;

            /// <summary>
            /// Current buffer size (the number of elements that the buffer has).
            /// </summary>
            public int Size => size;

            public T this[int index]
            {
                get
                {
                    if (IsEmpty) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");

                    if (index >= size) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {size}");

                    var actualIndex = InternalIndex(index);
                    return buffer[actualIndex];
                }
                set
                {
                    if (IsEmpty) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");

                    if (index >= size) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {size}");

                    var actualIndex = InternalIndex(index);
                    buffer[actualIndex] = value;
                }
            }

            #endregion

            public RingBuffer(int capacity)
                : this(capacity, new T[] { })
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
            /// 
            /// </summary>
            /// <param name='capacity'>
            /// Buffer capacity. Must be positive.
            /// </param>
            /// <param name='items'>
            /// Items to fill buffer with. Items length must be less than capacity.
            /// Suggestion: use Skip(x).Take(y).ToArray() to build this argument from
            /// any enumerable.
            /// </param>
            public RingBuffer(int capacity, T[] items)
            {
                if (capacity < 1)
                    throw new ArgumentException(
                        "Circular buffer cannot have negative or zero capacity.", nameof(capacity));

                if (items == null) throw new ArgumentNullException(nameof(items));

                if (items.Length > capacity)
                    throw new ArgumentException(
                        "Too many items to fit circular buffer", nameof(items));

                buffer = new T[capacity];

                Array.Copy(items, buffer, items.Length);
                size = items.Length;

                start = 0;
                end = size == capacity ? 0 : size;
            }

            #region Public methods

            /// <summary>
            /// Element at the front of the buffer - this[0].
            /// </summary>
            /// <returns>The value of the element of type T at the front of the buffer.</returns>
            public T Front()
            {
                ThrowIfEmpty();
                return buffer[start];
            }

            /// <summary>
            /// Element at the back of the buffer - this[Size - 1].
            /// </summary>
            /// <returns>The value of the element of type T at the back of the buffer.</returns>
            public T Back()
            {
                ThrowIfEmpty();
                return buffer[(end != 0 ? end : Capacity) - 1];
            }

            /// <summary>
            /// Pushes a new element to the back of the buffer. Back()/this[Size-1]
            /// will now return this element.
            /// 
            /// When the buffer is full, the element at Front()/this[0] will be 
            /// popped to allow for this new element to fit.
            /// </summary>
            /// <param name="item">Item to push to the back of the buffer</param>
            public void PushBack(T item)
            {
                if (IsFull)
                {
                    buffer[end] = item;
                    Increment(ref end);
                    start = end;
                }
                else
                {
                    buffer[end] = item;
                    Increment(ref end);
                    ++size;
                }
            }

            /// <summary>
            /// Pushes a new element to the front of the buffer. Front()/this[0]
            /// will now return this element.
            /// 
            /// When the buffer is full, the element at Back()/this[Size-1] will be 
            /// popped to allow for this new element to fit.
            /// </summary>
            /// <param name="item">Item to push to the front of the buffer</param>
            public void PushFront(T item)
            {
                if (IsFull)
                {
                    Decrement(ref start);
                    end = start;
                    buffer[start] = item;
                }
                else
                {
                    Decrement(ref start);
                    buffer[start] = item;
                    ++size;
                }
            }

            /// <summary>
            /// Removes the element at the back of the buffer. Decreasing the 
            /// Buffer size by 1.
            /// </summary>
            public void PopBack()
            {
                ThrowIfEmpty("Cannot take elements from an empty buffer.");
                Decrement(ref end);
                buffer[end] = default;
                --size;
            }

            /// <summary>
            /// Removes the element at the front of the buffer. Decreasing the 
            /// Buffer size by 1.
            /// </summary>
            public void PopFront()
            {
                ThrowIfEmpty("Cannot take elements from an empty buffer.");
                buffer[start] = default;
                Increment(ref start);
                --size;
            }

            /// <summary>
            /// Copies the buffer contents to an array, according to the logical
            /// contents of the buffer (i.e. independent of the internal 
            /// order/contents)
            /// </summary>
            /// <returns>A new array with a copy of the buffer contents.</returns>
            public T[] ToArray()
            {
                var newArray = new T[Size];
                var newArrayOffset = 0;
                var segments = new ArraySegment<T>[2] {ArrayOne(), ArrayTwo()};
                foreach (var segment in segments)
                {
                    Array.Copy(segment.Array, segment.Offset, newArray, newArrayOffset, segment.Count);
                    newArrayOffset += segment.Count;
                }

                return newArray;
            }

            #region IEnumerable<T> implementation

            public IEnumerator<T> GetEnumerator()
            {
                var segments = new ArraySegment<T>[2] {ArrayOne(), ArrayTwo()};
                foreach (var segment in segments)
                    for (var i = 0; i < segment.Count; i++)
                        yield return segment.Array[segment.Offset + i];
            }

            #endregion

            #endregion

            #region Not public methods

            #region IEnumerable implementation

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion

            private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
            {
                if (IsEmpty) throw new InvalidOperationException(message);
            }

            /// <summary>
            /// Increments the provided index variable by one, wrapping
            /// around if necessary.
            /// </summary>
            /// <param name="index"></param>
            private void Increment(ref int index)
            {
                if (++index == Capacity) index = 0;
            }

            /// <summary>
            /// Decrements the provided index variable by one, wrapping
            /// around if necessary.
            /// </summary>
            /// <param name="index"></param>
            private void Decrement(ref int index)
            {
                if (index == 0) index = Capacity;

                index--;
            }

            /// <summary>
            /// Converts the index in the argument to an index in <code>_buffer</code>
            /// </summary>
            /// <returns>
            /// The transformed index.
            /// </returns>
            /// <param name='index'>
            /// External index.
            /// </param>
            private int InternalIndex(int index) => start + (index < Capacity - start ? index : index - Capacity);

            #endregion

            #region Array items easy access.

            // The array is composed by at most two non-contiguous segments, 
            // the next two methods allow easy access to those.

            private ArraySegment<T> ArrayOne()
            {
                if (IsEmpty)
                    return new ArraySegment<T>(new T[0]);
                if (start < end)
                    return new ArraySegment<T>(buffer, start, end - start);
                return new ArraySegment<T>(buffer, start, buffer.Length - start);
            }

            private ArraySegment<T> ArrayTwo()
            {
                if (IsEmpty)
                    return new ArraySegment<T>(new T[0]);
                if (start < end)
                    return new ArraySegment<T>(buffer, end, 0);
                return new ArraySegment<T>(buffer, 0, end);
            }

            #endregion
        }
    }
}