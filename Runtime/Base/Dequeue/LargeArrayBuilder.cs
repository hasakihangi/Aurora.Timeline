using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aurora.Timeline
{
    internal struct LargeArrayBuilder<T>
{
  private const int StartingCapacity = 4;
  private const int ResizeLimit = 8;
  private readonly int _maxCapacity;
  private T[] _first;
  private ArrayBuilder<T[]> _buffers;
  private T[] _current;
  private int _index;
  private int _count;

  public LargeArrayBuilder(bool initialize)
    : this(int.MaxValue)
  {
  }

  public LargeArrayBuilder(int maxCapacity)
    : this()
  {
    this._first = this._current = Array.Empty<T>();
    this._maxCapacity = maxCapacity;
  }

  public int Count => this._count;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Add(T item)
  {
    int index = this._index;
    T[] current = this._current;
    if ((uint) index >= (uint) current.Length)
    {
      this.AddWithBufferAllocation(item);
    }
    else
    {
      current[index] = item;
      this._index = index + 1;
    }
    ++this._count;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private void AddWithBufferAllocation(T item)
  {
    this.AllocateBuffer();
    this._current[this._index++] = item;
  }

  public void AddRange(IEnumerable<T> items)
  {
    using (IEnumerator<T> enumerator = items.GetEnumerator())
    {
      T[] current1 = this._current;
      int index = this._index;
      while (enumerator.MoveNext())
      {
        T current2 = enumerator.Current;
        if ((uint) index >= (uint) current1.Length)
          this.AddWithBufferAllocation(current2, ref current1, ref index);
        else
          current1[index] = current2;
        ++index;
      }
      this._count += index - this._index;
      this._index = index;
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private void AddWithBufferAllocation(T item, ref T[] destination, ref int index)
  {
    this._count += index - this._index;
    this._index = index;
    this.AllocateBuffer();
    destination = this._current;
    index = this._index;
    this._current[index] = item;
  }

  public void CopyTo(T[] array, int arrayIndex, int count)
  {
    int index = 0;
    while (count > 0)
    {
      T[] buffer = this.GetBuffer(index);
      int length = Math.Min(count, buffer.Length);
      Array.Copy((Array) buffer, 0, (Array) array, arrayIndex, length);
      count -= length;
      arrayIndex += length;
      ++index;
    }
  }

  public CopyPosition CopyTo(CopyPosition position, T[] array, int arrayIndex, int count)
  {
    int row = position.Row;
    int column = position.Column;
    T[] buffer1 = this.GetBuffer(row);
    int core1 = CopyToCore(buffer1, column);
    if (count == 0)
      return new CopyPosition(row, column + core1).Normalize(buffer1.Length);
    T[] buffer2;
    int core2;
    do
    {
      buffer2 = this.GetBuffer(++row);
      core2 = CopyToCore(buffer2, 0);
    }
    while (count > 0);
    return new CopyPosition(row, core2).Normalize(buffer2.Length);

    int CopyToCore(T[] sourceBuffer, int sourceIndex)
    {
      int length = Math.Min(sourceBuffer.Length - sourceIndex, count);
      Array.Copy((Array) sourceBuffer, sourceIndex, (Array) array, arrayIndex, length);
      arrayIndex += length;
      count -= length;
      return length;
    }
  }

  public T[] GetBuffer(int index)
  {
    if (index == 0)
      return this._first;
    return index > this._buffers.Count ? this._current : this._buffers[index - 1];
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public void SlowAdd(T item) => this.Add(item);

  public T[] ToArray()
  {
    T[] array1;
    if (this.TryMove(out array1))
      return array1;
    T[] array2 = new T[this._count];
    this.CopyTo(array2, 0, this._count);
    return array2;
  }

  public bool TryMove(out T[] array)
  {
    array = this._first;
    return this._count == this._first.Length;
  }

  private void AllocateBuffer()
  {
    if ((uint) this._count < 8U)
    {
      this._current = new T[Math.Min(this._count == 0 ? 4 : this._count * 2, this._maxCapacity)];
      Array.Copy((Array) this._first, 0, (Array) this._current, 0, this._count);
      this._first = this._current;
    }
    else
    {
      int length;
      if (this._count == 8)
      {
        length = 8;
      }
      else
      {
        this._buffers.Add(this._current);
        length = Math.Min(this._count, this._maxCapacity - this._count);
      }
      this._current = new T[length];
      this._index = 0;
    }
  }
}

}
