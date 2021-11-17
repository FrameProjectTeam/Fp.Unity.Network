using System;

using UnityEngine;
using UnityEngine.Assertions;

namespace Fp.Network.Compression
{
    public static class VectorCompressionUtility
    {
        private const ushort SignMask = 0xE000;
        private const ushort XSignMask = 0x8000;
        private const ushort YSignMask = 0x4000;
        private const ushort ZSignMask = 0x2000;

        private const ushort TopMask = 0x1F80;
        private const ushort BottomMask = 0x007F;

        public static ushort PackDirectionToUint16(Vector3 dir)
        {
            Assert.IsTrue(dir.magnitude < 1.0001f, $"{dir.magnitude} < 1.0001f of vector {dir}");

            ushort res = 0;

            if (dir.x < 0)
            {
                res |= XSignMask;
                dir.x = -dir.x;
            }

            if (dir.y < 0)
            {
                res |= YSignMask;
                dir.y = -dir.y;
            }

            if (dir.z < 0)
            {
                res |= ZSignMask;
                dir.z = -dir.z;
            }

            float w = 126f / (dir.x + dir.y + dir.z);

            var xBits = (ushort) (dir.x * w);
            var yBits = (ushort) (dir.y * w);

            Assert.IsTrue(xBits < 127);
            Assert.IsTrue(yBits < 127);

            if (xBits >= 64)
            {
                xBits = (ushort) (127 - xBits);
                yBits = (ushort) (127 - yBits);
            }

            res |= (ushort) (xBits << 7);
            res |= yBits;

            return res;
        }

        public static Vector3 UnpackDirection(ushort compressed)
        {
            var x = (ushort) ((compressed & TopMask) >> 7);
            var y = (ushort) (compressed & BottomMask);

            if (x + y >= 127)
            {
                x = (ushort) (127 - x);
                y = (ushort) (127 - y);
            }

            var z = (ushort) (126 - x - y);

            float normalizer = 1f / Mathf.Sqrt(x * x + y * y + z * z);

            var vector = new Vector3(x * normalizer, y * normalizer, z * normalizer);

            if ((compressed & XSignMask) > 0)
            {
                vector.x = -vector.x;
            }

            if ((compressed & YSignMask) > 0)
            {
                vector.y = -vector.y;
            }

            if ((compressed & ZSignMask) > 0)
            {
                vector.z = -vector.z;
            }

            return vector;
        }
        
        public static short Encode(float value) {
            var cnt = 0;
            while (Math.Abs(value - Math.Floor(value)) > float.Epsilon) {
                value *= 10.0f;
                cnt++;
            }
            return (short)((cnt << 12) + (int)value);
        }

        public static float Decode(short value) {
            int cnt = value >> 12;
            float result = value & 0xfff;
            while (cnt > 0) {
                result /= 10.0f;
                cnt--;
            }
            return result;
        }
    }
}