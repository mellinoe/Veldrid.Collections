using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Xunit;

namespace Veldrid.Collections.Tests
{
    public class NativeListTests
    {
        [Fact]
        public static void Basic()
        {
            NativeList<int> list = new NativeList<int>(10);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            list[3] = 100;

            Assert.Equal(100, list[3]);
            ref int valRef = ref list[3];
            Assert.Equal(100, valRef);
            valRef = 5;

            Assert.Equal(5, list[3]);
        }

        [Fact]
        public static void Remove()
        {
            NativeList<int> list = CreateListWithRange(5);

            list.Remove(1); // 0, 4, 2, 3
            Assert.Equal(4u, list.Count);
            Assert.Equal(4, list[1]);

            list.Remove(0); // 3, 4, 2
            Assert.Equal(3u, list.Count);
            Assert.Equal(3, list[0]);

            list.Remove(3); // 2, 4
            Assert.Equal(2u, list.Count);
            Assert.Equal(2, list[0]);
        }

        [Fact]
        public static void RemoveAt()
        {
            NativeList<int> list = CreateListWithRange(5);

            list.RemoveAt(1); // 0, 4, 2, 3
            Assert.Equal(4u, list.Count);
            Assert.Equal(4, list[1]);

            list.RemoveAt(0); // 3, 4, 2
            Assert.Equal(3u, list.Count);
            Assert.Equal(3, list[0]);

            list.RemoveAt(0); // 2, 4
            Assert.Equal(2u, list.Count);
            Assert.Equal(2, list[0]);
        }

        [Fact]
        public static unsafe void AddPointer()
        {
            NativeList<float> list = new NativeList<float>();
            float x = 1f;
            list.Add(&x, 1);
            Assert.Equal(1u, list.Count);
            Assert.Equal(1f, list[0u]);

            float* floats = stackalloc float[3];
            floats[0] = 2f;
            floats[1] = 3f;
            floats[2] = 4f;
            list.Add(floats, 3);
            Assert.Equal(4u, list.Count);
            Assert.Equal(2f, list[1u]);
            Assert.Equal(3f, list[2u]);
            Assert.Equal(4f, list[3u]);

            float* floats2 = stackalloc float[5];
            floats2[0] = 5f;
            floats2[1] = 6f;
            floats2[2] = 7f;
            floats2[3] = 8f;
            floats2[4] = 9f;
            list.Add(floats2, 5);
            Assert.Equal(9u, list.Count);
            Assert.Equal(5f, list[4u]);
            Assert.Equal(6f, list[5u]);
            Assert.Equal(7f, list[6u]);
            Assert.Equal(8f, list[7u]);
            Assert.Equal(9f, list[8u]);

            Vector4 v = new Vector4(10, 11, 12, 13);
            float* vPtr = (float*)&v;
            list.Add(vPtr, 4);
            Assert.Equal(13u, list.Count);
            Assert.Equal(10f, list[9u]);
            Assert.Equal(11f, list[10u]);
            Assert.Equal(12f, list[11u]);
            Assert.Equal(13f, list[12u]);
        }

