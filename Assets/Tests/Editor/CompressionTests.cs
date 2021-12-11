using Fp.Network.Compression;

using NUnit.Framework;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace Tests.Editor
{
    public class CompressionTests
    {
        [Test]
        public void DirectionToInt16()
        {
            Vector3[] testDirs = { Vector3.up, Vector3.forward, Vector3.right, Vector3.down, Vector3.back, Vector3.left };
            
            foreach (Vector3 dir in testDirs)
            {
                Assert.IsTrue(Mathf.Approximately(dir.magnitude, 1));
                
                ushort compressed = VectorCompression.PackDirectionToUInt16(dir);
                TestContext.WriteLine($"{dir} -> {compressed}");
                Vector3 decompressed = VectorCompression.UnpackDirection(compressed);
                TestContext.WriteLine($"{compressed} -> {decompressed}");
                
                Assert.IsTrue(Mathf.Approximately(decompressed.magnitude, 1), $"{decompressed.magnitude}");
                
                float angle = Vector3.Angle(dir, decompressed);
                
                Assert.AreEqual(0, angle);
                Assert.AreEqual(dir, decompressed);
            }
        }
        
        [Test]
        public void RandomDirectionToInt16()
        {
            var minAngle = float.MaxValue;
            float maxAngle = 0;
            float avgAngle = 0;
            
            for (var i = 0; i < ushort.MaxValue; i++)
            {
                Vector3 randomDirection = Random.onUnitSphere;

                Assert.IsTrue(Mathf.Approximately(randomDirection.magnitude, 1));
                
                ushort compressed = VectorCompression.PackDirectionToUInt16(randomDirection);
                
                Vector3 decompressed = VectorCompression.UnpackDirection(compressed);
                
                Assert.IsTrue(Mathf.Approximately(decompressed.magnitude, 1), $"{decompressed.magnitude}");

                float angle = Vector3.Angle(randomDirection, decompressed);
                
                maxAngle = Mathf.Max(angle, maxAngle);
                minAngle = Mathf.Min(angle, minAngle);
                avgAngle += (angle - avgAngle) / (i + 1);
            }

            TestContext.WriteLine($"Max angle {maxAngle:F7}");
            TestContext.WriteLine($"Min angle {minAngle:F7}");
            TestContext.WriteLine($"Avg angle {avgAngle:F7}");
        }
        
        [Test]
        public void DirectionToInt32()
        {
            Vector3[] testDirs = { Vector3.up, Vector3.forward, Vector3.right, Vector3.down, Vector3.back, Vector3.left };
            
            foreach (Vector3 dir in testDirs)
            {
                Assert.IsTrue(Mathf.Approximately(dir.magnitude, 1));
                
                uint compressed = VectorCompression.PackDirectionToUInt32(dir);
                TestContext.WriteLine($"{dir} -> {compressed}");
                Vector3 decompressed = VectorCompression.UnpackDirection(compressed);
                TestContext.WriteLine($"{compressed} -> {decompressed}");
                
                Assert.IsTrue(Mathf.Approximately(decompressed.magnitude, 1), $"{decompressed.magnitude}");
                
                float angle = Vector3.Angle(dir, decompressed);
                
                Assert.AreEqual(0, angle);
                Assert.AreEqual(dir, decompressed);
            }
        }
        
        [Test]
        public void RandomDirectionToInt32()
        {
            var minAngle = float.MaxValue;
            float maxAngle = 0;
            float avgAngle = 0;
            
            for (var i = 0; i < ushort.MaxValue; i++)
            {
                Vector3 randomDirection = Random.onUnitSphere;

                Assert.IsTrue(Mathf.Approximately(randomDirection.magnitude, 1));
                
                uint compressed = VectorCompression.PackDirectionToUInt32(randomDirection);
                
                Vector3 decompressed = VectorCompression.UnpackDirection(compressed);
                
                Assert.IsTrue(Mathf.Approximately(decompressed.magnitude, 1), $"{decompressed.magnitude}");

                float angle = Vector3.Angle(randomDirection, decompressed);
                
                maxAngle = Mathf.Max(angle, maxAngle);
                minAngle = Mathf.Min(angle, minAngle);
                avgAngle += (angle - avgAngle) / (i + 1);
            }

            TestContext.WriteLine($"Max angle {maxAngle:F7}");
            TestContext.WriteLine($"Min angle {minAngle:F7}");
            TestContext.WriteLine($"Avg angle {avgAngle:F7}");
        }

        [Test]
        public void RandomRotationToInt32()
        {
            var minAngle = float.MaxValue;
            float maxAngle = 0;
            float avgAngle = 0;
            
            for (var i = 0; i < ushort.MaxValue; i++)
            {
                Quaternion randomRotation = Random.rotation;

                Assert.IsTrue(randomRotation.IsNormalized());
                
                uint compressed = QuaternionCompression.PackToUint32(randomRotation);
                
                Quaternion decompressed = QuaternionCompression.Unpack(compressed);
                
                Assert.IsTrue(decompressed.IsNormalized(), $"{decompressed.MagnitudeF()}");

                float angle = Quaternion.Angle(randomRotation, decompressed);
                
                maxAngle = Mathf.Max(angle, maxAngle);
                minAngle = Mathf.Min(angle, minAngle);
                avgAngle += (angle - avgAngle) / (i + 1);
            }

            TestContext.WriteLine($"Max angle {maxAngle:F7}");
            TestContext.WriteLine($"Min angle {minAngle:F7}");
            TestContext.WriteLine($"Avg angle {avgAngle:F7}");
        }
        
        [Test]
        public void RandomRotationToInt64()
        {
            var minAngle = float.MaxValue;
            float maxAngle = 0;
            float avgAngle = 0;
            
            for (var i = 0; i < ushort.MaxValue; i++)
            {
                Quaternion randomRotation = Random.rotation;

                Assert.IsTrue(randomRotation.IsNormalized());
                
                ulong compressed = QuaternionCompression.PackToUint64(randomRotation);
                
                Quaternion decompressed = QuaternionCompression.Unpack(compressed);
                
                Assert.IsTrue(decompressed.IsNormalized(), $"{decompressed.MagnitudeF()}");

                float angle = Quaternion.Angle(randomRotation, decompressed);
                
                maxAngle = Mathf.Max(angle, maxAngle);
                minAngle = Mathf.Min(angle, minAngle);
                avgAngle += (angle - avgAngle) / (i + 1);
            }

            TestContext.WriteLine($"Max angle {maxAngle:F7}");
            TestContext.WriteLine($"Min angle {minAngle:F7}");
            TestContext.WriteLine($"Avg angle {avgAngle:F7}");
        }
    }
}
