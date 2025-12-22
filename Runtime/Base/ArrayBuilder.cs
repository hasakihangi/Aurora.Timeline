using System;

namespace Aurora.Timeline
{
    internal struct ArrayBuilder<T>
    {
        private const int DefaultCapacity = 4;
        private const int MaxCoreClrArrayLength = 2146435071;
        private T[] _array;
        private int _count;

        public ArrayBuilder(int capacity)
            : this()
        {
            if (capacity <= 0)
                return;
            this._array = new T[capacity];
        }

        public int Capacity
        {
            get
            {
                T[] array = this._array;
                return array == null ? 0 : array.Length;
            }
        }

        public int Count => this._count;

        public T this[int index]
        {
            get => this._array[index];
            set => this._array[index] = value;
        }

        public void Add(T item)
        {
            if (this._count == this.Capacity)
                this.EnsureCapacity(this._count + 1);
            this.UncheckedAdd(item);
        }

        public T First() => this._array[0];

        public T Last() => this._array[this._count - 1];

        public T[] ToArray()
        {
            if (this._count == 0)
                return Array.Empty<T>();
            T[] destinationArray = this._array;
            if (this._count < destinationArray.Length)
            {
                destinationArray = new T[this._count];
                Array.Copy((Array) this._array, 0, (Array) destinationArray, 0, this._count);
            }
            return destinationArray;
        }

        public void UncheckedAdd(T item) => this._array[this._count++] = item;

        private void EnsureCapacity(int minimum)
        {
            int capacity = this.Capacity;
            int val1 = capacity == 0 ? 4 : 2 * capacity;
            if ((uint) val1 > 2146435071U)
                val1 = Math.Max(capacity + 1, 2146435071);
            T[] destinationArray = new T[Math.Max(val1, minimum)];
            if (this._count > 0)
                Array.Copy((Array) this._array, 0, (Array) destinationArray, 0, this._count);
            this._array = destinationArray;
        }
    }

}