        [Fact]
        public static void Resize()
        {
            NativeList<int> list = CreateListWithRange(5);

            list.Resize(2);
            Assert.Equal(2u, list.Count);
            Assert.Equal(0, list[0]);
            Assert.Equal(1, list[1]);

            list.Resize(5);
            Assert.Equal(0, list[0]);
            Assert.Equal(1, list[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[2]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[4]);
        }

        [Fact]
        public static void SetCount()
        {
            NativeList<int> list = CreateListWithRange(6);

            list.Count = 3;
            Assert.Equal(3u, list.Count);
            Assert.Equal(0, list[0]);
            Assert.Equal(1, list[1]);
            Assert.Equal(2, list[2]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[4]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[5]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[6]);

            list.Count = 20;
            for (int i = 3; i < 20; i++)
            {
                Assert.Equal(0, list[i]);
            }
        }

        [Fact]
        public static void Enumerate()
        {
            NativeList<int> list = CreateListWithRange(6);

            int index = 0;
            foreach (int x in list)
            {
                Assert.Equal(index, x);
                index += 1;
            }
        }

        [Fact]
        public static void ResetEnumerator()
        {
            NativeList<int> list = CreateListWithRange(6);

            var enumerator = list.GetEnumerator();
            Assert.Equal(0, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(0, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);
            Assert.True(enumerator.MoveNext());

            enumerator.Reset();
            Assert.Equal(0, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(0, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);
            Assert.True(enumerator.MoveNext());
        }

        [Fact]
        public static void ReadOnlyViewBasic()
        {
            NativeList<int> list = CreateListWithRange(6);
            var view = list.GetReadOnlyView();
            for (int i = 0; i < view.Count; i++)
            {
                Assert.Equal(i, view[i]);
            }
        }

        [Fact]
        public static void ReadOnlyViewOffset()
        {
            NativeList<int> list = CreateListWithRange(6);
            var view = list.GetReadOnlyView(2, 4);
            for (int i = 0; i < view.Count; i++)
            {
                Assert.Equal(i + 2, view[i]);
            }
        }

        [Fact]
        public static void ReadOnlyViewWithCount()
        {
            NativeList<int> list = CreateListWithRange(10);
            var view = list.GetReadOnlyView(0, 2);
            Assert.Equal(2u, view.Count);
            Assert.Equal(0, view[0u]);
            Assert.Equal(1, view[1u]);
            Assert.Throws<ArgumentOutOfRangeException>(() => view[2u]);

            view = list.GetReadOnlyView(0, 3);
            Assert.Equal(0, view[0u]);
            Assert.Equal(1, view[1u]);
            Assert.Equal(2, view[2u]);

            view = list.GetReadOnlyView(1, 4);
            Assert.Equal(1, view[0u]);
            Assert.Equal(2, view[1u]);
            Assert.Equal(3, view[2u]);
            Assert.Equal(4, view[3u]);
            Assert.Throws<ArgumentOutOfRangeException>(() => view[4u]);
        }

        [Fact]
        public static void Clear()
        {
            NativeList<int> list = CreateListWithRange(10);
            list.Clear();
            Assert.Equal(0u, list.Count);
            list.Count = 10;
            for (int i = 0; i < list.Count; i++)
            {
                Assert.Equal(i, list[i]);
            }
        }

        [Fact]
        public static void IndexOf()
        {
            NativeList<int> list = new NativeList<int>();
            list.Add(10);
            list.Add(100);
            list.Add(150);

            Assert.True(list.IndexOf(10, out uint index));
            Assert.Equal(0u, index);
            Assert.Equal(10, list[index]);

            Assert.True(list.IndexOf(100, out index));
            Assert.Equal(1u, index);
            Assert.Equal(100, list[index]);

            Assert.True(list.IndexOf(150, out index));
            Assert.Equal(2u, index);
            Assert.Equal(150, list[index]);

            int x = 250;
            list.Add(ref x);
            list.Add(ref x);
            Assert.True(list.IndexOf(ref x, out index));
            Assert.Equal(3u, index);
            Assert.Equal(x, list[index]);
        }

        [Fact]
        public static void IndexOfCompoundType()
        {
            NativeList<Matrix4x4> list = new NativeList<Matrix4x4>();
            Matrix4x4 m1 = new Matrix4x4(11, 12, 13, 14, 21, 22, 23, 24, 31, 32, 33, 34, 41, 42, 43, 44);
            Matrix4x4 m2 = m1 * 1.5f;
            list.Add(Matrix4x4.Identity);
            list.Add(ref m1);
            list.Add(ref m2);
            list.Add(Matrix4x4.Identity);

            Assert.True(list.IndexOf(m1, out uint index));
            Assert.Equal(1u, index);
            Assert.Equal(m1, list[index]);

            Assert.True(list.IndexOf(ref m2, out index));
            Assert.Equal(2u, index);
            Assert.Equal(m2, list[index]);

            Assert.False(list.IndexOf(new Matrix4x4(), out index));
        }

        [Fact]
        public static void IndexOfCompoundType2()
        {
            NativeList<(Vector3 A, Vector4 B)> list = new NativeList<(Vector3, Vector4)>();
            (Vector3, Vector4) x = (new Vector3(0, 1, 2), new Vector4(3, 4, 5, 6));
            list.Add((new Vector3(7, 8, 9), new Vector4(10, 11, 12, 13)));
            list.Add(x);
            list.Add((new Vector3(14, 15, 16), new Vector4(17, 18, 19, 20)));

            Assert.True(list.IndexOf(ref x, out uint index));
            Assert.Equal(1u, index);
            Assert.Equal(x, list[index]);

            Assert.False(list.IndexOf((Vector3.One, Vector4.One), out index));
        }

        [Fact]
        public static unsafe void FloatToVectorView()
        {
            NativeList<float> list = new NativeList<float>();
            NativeList<float>.View<Vector3> view = list.GetView<Vector3>();
            Assert.Equal(0u, view.Count);
            list.Add(1f);
            Assert.Equal(0u, view.Count);
            list.Add(2f);
            Assert.Equal(0u, view.Count);
            list.Add(3f);
            Assert.Equal(1u, view.Count);
            Assert.Equal(new Vector3(1f, 2f, 3f), view[0u]);
            Vector3 x = new Vector3(4f, 5f, 6f);
            list.Add(&x.X, 3);
            Assert.Equal(2u, view.Count);
            Assert.Equal(x, view[1]);
        }

        [Fact]
        public static void Sort()
        {
            NativeList<int> keys = new NativeList<int>();
            keys.Add(5);
            keys.Add(4);
            keys.Add(3);
            keys.Add(2);
            keys.Add(1);
            keys.Add(0);

            NativeList<float> items = new NativeList<float>();
            items.Add(0f);
            items.Add(1f);
            items.Add(2f);
            items.Add(3f);
            items.Add(4f);
            items.Add(5f);

            NativeList.Sort(keys, items, 0, keys.Count, Comparer<int>.Default);
            Assert.Equal(0, keys[0u]);
            Assert.Equal(1, keys[1u]);
            Assert.Equal(2, keys[2u]);
            Assert.Equal(3, keys[3u]);
            Assert.Equal(4, keys[4u]);
            Assert.Equal(5, keys[5u]);

            Assert.Equal(5f, items[0u]);
            Assert.Equal(4f, items[1u]);
            Assert.Equal(3f, items[2u]);
            Assert.Equal(2f, items[3u]);
            Assert.Equal(1f, items[4u]);
            Assert.Equal(0f, items[5u]);
        }

        private static NativeList<int> CreateListWithRange(uint count, int start = 0, int increment = 1)
        {
            NativeList<int> list = new NativeList<int>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(i * increment + start);
            }

            return list;
        }
    }
}