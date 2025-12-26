using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Aurora.Timeline
{
    public class Dequeue<T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
    {
        private T[] _array;
        private int _head;
        private int _tail;
        private int _size;
        private int _version;
        private const int MinimumGrow = 4;
        private const int GrowFactor = 200;

        public Dequeue() => this._array = Array.Empty<T>();

        public Dequeue(int capacity)
        {
            if (capacity < 0)
            {
                Debug.LogError($"Dequeue: capacity must be non-negative, got {capacity}");
                this._array = Array.Empty<T>();
                return;
            }
            this._array = new T[capacity];
        }

        public Dequeue(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                Debug.LogError("Dequeue: collection cannot be null");
                this._array = Array.Empty<T>();
                return;
            }
            this._array = EnumerableHelpers.ToArray<T>(collection, out this._size);
            if (this._size != this._array.Length)
                this._tail = this._size;
        }

        public int Count => this._size;

        public int Capacity => this._array.Length;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public void Clear()
        {
            if (this._size != 0)
            {
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    if (this._head < this._tail)
                    {
                        Array.Clear(this._array, this._head, this._size);
                    }
                    else
                    {
                        Array.Clear(this._array, this._head, this._array.Length - this._head);
                        Array.Clear(this._array, 0, this._tail);
                    }
                }
                this._size = 0;
            }
            this._head = 0;
            this._tail = 0;
            ++this._version;
        }

        public void EnqueueBack(T item)
        {
            if (this._size == this._array.Length)
            {
                int capacity = (int)((long)this._array.Length * 200L / 100L);
                if (capacity < this._array.Length + 4)
                    capacity = this._array.Length + 4;
                this.SetCapacity(capacity);
            }

            this._array[this._tail] = item;
            this.MoveNext(ref this._tail);
            ++this._size;
            ++this._version;
        }

        public void EnqueueFront(T item)
        {
            if (this._size == this._array.Length)
            {
                int capacity = (int)((long)this._array.Length * 200L / 100L);
                if (capacity < this._array.Length + 4)
                    capacity = this._array.Length + 4;
                this.SetCapacity(capacity);
            }

            this.MovePrevious(ref this._head);
            this._array[this._head] = item;
            ++this._size;
            ++this._version;
        }

        public T DequeueFront()
        {
            if (this._size == 0)
            {
                Debug.LogError("Dequeue: Cannot dequeue from empty dequeue");
                return default(T);
            }

            int head = this._head;
            T[] array = this._array;
            T obj = array[head];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                array[head] = default(T);
            this.MoveNext(ref this._head);
            --this._size;
            ++this._version;
            return obj;
        }


        public T DequeueBack()
        {
            if (this._size == 0)
            {
                Debug.LogError("Dequeue: Cannot dequeue from empty dequeue");
                return default(T);
            }

            this.MovePrevious(ref this._tail);
            T[] array = this._array;
            T obj = array[this._tail];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                array[this._tail] = default(T);
            --this._size;
            ++this._version;
            return obj;
        }


        public bool TryDequeueFront(out T result)
        {
            if (this._size == 0)
            {
                result = default(T);
                return false;
            }

            int head = this._head;
            T[] array = this._array;
            result = array[head];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                array[head] = default(T);
            this.MoveNext(ref this._head);
            --this._size;
            ++this._version;
            return true;
        }


        public bool TryDequeueBack(out T result)
        {
            if (this._size == 0)
            {
                result = default(T);
                return false;
            }

            this.MovePrevious(ref this._tail);
            T[] array = this._array;
            result = array[this._tail];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                array[this._tail] = default(T);
            --this._size;
            ++this._version;
            return true;
        }


        public T PeekFront()
        {
            if (this._size == 0)
            {
                return default(T);
            }
            return this._array[this._head];
        }


        public T PeekBack()
        {
            if (this._size == 0)
            {
                return default(T);
            }
            int backIndex = this._tail - 1;
            if (backIndex < 0)
                backIndex = this._array.Length - 1;
            return this._array[backIndex];
        }


        public bool TryPeekFront(out T result)
        {
            if (this._size == 0)
            {
                result = default(T);
                return false;
            }
            result = this._array[this._head];
            return true;
        }


        public bool TryPeekBack(out T result)
        {
            if (this._size == 0)
            {
                result = default(T);
                return false;
            }
            int backIndex = this._tail - 1;
            if (backIndex < 0)
                backIndex = this._array.Length - 1;
            result = this._array[backIndex];
            return true;
        }

        public bool Contains(T item)
        {
            if (this._size == 0)
                return false;
            if (this._head < this._tail)
                return Array.IndexOf<T>(this._array, item, this._head, this._size) >= 0;
            return Array.IndexOf<T>(this._array, item, this._head, this._array.Length - this._head) >= 0 ||
                   Array.IndexOf<T>(this._array, item, 0, this._tail) >= 0;
        }

        public T[] ToArray()
        {
            if (this._size == 0)
                return Array.Empty<T>();
            T[] destinationArray = new T[this._size];
            if (this._head < this._tail)
            {
                Array.Copy(this._array, this._head, destinationArray, 0, this._size);
            }
            else
            {
                Array.Copy(this._array, this._head, destinationArray, 0, this._array.Length - this._head);
                Array.Copy(this._array, 0, destinationArray, this._array.Length - this._head, this._tail);
            }
            return destinationArray;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                Debug.LogError("Dequeue: array cannot be null");
                return;
            }
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                Debug.LogError($"Dequeue: arrayIndex {arrayIndex} is out of range");
                return;
            }
            if (array.Length - arrayIndex < this._size)
            {
                Debug.LogError("Dequeue: array is too small to copy all elements");
                return;
            }
            int size = this._size;
            if (size == 0)
                return;
            int length1 = Math.Min(this._array.Length - this._head, size);
            Array.Copy(this._array, this._head, array, arrayIndex, length1);
            int length2 = size - length1;
            if (length2 > 0)
                Array.Copy(this._array, 0, array, arrayIndex + this._array.Length - this._head, length2);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                Debug.LogError("Dequeue: array cannot be null");
                return;
            }
            if (array.Rank != 1)
            {
                Debug.LogError("Dequeue: only single dimensional arrays are supported");
                return;
            }
            if (array.GetLowerBound(0) != 0)
            {
                Debug.LogError("Dequeue: array lower bound must be zero");
                return;
            }
            int num = array.Length;
            if (index < 0 || index > num)
            {
                Debug.LogError($"Dequeue: index {index} is out of range");
                return;
            }
            if (num - index < this._size)
            {
                Debug.LogError("Dequeue: array is too small to copy all elements");
                return;
            }
            int size = this._size;
            if (size == 0)
                return;
            try
            {
                int length1 = this._array.Length - this._head < size ? this._array.Length - this._head : size;
                Array.Copy(this._array, this._head, array, index, length1);
                int length2 = size - length1;
                if (length2 > 0)
                    Array.Copy(this._array, 0, array, index + this._array.Length - this._head, length2);
            }
            catch (ArrayTypeMismatchException)
            {
                Debug.LogError("Dequeue: target array type is not compatible");
            }
        }

        public void TrimExcess()
        {
            if (this._size >= (int)((double)this._array.Length * 0.9))
                return;
            this.SetCapacity(this._size);
        }

        private void SetCapacity(int capacity)
        {
            T[] destinationArray = new T[capacity];
            if (this._size > 0)
            {
                if (this._head < this._tail)
                {
                    Array.Copy(this._array, this._head, destinationArray, 0, this._size);
                }
                else
                {
                    Array.Copy(this._array, this._head, destinationArray, 0, this._array.Length - this._head);
                    Array.Copy(this._array, 0, destinationArray, this._array.Length - this._head, this._tail);
                }
            }
            this._array = destinationArray;
            this._head = 0;
            this._tail = this._size == capacity ? 0 : this._size;
            ++this._version;
        }

        private void MoveNext(ref int index)
        {
            int num = index + 1;
            if (num == this._array.Length)
                num = 0;
            index = num;
        }

        private void MovePrevious(ref int index)
        {
            int num = index - 1;
            if (num < 0)
                num = this._array.Length - 1;
            index = num;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        [Serializable]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private readonly Dequeue<T> _dequeue;
            private readonly int _version;
            private int _index;
            private T _currentElement;

            internal Enumerator(Dequeue<T> dequeue)
            {
                this._dequeue = dequeue;
                this._version = dequeue._version;
                this._index = -1;
                this._currentElement = default(T);
            }

            public void Dispose()
            {
                this._index = -2;
                this._currentElement = default(T);
            }

            public bool MoveNext()
            {
                if (this._version != this._dequeue._version)
                {
                    Debug.LogError("Dequeue: collection was modified during enumeration");
                    return false;
                }
                if (this._index == -2)
                    return false;
                ++this._index;
                if (this._index == this._dequeue._size)
                {
                    this._index = -2;
                    this._currentElement = default(T);
                    return false;
                }

                T[] array = this._dequeue._array;
                int length = array.Length;
                int index = this._dequeue._head + this._index;
                if (index >= length)
                    index -= length;
                this._currentElement = array[index];
                return true;
            }

            public T Current
            {
                get
                {
                    if (this._index < 0)
                    {
                        Debug.LogError(this._index == -1
                            ? "Dequeue: enumeration has not started"
                            : "Dequeue: enumeration already finished");
                        return default(T);
                    }
                    return this._currentElement;
                }
            }

            object IEnumerator.Current => this.Current;

            void IEnumerator.Reset()
            {
                if (this._version != this._dequeue._version)
                {
                    Debug.LogError("Dequeue: collection was modified during enumeration");
                    return;
                }
                this._index = -1;
                this._currentElement = default(T);
            }
        }
    }
}
