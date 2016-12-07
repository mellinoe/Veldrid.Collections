using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Veldrid.Collections
{
    public class FastRemoveRawList<T> : IEnumerable<T>
    {
        private T[] _items;
        private uint _count;

        private const float GrowthFactor = 2f;

        public FastRemoveRawList(uint capacity)
        {
#if VALIDATE
            if (capacity > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
#else
            Debug.Assert(capacity <= int.MaxValue);
#endif
            _items = capacity == 0 ? Array.Empty<T>() : new T[capacity];
        }

        public uint Count => _count;

        public T[] Items => _items;

        public ArraySegment<T> ArraySegment => new ArraySegment<T>(_items, 0, (int)_count);

        public T this[uint index]
        {
            get
            {
                ValidateIndex(index);
                return _items[index];
            }
            set
            {
                ValidateIndex(index);
                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            if (_count == _items.Length)
            {
                Array.Resize(ref _items, (int)(_items.Length * GrowthFactor));
            }

            _items[_count] = item;
            _count += 1;
        }

        public void AddRange(T[] items)
        {
#if VALIDATE
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
#else
            Debug.Assert(items != null);
#endif

            int requiredSize = (int)(_count + items.Length);
            if (requiredSize > _items.Length)
            {
                Array.Resize(ref _items, (int)(requiredSize * GrowthFactor));
            }

            Array.Copy(items, 0, _items, (int)_count, items.Length);
            _count += (uint)items.Length;
        }

        public void Replace(uint index, ref T item)
        {
            ValidateIndex(index);
            _items[index] = item;
        }

        public void Replace(uint index, T item) => Replace(index, ref item);

        /// <summary>
        /// Removes the first instance of the given item by replacing it with the last item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was sucessfully found and removed; false otherwise.</returns>
        public bool Remove(T item)
        {
            bool contained = GetIndex(item, out uint index);
            if (contained)
            {
                CoreRemoveAt(index);
            }

            return contained;
        }

        /// <summary>
        /// Removes the item at the given index by replacing it with the last item.
        /// </summary>
        /// <param name="item">The index of the item to remove.</param>
        public void RemoveAt(uint index)
        {
            ValidateIndex(index);
            CoreRemoveAt(index);
        }

        /// <summary>
        /// Sets the Count to 0, but does not modify the backing array.
        /// </summary>
        public void Clear()
        {
            _count = 0;
        }

        public bool GetIndex(T item, out uint index)
        {
            int signedIndex = Array.IndexOf(_items, item);
            index = (uint)signedIndex;
            return signedIndex != -1;
        }

        public void Sort() => Sort(null);

        public void Sort(IComparer<T> comparer)
        {
#if VALIDATE
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
#else
            Debug.Assert(comparer != null);
#endif
            Array.Sort(_items, comparer);
        }

        public void TransformAll(Func<T, T> transformation)
        {
#if VALIDATE
            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(transformation));
            }
#else
            Debug.Assert(transformation != null);
#endif

            for (int i = 0; i < _count; i++)
            {
                _items[i] = transformation(_items[i]);
            }
        }

        public ReadOnlyArrayView<T> GetReadOnlyView() => new ReadOnlyArrayView<T>(_items, 0, _count);

        public ReadOnlyArrayView<T>  GetReadOnlyView(uint start, uint length)
        {
#if VALIDATE
            if (start + length >= _count)
            {
                throw new ArgumentOutOfRangeException();
            }
#else
            Debug.Assert(start + length < _count);
#endif
            return new ReadOnlyArrayView<T>(_items, start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CoreRemoveAt(uint index)
        {
            _items[index] = _items[_count - 1];
            _count -= 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateIndex(uint index)
        {
#if VALIDATE
            if (index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
#else
            Debug.Assert(index < _count);
#endif
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private FastRemoveRawList<T> _list;
            private int _currentIndex;

            public Enumerator(FastRemoveRawList<T> list)
            {
                _list = list;
                _currentIndex = 0;
            }

            public T Current => _list._items[_currentIndex];
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_currentIndex != _list._count - 1)
                {
                    _currentIndex += 1;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _currentIndex = 0;
            }

            public void Dispose() { }
        }
    }
}
