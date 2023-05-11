using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace CustomMath
{
    public struct Vec3 : IEquatable<Vec3>
    {
        #region Variables
        public float x;
        public float y;
        public float z;

        // la función sqrMagnitude devuelve el valor de la magnitud al cuadrado de un vector,
        // que se utiliza comúnmente en programación para cálculos de vectores eficientes.
        public float sqrMagnitude { get { return (x * x + y * y + z * z); } }

        // La magnitud de un vector es un escalar no negativa que representa la distancia entre
        // el origen del sistema de coordenadas y el punto final del vector.
        public float magnitude { get { return Mathf.Sqrt(x * x + y * y + z * z); } }

        // Normalizar un vector significa que se ajusta su magnitud a 1 mientras se mantiene su dirección original.
        public Vec3 normalized 
        { 
            get 
            { 
                float mag = magnitude;

                Vec3 Length = new Vec3(x / mag, y / mag, z / mag);

                return Length; 
            
            } 
        }

        #endregion

        #region constants
        public const float epsilon = 1e-05f;
        #endregion

        #region Default Values
        public static Vec3 Zero { get { return new Vec3(0.0f, 0.0f, 0.0f); } }
        public static Vec3 One { get { return new Vec3(1.0f, 1.0f, 1.0f); } }
        public static Vec3 Forward { get { return new Vec3(0.0f, 0.0f, 1.0f); } }
        public static Vec3 Back { get { return new Vec3(0.0f, 0.0f, -1.0f); } }
        public static Vec3 Right { get { return new Vec3(1.0f, 0.0f, 0.0f); } }
        public static Vec3 Left { get { return new Vec3(-1.0f, 0.0f, 0.0f); } }
        public static Vec3 Up { get { return new Vec3(0.0f, 1.0f, 0.0f); } }
        public static Vec3 Down { get { return new Vec3(0.0f, -1.0f, 0.0f); } }
        public static Vec3 PositiveInfinity { get { return new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }
        public static Vec3 NegativeInfinity { get { return new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }
        #endregion                                                                                                                                                                               

        #region Constructors
        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vec3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(UnityEngine.Vector3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector2 v2)
        {
            this.x = v2.x;
            this.y = v2.y;
            this.z = 0.0f;
        }
        #endregion
        #region Operators
        public static bool operator ==(Vec3 left, Vec3 right)
        {
            float diff_x = left.x - right.x;
            float diff_y = left.y - right.y;
            float diff_z = left.z - right.z;
            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z;
            return sqrmag < epsilon * epsilon;
        }

        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !(left == right);
        } 

        public static Vec3 operator +(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x + rightV3.x, leftV3.y + rightV3.y, leftV3.z + rightV3.z);
        }
        public static Vec3 operator -(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x - rightV3.x, leftV3.y - rightV3.y, leftV3.z - rightV3.z);
        }

        public static Vec3 operator -(Vec3 v3)
        {
            return new Vec3(0f - v3.x, 0f - v3.y, 0f - v3.z);
        }

        public static Vec3 operator *(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }
        public static Vec3 operator *(float scalar, Vec3 v3)
        {
            return new Vec3(scalar * v3);
        }

        
        public static Vec3 operator /(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x / scalar, v3.y / scalar, v3.z / scalar);
        }

        public static implicit operator Vector3(Vec3 v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }
        

        public static implicit operator Vector2(Vec3 v2)
        {
            return new Vector2(v2.x, v2.y);
        }
        #endregion


        #region Functions
        public override string ToString()
        {
            return "X = " + x.ToString() + "   Y = " + y.ToString() + "   Z = " + z.ToString();
        }

        //Se utiliza para calcular el ángulo entre dos vectores (siempre devuelve un valor positivo).
        public static float Angle(Vec3 from, Vec3 to)
        {
            return Mathf.Acos(Vec3.Dot(from.normalized, to.normalized)* 180 / Mathf.PI);
        }

        //se utiliza para limitar la magnitud de un vector tridimensional a un valor máximo.
        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength)
        {
            if(vector.magnitude > maxLength)
            {
                return vector.normalized * maxLength;
            }
            else
            {
                return vector;
            }
        }

        //Se utiliza para calcular la magnitud o longitud de un vector.
        public static float Magnitude(Vec3 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        //Es para determinar la dirección de una normal a un plano en el espacio tridimensional.
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            float i; 
            float j; 
            float k;

            i = (a.y * b.z) - (a.z * b.y);
            j = (a.x * b.z) - (a.z * b.x);
            k = (a.x * b.y) - (a.y * b.x);

            return new Vec3(i, -j, k);
        }

        //Se utiliza para calcular la distancia euclidiana entre dos puntos tridimensionales en el espacio.
        //(La distancia euclidiana es la distancia más corta entre dos puntos en una línea recta en el espacio tridimensional).
        public static float Distance(Vec3 a, Vec3 b)
        {
            float distanceX;
            float distanceY;
            float distanceZ;

            distanceX = a.x - b.x;  
            distanceY = a.y - b.y;
            distanceZ = a.z - b.z;

            return (Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2) + Mathf.Pow(distanceZ, 2)));
        }

        //El producto punto es un operador matemático que produce un valor escalar y, por lo tanto,
        //es útil para determinar la magnitud de un vector proyectado en una dirección particular.
        public static float Dot(Vec3 a, Vec3 b)
        {
            return ((a.x * b.x) + (a.y * b.y) + (a.z * b.z));
        }

        //Se utiliza para realizar una interpolación lineal entre dos vectores tridimensionales.
        //(La interpolación lineal es un método para encontrar un valor intermedio entre dos puntos mediante una línea recta que los une).
        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            if(t < 0) 
            {
                t = 0;
            }
            else if (t > 1)
            {
                t = 1;
            }

            return (a + (b - a) * t);
        }

        //Realiza una interpolación lineal entre dos vectores tridimensionales.
        //pero no está limitada a los valores de interpolación entre 0 y 1.
        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            return (a + (b - a) * t);
        }

        //Devuelve un vector que contiene el valor máximo para cada componente de los dos vectores de entrada.
        public static Vec3 Max(Vec3 a, Vec3 b)
        {
            float maxX = 0;
            if(a.x > b.x)
            {
                maxX = a.x;
            }
            else
            {
                maxX = b.x;
            }

            float maxY = 0;
            if (a.y > b.y)
            {
                maxY = a.y;
            }
            else
            {
                maxY = b.y;
            }

            float maxZ = 0;
            if (a.z > b.z)
            {
                maxZ = a.z;
            }
            else
            {
                maxZ = b.z;
            }

            return new Vec3(maxX, maxY, maxZ);
        }

        //Devuelve un vector que contiene el valor minimo para cada componente de los dos vectores de entrada.
        public static Vec3 Min(Vec3 a, Vec3 b)
        {
            float minX = 0;
            if (a.x < b.x)
            {
                minX = a.x;
            }
            else
            {
                minX = b.x;
            }

            float minY = 0;
            if (a.y < b.y)
            {
                minY = a.y;
            }
            else
            {
                minY = b.y;
            }

            float minZ = 0;
            if (a.z < b.z)
            {
                minZ = a.z;
            }
            else
            {
                minZ = b.z;
            }

            return new Vec3(minZ, minY, minZ);
        }

        //La función sqrMagnitude devuelve el valor de la magnitud al cuadrado de un vector,
        //que se utiliza comúnmente en programación para cálculos de vectores eficientes.
        public static float SqrMagnitude(Vec3 vector)
        {
            return (Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2) + Mathf.Pow(vector.z, 2));
        }

        //La proyección de un vector sobre otro vector da como resultado la magnitud del primer vector en la dirección del segundo vector.
        public static Vec3 Project(Vec3 vector, Vec3 onNormal) 
        {
            float sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < epsilon)
            {
                return Zero;
            }
            else
            {
                float dot = Dot(vector, onNormal);
                return onNormal * dot / sqrMag;
            }
        }

        //Se utiliza para reflejar un vector tridimensional sobre un plano definido por una normal.
        //La reflexión de un vector sobre un plano da como resultado un nuevo vector que apunta en la dirección opuesta
        //pero que se encuentra a la misma distancia del plano que el vector original.
        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal) 
        {
            return inDirection - 2 * (Dot(inDirection, inNormal)) * inNormal;
        }

        //Se utiliza para setear nuevos valores a un vector.
        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        //Se utiliza para escalar un vector componente por componenete
        public void Scale(Vec3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        //Convierte un vector en un vector de longitud 1, manteniendo su dirección original. 
        public static Vec3 Normalize(Vec3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return value / num;
            }

            return Zero;
        }
        #endregion

        #region Internals
        public override bool Equals(object other)
        {
            if (!(other is Vec3)) return false;
            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        #endregion
    }
}