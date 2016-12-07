using System;
using Xunit;

namespace Veldrid.Collections.Tests
{
    public class RawListTests
    {
        [Fact]
        public void Ctor_ZeroCapacity()
        {
            RawList<int> list = new RawList<int>(0);
            Assert.Same(Array.Empty<int>(), list.Items);
        }

        [Fact]
        public void CtorRegular()
        {
            uint capacity = 10;
            RawList<int> list = new RawList<int>(capacity);
            Assert.Equal(capacity, (uint)list.Items.Length);
            Assert.Equal(0u, list.Count);
        }

        [Fact]
        public void CtorEmpty()
        {
            uint capacity = 0;
            RawList<int> list = new RawList<int>(0);
            Assert.Equal(capacity, (uint)list.Items.Length);
            Assert.Equal(0u, list.Count);
        }

        [Fact]
        public void CtorDefault()
        {
            RawList<int> list = new RawList<int>();
            Assert.Equal(RawList<int>.DefaultCapacity, (uint)list.Items.Length);
            Assert.Equal(0u, list.Count);
        }

        [Fact]
        public void Add()
        {
            RawList<int> list = new RawList<int>();
            list.Add(1);
            Assert.Equal(1u, list.Count);
            Assert.Equal(1, list[0]);

            int val = 2;
            list.Add(ref val);
            Assert.Equal(2u, list.Count);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public void Remove()
        {
            RawList<int> list = new RawList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            Assert.True(list.Remove(1));
            Assert.Equal(2u, list.Count);
            Assert.Equal(2, list[0]);
            Assert.Equal(3, list[1]);

            Assert.True(list.Remove(2));
            Assert.Equal(1u, list.Count);
            Assert.Equal(3, list[0]);

            Assert.True(list.Remove(3));
            Assert.Equal(0u, list.Count);
        }
        
        [Fact]
        public void RemoveAt()
        {
            RawList<int> list = new RawList<int>();
            int[] values = { 0, 1, 2, 3, 4, 5 };
            list.AddRange(values);
            list.RemoveAt(3);
            Assert.Equal(5u, list.Count);
            Assert.Equal(2, list[2]);
            Assert.Equal(4, list[3]);
            Assert.Equal(5, list[4]);
        }

        [Fact]
        public void AddRange()
        {
            RawList<int> list = new RawList<int>();
            int[] values = { 0, 1, 2, 3, 4, 5 };
            list.AddRange(values);
            Assert.Equal(6u, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Assert.Equal(i, list[i]);
            }
        }
    }
}