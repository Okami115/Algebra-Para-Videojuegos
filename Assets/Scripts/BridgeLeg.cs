using CustomMath;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BridgeLeg
{
    /// <summary>
    /// Un Quaternion es la reprecentacion de una rotacion en el espacio y se compone por 3 numeros complejos y uno real
    /// los numeros complejos estan compuestos por un numero complejo y uno real (Xi, Yj, Zk) y guarda la representacion del seno del objeto con respecto al espacio o a su padre.
    /// 
    /// Y W (La aparte real de un quaternion) es la distancia al punto de origen del objeto y al ser una distancia, siepre es positivo.
    /// </summary>
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

        // Noramliza angulos entre 0 y 360 grados
        private static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }
        
        // Calcula el producto escalar entre 2 quaterniones
        public static float Dot(VandalQuaternion a, VandalQuaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        // atraves de la trigonometria se calcula los grados euler de un quaternion
        public Vec3 FromQuaternionToEuler(VandalQuaternion rotation)
        {
            Vec3 angles;

            // Basicament lo que intentamos hacer es calcular la hipotenusa de un triangulo en 2D
            // para luego usarlo como cateto para clacula, junto con la profundidad, la hipotenusa del tringulo completo en 3D
            // y ya con el triangulo 3D formado podemos calcular el angulo de rotacion en euler.

            //(x-axis rotation)
            float SinX = 2 * (w * x + y * z); // Y y Z se calculan para saber cuanto afecta esos valores sobre X
            float CosX = 1 - 2 * (x * x + y * y);// 1- porque es el quaternion normalizado, el 2 porque se mezcla con la cantidad dimenciones
            angles.x = Mathf.Atan2(SinX, CosX);// resulta en la rotacion del eje en X

            //(y-axis rotation) //Se hace de esta forma para evitar un gimbal lock, ya que si X y Z se perdieran se pueden recuperar bajo la regla de
            // que los ejes estan a 90 grados entre si.
            float SinY = Mathf.Sqrt(1 + 2 * (w * y - x * z));
            float CosY = Mathf.Sqrt(1 - 2 * (w * y - x * z));
            angles.y = 2 * Mathf.Atan2(SinY, CosY) - MathF.PI / 2; // Luego se realiza la operacion con pi para alinearlo con el resto de los ejes a 90 grados

            //(z-axis rotation) // Se calcula igual que X
            float SinZ = 2 * (w * z + x * y);
            float CosZ = 1 - 2 * (y * y + z * z);
            angles.z = Mathf.Atan2(SinZ, CosZ);// resulta en la rotacion del eje en Z

            return angles; //devuelve los angulos euler
        }

        // compone un quaternion a partir de los angulos euler
        public VandalQuaternion FormEulerToQuaternion(Vec3 euler)
        {
            float sinAngle = 0.0f;
            float cosAngle = 0.0f;

            VandalQuaternion qx = identity;
            VandalQuaternion qy = identity;
            VandalQuaternion qz = identity;
            VandalQuaternion r = identity;

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.z * 0.5f);// Se calcula la parte imaginaria (Z)
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.z * 0.5f);// Y se calcula la parte real W
            qz =  new VandalQuaternion(0, 0, sinAngle, cosAngle);

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.x * 0.5f);// Se calcula la parte imaginaria (X)
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.x * 0.5f);// Y se calcula la parte real W
            qx = new VandalQuaternion(sinAngle, 0, 0, cosAngle);

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.y * 0.5f);// Se calcula la parte imaginaria (Y)
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.y * 0.5f);// Y se calcula la parte real W
            qy = new VandalQuaternion(0, sinAngle, 0, cosAngle);

            // Se multiplica de esta manera (Con la Y al principio)
            r = qy * qx * qz;

            return r;
        }

        // Noramliza los angulos de los quaterniones
        private static Vec3 NormalizeAngles(Vec3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        // Normaliza un quaternion, es decir que su longitud va a ser 1, pero mantiene su direccion
        public static VandalQuaternion Normalize(VandalQuaternion q)
        {
            float mag = Mathf.Sqrt(Dot(q, q));

            if (mag < Mathf.Epsilon)
                return identity;

            return new VandalQuaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        // Saca el angulo entre 2 quaterniones
        public static float Angle(VandalQuaternion a, VandalQuaternion b)
        {
            float dot = Dot(a, b);
            // Se saca el valor absoluto porque los angulos siempre son positivos
            float dotAbs = Mathf.Abs(dot);
            // Luego calcula el angulo entre los 2 quaterniones y se lo multiplica por 2 por la cantidad de dimenciones en las que trabajamos
            return IsEqualUsingDot(dot) ? 0.0f : Mathf.Acos(Mathf.Min(dotAbs, 1.0f)) * 2.0f * Mathf.Rad2Deg;
        }

        // Rota un quaternion sobre un eje en especifio y una cantidad de grados especificos
        public static VandalQuaternion AngleAxis(float angle, Vec3 axis)
        {
            // primero se normaliza el eje de rotacion sobre la cual se va a rotar el quaternion
            Vec3 axisVec = Vec3.Normalize(axis);
            // Se calcula la parte imaginaria de rotacion en base al eje
            axisVec *= Mathf.Sin(angle * Mathf.Deg2Rad * 0.5f);
            return new VandalQuaternion(axisVec.x, axisVec.y, axisVec.z, Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f)); // Y luego con los angulos se calcula la parte real del quaternion
        }

        public static VandalQuaternion AxisAngle(Vec3 axis, float angle)
        {
            return AngleAxis(Mathf.Rad2Deg * angle, axis);
        }

        // Crea un quaternion que representa la rotacion de un vector sobre otro
        public static VandalQuaternion FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            //Primero saca el eje de rotacion sobre la cual se va a rotar un vector sobre otro
            Vec3 axis = Vec3.Cross(fromDirection, toDirection);
            //Luego se calcula el angulo entre estos 2 vectores.
            float angle = Vec3.Angle(fromDirection, toDirection);
            //Y se rota devuelve un quaternion que almacena la rotacion necesaria para que un vector quede sobre otro.
            return AngleAxis(angle, axis.normalized);
        }

        public static VandalQuaternion Inverse(VandalQuaternion rotation)
        {
            //Simplemenete niega las partes imaginarias del quaternion para lograr invertir la direccion de la rotacion.
            return new VandalQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        }

        // realiza una superposicion lineal de un quaternion sobre otro atravez del tiempo.
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

        //Esta funcion orienta un objeto hacie el forward del mundo.
        private static VandalQuaternion LookRotation(Vec3 forward, Vec3 up)
        {
            //Aca mantiene la relacion entre los ejes
            forward = Vec3.Normalize(forward);
            Vec3 right = Vec3.Normalize(Vec3.Cross(up, forward));
            up = Vec3.Cross(forward, right);

            //Inicializa una matriz 3x3 con la rotacion de los 3 ejes.
            float m00 = right.x; float m01 = right.y; float m02 = right.z;
            float m10 = up.x; float m11 = up.y; float m12 = up.z;
            float m20 = forward.x; float m21 = forward.y; float m22 = forward.z;

            // define la diagonal
            float diagonals = m00 + m11 + m22;
            var q = new VandalQuaternion();

            // Eleji que eje de rotacion uso para rota y evitar el gimbal lock
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

        // Realiza una interpolacion circular entre 2 quaterniones a lo largo del tiempo
        public static VandalQuaternion SlerpUnclamped(VandalQuaternion a, VandalQuaternion b, float t)
        {
            // chequea que los valores sea valido y que se puedatrabajar con ellos
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

            // Luego se chequea que los quaternoiones no sean similares.
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

            float blendA; // Representa la influencia sobre la rotacion a aplicar sobre XYZ.
            float blendB; // Representa la influencia sobre la rotacion a aplicar sobre XYZ.
            if (dot < 0.99f)
            {
                //Luego se calcula el angulo entre los quaterniones
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

        // Esta funcion rota un quaternion una cierta cantidad de grados en intervalos
        public static VandalQuaternion RotateTowards(VandalQuaternion from, VandalQuaternion to, float maxDegreesDelta)
        {
            //Primero calcula el angulo entre los 2 quaterniones, si el angulo es 0, significa que los 2 son identicos
            float angle = Angle(from, to);
            if (angle == 0.0f) return to;
            
            // Luego se realiza una interpolacion circular entre el quaternion "from" y el quaternion "to"
            // Y se hace en intervalos de los angulos que se hayan pasado como parametros hasta llegar a "to"
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

        // extrae el eje de rotacion del quaternion
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
            //Se aplica distributiba para conseguir la multiplicacion de 2 quaterniones siguiendo la formula de hamilton
            return new VandalQuaternion(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        // Rota un punto con un cuaternion 
        public static Vec3 operator *(VandalQuaternion rotation, Vec3 point)
        {
            // se crea este quaternion sin valor en W que es la representacion de un vector dentro del quaternion 
            VandalQuaternion p = new VandalQuaternion(point.x, point.y, point.z, 0);
            //Luego se mutiplican esto quaterniones
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
