using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace CustomMath
{
    [Serializable]
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
            // Noramaliza los vectores para elimianr sus magnitudes y que afecten el resultado
            // Se calcula el producto punto, para obtener el coseno del angulo entre los vectores
            // Apartir de ese resultado, se obtiene el arco coseno para obtener el angulo en radianes
            // y por ultimo se multiplica por 180 / PI para pasarlo a grados
            
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
            // eleva cada componente al cuadrado para que las distancias siempre den positivas
            // Suma el cuadrado de cada eje para obtener la suma de las distancias al cuadrado
            // y por ultimo se aplica la raiz cuadra del resultado para revertir el proceso del cuadrado previamente realizado
            
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        //Es para determinar la dirección de una normal a un plano en el espacio tridimensional.
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            float i; 
            float j; 
            float k;
            
            //La formula para calcular el producto cruz entre 2 vectores es la sigueinte : 
            
            i = (a.y * b.z) - (a.z * b.y); // i : el resultado de como Z e Y interactuan entre si
            j = (a.x * b.z) - (a.z * b.x); // j : el resultado de como X y Z interactuan entre si
            k = (a.x * b.y) - (a.y * b.x); // k : el resultado de como X e Y interactuan entre si

            //El resultado es un vector perpendicular a ambos vectores originales
            return new Vec3(i, -j, k);
            // nota : -j es para mantener la direccion correcta del resultado dentro del espacio tridimencional
        }

        //Se utiliza para calcular la distancia euclidiana entre dos puntos tridimensionales en el espacio.
        //(La distancia euclidiana es la distancia más corta entre dos puntos en una línea recta en el espacio tridimensional).
        public static float Distance(Vec3 a, Vec3 b)
        {
            float distanceX;
            float distanceY;
            float distanceZ;

            // calcula la diferencia entre ambos vectores para calcular la distancia
            distanceX = a.x - b.x;  
            distanceY = a.y - b.y;
            distanceZ = a.z - b.z;

            //luego eleva el resultado al cuadrado para que el resultado sea postivio
            //Y por ultimo aplica la raiz cuadrada al resultado para revertir el proceso anterior
            return (Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2) + Mathf.Pow(distanceZ, 2)));
        }

        //El producto punto es un operador matemático que produce un valor escalar y, por lo tanto,
        //es útil para determinar la magnitud de un vector proyectado en una dirección particular.
        public static float Dot(Vec3 a, Vec3 b)
        {
            // El producto punto o escalar indica la relacion entre 2 vectores
            // basandose en su mangnitud y direccion
            
            
            // Se multiplica los componentes de uno sobre otro, esto proyecta la magnitud de un vector sobre el otro
            // Luego se suma para mostrar el cambio total en la direccion de los vectores.
            return ((a.x * b.x) + (a.y * b.y) + (a.z * b.z));
            
            // Si el valor es positivo, es que tiene una direccion similar
            // Si el valor es negativo, es que estan orientados en direcciones opuestas
            // Si el valor es cero, es que son perpendiculares entre si (estan a 90 grados uno del otro)
        }

        //Se utiliza para realizar una interpolación lineal entre dos vectores tridimensionales.
        //(La interpolación lineal es un método para encontrar un valor intermedio entre dos puntos mediante una línea recta que los une).
        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            // podriamos interpertar como a = el punto de inicio, b = el destino y t = el tiempo transcurrido
            
            // Clampea t entre 0 y 1 para que el resultado no exeda del vector target (b)
            t = Mathf.Clamp(t, 0, 1);

            // se saca la diferencia entre  b y a par obtener un vector de direccion que apunta de a hacia b
            // luego se multiplica por t (tiempo) para determinar el valor intermedio en el que se encuentra este vector
            // teniendo en cuenta que 0 es igual a el punto de inicio (a) y 1 es el destino (b)
            // finalmente el resultado lo sumamos con el vector inicial y esto lo moveria hacia el vector final en base al tiempo
            return (a + (b - a) * t);
        }

        //Realiza una interpolación lineal entre dos vectores tridimensionales.
        //pero no está limitada a los valores de interpolación entre 0 y 1.
        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            // podriamos interpertar como a = el punto de inicio, b = el destino y t = el tiempo transcurrido
            
            // se saca la diferencia entre  b y a par obtener un vector de direccion que apunta de a hacia b
            // luego se multiplica por t (tiempo) para determinar el valor intermedio en el que se encuentra este vector
            // teniendo en cuenta que 0 es igual a el punto de inicio (a) y 1 es el destino (b)
            // finalmente el resultado lo sumamos con el vector inicial y esto lo moveria hacia el vector final en base al tiempo
            return (a + (b - a) * t);
            
            // nota : al t no esta clampeado este vector resultante puede excederse del target, 
            // si los 2 vectores los ubicamos dentro de un circulo, este vector resultante podria dar la vuelta completa
        }

        //Devuelve un vector que contiene el valor máximo para cada componente de los dos vectores de entrada.
        public static Vec3 Max(Vec3 a, Vec3 b)
        {
            float maxX = 0;
            if(a.x > b.x)
                maxX = a.x;
            else
                maxX = b.x;

            float maxY = 0;
            if (a.y > b.y)
                maxY = a.y;
            else
                maxY = b.y;

            float maxZ = 0;
            if (a.z > b.z)
                maxZ = a.z;
            else
                maxZ = b.z;

            return new Vec3(maxX, maxY, maxZ);
        }

        //Devuelve un vector que contiene el valor minimo para cada componente de los dos vectores de entrada.
        public static Vec3 Min(Vec3 a, Vec3 b)
        {
            float minX = 0;
            if (a.x < b.x)
                minX = a.x;
            else
                minX = b.x;

            float minY = 0;
            if (a.y < b.y)
                minY = a.y;
            else
                minY = b.y;

            float minZ = 0;
            if (a.z < b.z)
                minZ = a.z;
            else
                minZ = b.z;

            return new Vec3(minX, minY, minZ);
        }

        //La función sqrMagnitude devuelve el valor de la magnitud al cuadrado de un vector,
        //que se utiliza comúnmente en programación para cálculos de vectores eficientes.
        public static float SqrMagnitude(Vec3 vector)
        {
            // util para cuando se requier comparar magnitudes ya qye es mas eficiente
            
            // Se elevea cada componente al cuadrado para eliminar el signo
            // y al final se suman todos los componentes para devolver la magnitud al cuadrado
            return (Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2) + Mathf.Pow(vector.z, 2));
        }

        //La proyección de un vector sobre otro vector da como resultado la magnitud del primer vector en la dirección del segundo vector.
        public static Vec3 Project(Vec3 vector, Vec3 onNormal) 
        {
            // esto calcula el cuadrado de la magnitud, se hace con el producto punto porque es mas eficeinte
            float sqrMag = Dot(onNormal, onNormal);
            
            // Si la magnitud es demaciado chica, returno un vector zero ya que no tiene una direccion significativa
            if (sqrMag < epsilon)
            {
                return Zero;
            }
            else
            {
                // conseguimos el producto punto entre el vector que queremos proyectar con la normal,
                // esto marca que tanto esta alineado este vector con la normal
                float dot = Dot(vector, onNormal);
                
                // Y al multiplicar la normal por el producto punto y la magnitud da como resultado
                // un vector con la misma longitud que el vector inicial, hacia la direccion de la normal
                return onNormal * dot / sqrMag;
            }
        }

        //Se utiliza para reflejar un vector tridimensional sobre un plano definido por una normal.
        //La reflexión de un vector sobre un plano da como resultado un nuevo vector que apunta en la dirección opuesta
        //pero que se encuentra a la misma distancia del plano que el vector original.
        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal) 
        {
            // el producto punto mide cuanto estan alineados estos vectores
            // luego multiplicamos el resultado por 2 porque al ser una reflexion simetrica, se busca duplicar el componente
            // se vuelve a multiplicar por la normal para ajustar la magnitud del componente reflejado
            // y por ultimo se resta a la direccion ya que queremos que vaya en direccion opuesta
            
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