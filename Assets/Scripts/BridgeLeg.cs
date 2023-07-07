using CustomMath;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BridgeLeg
{
    struct VandalQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public VandalQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public VandalQuaternion(Vec3 v, float w)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            this.w = w;
        }
        public VandalQuaternion(UnityEngine.Quaternion q)
        {
            this.x = q.x;
            this.y = q.y;
            this.z = q.z;
            this.w = q.w;
        }

        private static readonly VandalQuaternion identityQuaternion = new VandalQuaternion(0f, 0f, 0f, 1f);

        public VandalQuaternion normalized
        {
            get { return Normalize(this); }
        }
        public static VandalQuaternion identity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return identityQuaternion;
            }
        }

        public const float kEpsilon = 1E-06f;

        // Es utilizado para las operaciones con matices
        public float this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return index switch
                {
                    0 => x,
                    1 => y,
                    2 => z,
                    3 => w,
                    _ => throw new IndexOutOfRangeException("Invalid Quaternion index!"),
                };
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        public float Length
        {
            get
            {
                return Mathf.Sqrt(x * x + y * y + z * z + w * w);
            }
        }
        public float LengthSquared
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        public Vec3 xyz
        {
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
            get
            {
                return new Vec3(x, y, z);
            }
        }

        public Vec3 eulerAngle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return FromQuaternionToEuler(this);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this = FormEulerToQuaternion(value);
            }

        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }
        public static float Dot(VandalQuaternion a, VandalQuaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public Vec3 FromQuaternionToEuler(VandalQuaternion rotation)
        {
            Vec3 angles;

            //(x-axis rotation)
            float SinX = 2 * (w * x + y * z);
            float CosX = 1 - 2 * (x * x + y * y);
            angles.x = Mathf.Atan2(SinX, CosX);

            //(y-axis rotation)
            float SinY = Mathf.Sqrt(1 + 2 * (w * y - x * z));
            float CosY = Mathf.Sqrt(1 - 2 * (w * y - x * z));
            angles.y = 2 * Mathf.Atan2(SinY, CosY) - MathF.PI / 2;

            //(z-axis rotation)
            float SinZ = 2 * (w * z + x * y);
            float CosZ = 1 - 2 * (y * y + z * z);
            angles.z = Mathf.Atan2(SinZ, CosZ);

            return angles;
        }

        public VandalQuaternion FormEulerToQuaternion(Vec3 euler)
        {
            float sinAngle = 0.0f;
            float cosAngle = 0.0f;

            VandalQuaternion qx = identity;
            VandalQuaternion qy = identity;
            VandalQuaternion qz = identity;
            VandalQuaternion r = identity;

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.z * 0.5f);
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.z * 0.5f);
            qz =  new VandalQuaternion(0, 0, sinAngle, cosAngle);

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.x * 0.5f);
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.x * 0.5f);
            qx = new VandalQuaternion(sinAngle, 0, 0, cosAngle);

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.y * 0.5f);
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.y * 0.5f);
            qy = new VandalQuaternion(0, sinAngle, 0, cosAngle);

            r = qy * qx * qz;

            return r;
        }

        private static Vec3 NormalizeAngles(Vec3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        public static VandalQuaternion Normalize(VandalQuaternion q)
        {
            float mag = Mathf.Sqrt(Dot(q, q));

            if (mag < Mathf.Epsilon)
                return identity;

            return new VandalQuaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        public static float Angle(VandalQuaternion a, VandalQuaternion b)
        {
            float dot = Dot(a, b);
            float dotAbs = Mathf.Abs(dot);
            return IsEqualUsingDot(dot) ? 0.0f : Mathf.Acos(Mathf.Min(dotAbs, 1.0F)) * 2.0f * Mathf.Rad2Deg;
        }

        public static VandalQuaternion AngleAxis(float angle, Vec3 axis)
        {
            Vec3 axisVec = Vec3.Normalize(axis);
            axisVec *= Mathf.Sin(angle * Mathf.Deg2Rad * 0.5f);
            return new VandalQuaternion(axisVec.x, axisVec.y, axisVec.z, Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f));
        }

        public static VandalQuaternion AxisAngle(Vec3 axis, float angle)
        {
            return AngleAxis(Mathf.Rad2Deg * angle, axis);
        }

        public static VandalQuaternion FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            Vec3 axis = Vec3.Cross(fromDirection, toDirection);
            float angle = Vec3.Angle(fromDirection, toDirection);
            return AngleAxis(angle, axis.normalized);
        }

        public static VandalQuaternion Inverse(VandalQuaternion rotation)
        {
            return new VandalQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        }

        public static VandalQuaternion LerpUnclamped(VandalQuaternion a, VandalQuaternion b, float t)
        {
            VandalQuaternion result = identity;

            float timeLeft = 1f - t;

            if (Dot(a, b) >= 0f)
            {
                result.x = (timeLeft * a.x) + (t * b.x);
                result.y = (timeLeft * a.y) + (t * b.y);
                result.z = (timeLeft * a.z) + (t * b.z);
                result.w = (timeLeft * a.w) + (t * b.w);
            }
            else
            {
                result.x = (timeLeft * a.x) - (t * b.x);
                result.y = (timeLeft * a.y) - (t * b.y);
                result.z = (timeLeft * a.z) - (t * b.z);
                result.w = (timeLeft * a.w) - (t * b.w);
            }

            return result.normalized;
        }
        public static VandalQuaternion Lerp(VandalQuaternion a, VandalQuaternion b, float t)
        {
            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;

            return LerpUnclamped(a, b, t);
        }

        private static VandalQuaternion LookRotation(Vec3 forward, Vec3 up)
        {
            forward = Vec3.Normalize(forward);
            Vec3 right = Vec3.Normalize(Vec3.Cross(up, forward));
            up = Vec3.Cross(forward, right);

            float m00 = right.x; float m01 = right.y; float m02 = right.z;
            float m10 = up.x; float m11 = up.y; float m12 = up.z;
            float m20 = forward.x; float m21 = forward.y; float m22 = forward.z;

            float diagonals = m00 + m11 + m22;
            var q = new VandalQuaternion();
            if (diagonals > 0f)
            {
                float num = Mathf.Sqrt(diagonals + 1f);
                q.w = num * 0.5f;
                num = 0.5f / num;
                q.x = (m12 - m21) * num;
                q.y = (m20 - m02) * num;
                q.z = (m01 - m10) * num;
                return q;
            }
            if (m00 >= m11 && m00 >= m22)
            {
                float num = Mathf.Sqrt(1f + m00 - m11 - m22);
                float num4 = 0.5f / num;
                q.x = 0.5f * num;
                q.y = (m01 + m10) * num4;
                q.z = (m02 + m20) * num4;
                q.w = (m12 - m21) * num4;
                return q;
            }
            if (m11 > m22)
            {
                float num = Mathf.Sqrt(1f + m11 - m00 - m22);
                float num3 = 0.5f / num;
                q.x = (m10 + m01) * num3;
                q.y = 0.5f * num;
                q.z = (m21 + m12) * num3;
                q.w = (m20 - m02) * num3;
                return q;
            }

            float num5 = Mathf.Sqrt(1f + m22 - m00 - m11);
            float num2 = 0.5f / num5;
            q.x = (m20 + m02) * num2;
            q.y = (m21 + m12) * num2;
            q.z = 0.5f * num5;
            q.w = (m01 - m10) * num2;

            return q;
        }

        public static VandalQuaternion SlerpUnclamped(VandalQuaternion a, VandalQuaternion b, float t)
        {
            if (a.LengthSquared == 0.0f)
            {
                if (b.LengthSquared == 0.0f)
                {
                    return identity;
                }
                return b;
            }
            else if (b.LengthSquared == 0.0f)
            {
                return a;
            }

            float dot = Dot(a, b);

            if (dot >= 1.0f || dot <= -1.0f)
            {
                return a;
            }
            else if (dot < 0.0f)
            {
                b.xyz = -b.xyz;
                b.w = -b.w;
                dot = -dot;
            }

            float blendA;
            float blendB;
            if (dot < 0.99f)
            {
                float halfAngle = Mathf.Acos(dot);
                float sinHalfAngle = Mathf.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = Mathf.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
                blendB = Mathf.Sin(halfAngle * t) * oneOverSinHalfAngle;
            }
            else
            {
                blendA = 1.0f - t;
                blendB = t;
            }

            VandalQuaternion result = new VandalQuaternion(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return identity;
        }

        public static VandalQuaternion Slerp(VandalQuaternion a, VandalQuaternion b, float t)
        {
            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;

            return SlerpUnclamped(a, b, t);
        }
        public static VandalQuaternion RotateTowards(VandalQuaternion from, VandalQuaternion to, float maxDegreesDelta)
        {
            float angle = Angle(from, to);
            if (angle == 0.0f) return to;
            return SlerpUnclamped(from, to, Mathf.Min(1.0f, maxDegreesDelta / angle));
        }

        public static VandalQuaternion LookRotation(Vec3 forward)
        {
            return LookRotation(forward, Vec3.Up);
        }
        public static VandalQuaternion Euler(Vec3 euler)
        {
            VandalQuaternion q = new VandalQuaternion();

            return q.FormEulerToQuaternion(euler);
        }
        public static VandalQuaternion Euler(float x, float y, float z)
        {
            Vec3 euler = new Vec3(x, y, z);
            VandalQuaternion q = new VandalQuaternion();

            return q.FormEulerToQuaternion(euler);
        }
        private static bool IsEqualUsingDot(float dot)
        {
            return dot > 1.0f - kEpsilon;
        }

        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            this = FromToRotation(fromDirection, toDirection);
        }

        public void SetLookRotation(Vec3 view, Vec3 up)
        {
            this = LookRotation(view, up);
        }
        public void SetLookRotation(Vec3 view)
        {
            this = LookRotation(view, Vec3.Up);
        }

        public void ToAxisAngle(out Vec3 axis, out float angle)
        {
            angle = 2.0f * Mathf.Acos(w);
            float mag = Mathf.Sqrt(1.0f - w * w);
            if (mag > 0.0001f)
            {
                axis = new Vec3(x, y, z) / mag;
            }
            else
            {
                axis = new Vec3(1, 0, 0);
            }
        }
        public void ToAngleAxis(out float angle, out Vec3 axis)
        {
            ToAxisAngle(out axis, out angle);
            angle *= Mathf.Rad2Deg;
        }
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
        }

        public static VandalQuaternion operator *(VandalQuaternion lhs, VandalQuaternion rhs)
        {
            return new VandalQuaternion(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        public static Vec3 operator *(VandalQuaternion rotation, Vec3 point)
        {
            VandalQuaternion p = new VandalQuaternion(point.x, point.y, point.z, 0);
            VandalQuaternion p2 = (rotation * p) * Inverse(rotation);
            Vec3 res = new Vec3(p2.x, p2.y, p2.z);
            return res;
        }

        public static bool operator ==(VandalQuaternion lhs, VandalQuaternion rhs)
        {
            return IsEqualUsingDot(Dot(lhs, rhs));
        }

        public static bool operator !=(VandalQuaternion lhs, VandalQuaternion rhs)
        {
            return !(lhs == rhs);
        }
    }

}
