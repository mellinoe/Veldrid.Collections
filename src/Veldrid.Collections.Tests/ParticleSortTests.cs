using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Xunit;

namespace Veldrid.Collections.Tests
{
    public static class ParticleSortTests
    {
        [Fact]
        public static void ParticleSorting()
        {
            NativeList<InstanceData> instanceData = new NativeList<InstanceData>();
            NativeList<ParticleStateInternal> state = new NativeList<ParticleStateInternal>();
            CameraDistanceComparer cdc = new CameraDistanceComparer();
            cdc.CurrentCameraPosition = new Vector3(0, 0, -10);

            instanceData.Add(new InstanceData(new Vector3(0, 0, 1), 1f, 1f));
            state.Add(new ParticleStateInternal(1, Vector3.One * 1));

            instanceData.Add(new InstanceData(new Vector3(0, 0, -1), 1f, 1f));
            state.Add(new ParticleStateInternal(2, Vector3.One * 2));

            instanceData.Add(new InstanceData(new Vector3(0, 0, -2f), 1f, 1f));
            state.Add(new ParticleStateInternal(3, Vector3.One * 3));

            instanceData.Add(new InstanceData(new Vector3(0, 0, 10), 1f, 1f));
            state.Add(new ParticleStateInternal(4, Vector3.One * 4));

            instanceData.Add(new InstanceData(new Vector3(0, 0, -2f), 1f, 1f));
            state.Add(new ParticleStateInternal(5, Vector3.One * 5));

            instanceData.Add(new InstanceData(new Vector3(0, 0, -50f), 1f, 1f));
            state.Add(new ParticleStateInternal(6, Vector3.One * 6));

            instanceData.Add(new InstanceData(new Vector3(100, 0, 0), 1f, 1f));
            state.Add(new ParticleStateInternal(7, Vector3.One * 7));

            instanceData.Add(new InstanceData(new Vector3(-200, 0, 0), 1f, 1f));
            state.Add(new ParticleStateInternal(8, Vector3.One * 8));

            instanceData.Add(new InstanceData(new Vector3(0.01f, 0, -10), 2f, 3f));
            state.Add(new ParticleStateInternal(9, Vector3.One * 9));

            NativeList.Sort(instanceData, state, 0, instanceData.Count, cdc);

            Assert.Equal(new ParticleStateInternal(8, Vector3.One * 8), state[0]);
            Assert.Equal(new ParticleStateInternal(7, Vector3.One * 7), state[1]);
            Assert.Equal(new ParticleStateInternal(6, Vector3.One * 6), state[2]);
            Assert.Equal(new ParticleStateInternal(4, Vector3.One * 4), state[3]);
            Assert.Equal(new ParticleStateInternal(1, Vector3.One * 1), state[4]);
            Assert.Equal(new ParticleStateInternal(2, Vector3.One * 2), state[5]);
            Assert.Equal(new ParticleStateInternal(3, Vector3.One * 3), state[6]);
            Assert.Equal(new ParticleStateInternal(5, Vector3.One * 5), state[7]);
            Assert.Equal(new ParticleStateInternal(9, Vector3.One * 9), state[instanceData.Count - 1]);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ParticleStateInternal
        {
            public float Age;
            public Vector3 Velocity;
            public ParticleStateInternal(float age, Vector3 velocity)
            {
                Age = age;
                Velocity = velocity;
            }

            public override string ToString() => $"Age:{Age}, Velocity:{Velocity}";
        }

        private struct InstanceData
        {
            public const byte SizeInBytes = 20;
            public const byte ElementCount = 3;

            public Vector3 Offset;
            public float Alpha;
            public float Size;

            public InstanceData(Vector3 offset, float alpha, float size)
            {
                Offset = offset;
                Alpha = alpha;
                Size = size;
            }

            public override string ToString() => $"OFfset:{Offset}, Alpha:{Alpha}, Size:{Size}";
        }

        private class CameraDistanceComparer : IComparer<InstanceData>
        {
            public Vector3 CurrentCameraPosition { get; set; }

            public int Compare(InstanceData id1, InstanceData id2)
            {
                float distance1 = Vector3.DistanceSquared(CurrentCameraPosition, id1.Offset);
                float distance2 = Vector3.DistanceSquared(CurrentCameraPosition, id2.Offset);

                return distance2.CompareTo(distance1);
            }
        }
    }
}
