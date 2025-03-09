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
        #region Variables
        public float x;
        public float y;
        public float z;
        public float w;

        public const float kEpsilon = 1E-06f;
        #endregion
        
        #region Constructors
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

        #endregion
        public VandalQuaternion normalized
        {
            get { return Normalize(this); }
        }
        public static VandalQuaternion identity
        {
            // esta es una directiva que le indica al compilador que inserte una linea que mejora el rendimiento
            // que en lugar de llamar a la funcion, inserte el valor directamente
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // retorna identeity que es un quaternion sin ninguna rotacion
                return identityQuaternion;
            }
        }
        
        // Es utilizado para las operaciones con matices
        public float this[int index]
        {
            // esta funcion crea una indexadora para manejar la estructura del quaternion como una matriz 4x1
            // lo que mejora el rendimiento al realizar este tipo de calculos y evita llamados inecesarios
            
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
            // retorna la longitud total del cuaternion
            // suma los cuadrados de los componentes para que sean positivos
            // luego aplica la raiz cuadrad para revertir el proceso y obtener la longitud normal
            get
            {
                return Mathf.Sqrt(x * x + y * y + z * z + w * w);
            }
        }
        public float LengthSquared
        {
            // retorna la longitud al cuadrado del cuaternion
            // suma los cuadrados de los componentes para que sean positivos
            // En este caso no se aplica la raiz cuadrada para que no perder rendimiento
            get
            {
                return x * x + y * y + z * z + w * w;
            }
            
            // nota : esto se utiliza para comparar magnitudes rapidamente
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
            // los bucles estan para reducir los excesos de los angulos y mantenerlos de manera proporcional entre un rango de 360 y 0
            // ejemplo : -90 = 270 o 720 = 360
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }
        
        // Calcula el producto escalar entre 2 quaterniones
        public static float Dot(VandalQuaternion a, VandalQuaternion b)
        {
            // multiplica los componentes de a con b para medir su similitud
            // luego se suman todos los componentes para obtener un float
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
            
            // nota : si el resultado es igual a 1 significa que los quaterniones son identicos
            //        si el resultado es igual a 0 significa que son ortogonales (estan uno a 90 grados del otro)
            //        si el resultado es negativo significa que son opuestos
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
            // variables auxiliares que almacenna temporalemnte el seno y coseno de los angulos
            float sinAngle = 0.0f;
            float cosAngle = 0.0f;

            // Se inicializa cada quaternions
            VandalQuaternion qx = identity;
            VandalQuaternion qy = identity;
            VandalQuaternion qz = identity;
            VandalQuaternion r = identity;

            // combiente el angulo en radianas y calcula el ceno y coseno
            // esto se multiplica por 0.5f porque se utiliza la mitad del angulo en la formula de los quaterniones
            // por la cantida de dimenciones con las que trabaja
            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.z * 0.5f);
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.z * 0.5f);
            qz =  new VandalQuaternion(0, 0, sinAngle, cosAngle);

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.x * 0.5f);
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.x * 0.5f);
            qx = new VandalQuaternion(sinAngle, 0, 0, cosAngle);

            sinAngle = Mathf.Sin(Mathf.Deg2Rad * euler.y * 0.5f);
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * euler.y * 0.5f);
            qy = new VandalQuaternion(0, sinAngle, 0, cosAngle);
            
            // El orden de la multiplicacion afecta al resultado de la rotacion
            // este se hace de esta manera ya que es un forma comun de ordenar los ejes
            // y se multiplica para combinar todas las rotaciones en un solo quaternion
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
            // Esto normaliza un quaternion asegurando su magnitud en 1
            
            // con esto se obtiene la magnitud real del quaternion
            // se calcula el producto escalar del quaternnion para elevarlo al cuadrado para eliminar
            // esto se hace con esta funcion ya que es mas eficiente
            // y se realiza la raiz cuadrada del resultada para revertir el cuadrado y onbtener la magnitud normal
            float mag = Mathf.Sqrt(Dot(q, q));

            // si la magnitud es muy cercana a 0, devuelve el un quaternion sin rotacion ya que la magnitud es irrelevante
            if (mag < Mathf.Epsilon)
                return identity;

            // y al final se divide cada componente por la magnitud, para que la longitud del resultado sea 1
            return new VandalQuaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        // Saca el angulo entre 2 quaterniones
        public static float Angle(VandalQuaternion a, VandalQuaternion b)
        {
            // obtiene el producto punto entre estos 2 quaterniones para evitar calculos inencesarios
            float dot = Dot(a, b);
            // Se obtiene el valor absoluto para evitar ambiguedades ya que pueden ser opuestos pero equivalentes
            float dotAbs = Mathf.Abs(dot);
            // si el producto punto es muy cercano a 1, de vuelve la rotacion 0 ya que son muy identicos
            // sino dotAbs evita que sea mayor a 1 para no tener errores con el arco coseno
            // Luego calcula el angulo en radianes y se multiplica el angulo por 2 ya que el angulo real es el doble del angulo obtenido
            // y por ultimo se multiplica por Rad2Deg para obtener el angulo en grados
            return IsEqualUsingDot(dot) ? 0.0f : Mathf.Acos(Mathf.Min(dotAbs, 1.0f)) * 2.0f * Mathf.Rad2Deg;
        }

        // Rota un quaternion sobre un eje en especifio y una cantidad de grados especificos
        public static VandalQuaternion AngleAxis(float angle, Vec3 axis)
        {
            // noramlizamos el eje de rotacion para evitar rotaciones erradas causadas por su magnitud
            Vec3 axisVec = Vec3.Normalize(axis);
            // Esta es la formula de rotacion de un quaternion
            // pasa los angulos a radianes, lo divide por 2 para obtener la mitad del angulo real
            // y luego se multiplica para obtener el vector nomalizado, rotado
            axisVec *= Mathf.Sin(angle * Mathf.Deg2Rad * 0.5f);
            
            // Con esto ya tenemos los valores de x, y, z pero para calcular w se requiere el coseno del angulo en radianes divido 2
            return new VandalQuaternion(axisVec.x, axisVec.y, axisVec.z, Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f));
        }

        public static VandalQuaternion AxisAngle(Vec3 axis, float angle)
        {
            return AngleAxis(Mathf.Rad2Deg * angle, axis);
        }

        // Crea un quaternion que representa la rotacion mas corta de un vector sobre otro
        public static VandalQuaternion FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            // se utiliza para encontrar un vector perpendicular entre ambos que sea el eje de rotacion
            Vec3 axis = Vec3.Cross(fromDirection, toDirection);
            // Se calcula el angulo entre los 2 vectores para determina cuanto tiene que rotar par que esten alineados
            float angle = Vec3.Angle(fromDirection, toDirection);
            // Luego se llama a esta funcion para que devuelva un quaternion que represente la rotacion necesaria en base al eje
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
            // inicializa el quaternion resultante
            VandalQuaternion result = identity;

            // calcula el tiempo restante para que termine la interpolacion
            float timeLeft = 1f - t;
           
            // el producto punto se utiliza para verificar si los quaterniones estan apuntando hacia la misma direccion
            if (Dot(a, b) >= 0f)
            {
                // en caso de que el resultado se apositivo o 0, la interpolacion se hace sobre los mismo quaterniones
                
                // esto es una interpolacion lineal entre sus componentes
                result.x = (timeLeft * a.x) + (t * b.x);
                result.y = (timeLeft * a.y) + (t * b.y);
                result.z = (timeLeft * a.z) + (t * b.z);
                result.w = (timeLeft * a.w) + (t * b.w);
            }
            else
            {
                // en este caso, los quaterniones estan orientados en direcciones opuestas
                // asi que se resstan para invertirlos
                
                result.x = (timeLeft * a.x) - (t * b.x);
                result.y = (timeLeft * a.y) - (t * b.y);
                result.z = (timeLeft * a.z) - (t * b.z);
                result.w = (timeLeft * a.w) - (t * b.w);
            }

            return result.normalized;
        }
        public static VandalQuaternion Lerp(VandalQuaternion a, VandalQuaternion b, float t)
        {
            // clampea el valor t entre 0 y 1
            Mathf.Clamp(t, 0, 1);

            return LerpUnclamped(a, b, t);
        }

        //Esta funcion orienta un objeto hacie el forward del mundo.
        private static VandalQuaternion LookRotation(Vec3 forward, Vec3 up)
        {
            // normaliza el vector para que su lingitud sea 1 y no distorcione las rotaciones
            forward = Vec3.Normalize(forward);
            // obtiene un vector perpendicular a up y foward, osea right y lo normaliza
            Vec3 right = Vec3.Normalize(Vec3.Cross(up, forward));
            // aca se asegura que up este perpendicular a foward y a right
            up = Vec3.Cross(forward, right);
            // aca todos los ejes son perpendiculares entre si
            
            // aca se construye una matriz de rotacion 3x3 
            // cada uno de los componentes de los vectores se convierten en una posicion de la matriz 
            float m00 = right.x; float m01 = right.y; float m02 = right.z;
            float m10 = up.x; float m11 = up.y; float m12 = up.z;
            float m20 = forward.x; float m21 = forward.y; float m22 = forward.z;
            
            // el calculo de de quaternion depende de la diagonal ya que depende del resultado se usan distintas formulas
            float diagonals = m00 + m11 + m22;
            // se inicializa un nuevo quaternion
            var q = new VandalQuaternion();
            
            // realizar calculos de quaternions en base a la diagonal, es muy eficiente
            if (diagonals > 0f)
            {
                // si la diagonal es positiva se calquica el quaternion de manera directa
                
                // esta formula se utiliza para obtener la w en base a la digonal +1
                float num = Mathf.Sqrt(diagonals + 1f);
                // se divide por 2 por que los quaterniones guardan la mitad de los angulos por la cantidad de dimenciones con las que trabaja
                q.w = num * 0.5f;
                
                num = 0.5f / num;
                q.x = (m12 - m21) * num;
                q.y = (m20 - m02) * num;
                q.z = (m01 - m10) * num;
                return q;
            }
            if (m00 >= m11 && m00 >= m22)
            {
                // si m00 es el mayor, la formula se adapta a este valor para que sea un calculo mas eficiente
                // donde q.x se puede calcula ya que m00 es el valor con mayor peso en la diagonal
                float num = Mathf.Sqrt(1f + m00 - m11 - m22);
                // en base a este resultado se puede obtener el calculo del resto de componentes fuera de la diagonal
                float num4 = 0.5f / num;
                q.x = 0.5f * num;
                q.y = (m01 + m10) * num4;
                q.z = (m02 + m20) * num4;
                q.w = (m12 - m21) * num4;
                return q;
            }
            if (m11 > m22)
            {
                // mismo caso que el anterio pero el componente de la digonal con mas peso es m11
                float num = Mathf.Sqrt(1f + m11 - m00 - m22);
                float num3 = 0.5f / num;
                q.x = (m10 + m01) * num3;
                q.y = 0.5f * num;
                q.z = (m21 + m12) * num3;
                q.w = (m20 - m02) * num3;
                return q;
            }

            // mismo caso que el anterio pero el componente de la digonal con mas peso es m22
            float num5 = Mathf.Sqrt(1f + m22 - m00 - m11);
            float num2 = 0.5f / num5;
            q.x = (m20 + m02) * num2;
            q.y = (m21 + m12) * num2;
            q.z = 0.5f * num5;
            q.w = (m01 - m10) * num2;

            return q;
            
            // nota : todo esto espara que el calculo de quaterniones sea lo mas eficiente posible
            // nota : todo esto tambien saltea el proble del gimbal lock al utilizar matrices en el calculo 
        }

        // Realiza una interpolacion circular entre 2 quaterniones a lo largo del tiempo
        public static VandalQuaternion SlerpUnclamped(VandalQuaternion a, VandalQuaternion b, float t)
        {
            // verifica si uno de los quaterniones tiene una logitud de 0
            // en ese caso devuelve el opuesto, si ambos lo son, devuelve un quaternion sin rotacion
            if (a.LengthSquared == 0.0f)
            {
                if (b.LengthSquared == 0.0f)
                    return identity;
                
                return b;
            }
            
            if (b.LengthSquared == 0.0f)
                return a;

            
            //calcula el producto punto entre los 2 cuaquerniones para verificar su similitud
            float dot = Dot(a, b);
            
            // si esto es mayero a 1 significa que son identicos y se pueden simplificar los calculos
            if (dot >= 1.0f || dot <= -1.0f)
                return a;

            // si el dot es negativo, significa que son opuestos, por lo que se invierte b
            // para que la interpolacion sea lo mas eficiente y corta posible
            if (dot < 0.0f)
            {
                b.xyz = -b.xyz;
                b.w = -b.w;
                dot = -dot;
            }

            float blendA; 
            float blendB; 
            if (dot < 0.99f)
            {
                // se obtiene el arco coseno de dot para obtener el angulo entre las rotaciones
                // ya que  dot es el resultado del coseno del angulo entre los quaterniones
                float halfAngle = Mathf.Acos(dot);
                // aca se obtiene el seno de medio angulo ya que determina la interpolacion en una esfera
                // y esta formula obtine la propocion adecuda de cada quaternion en la interpolacion
                float sinHalfAngle = Mathf.Sin(halfAngle);
                // aca se calcula el inverso del seno para normalizar los pesos de los quaterniones
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                // y aca se calcula el peso de cada quaternion en base a t
                blendA = Mathf.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
                blendB = Mathf.Sin(halfAngle * t) * oneOverSinHalfAngle;
                
                // nota : ya que en base t avance, a va teniendo menos peso
            }
            else
            {
                // en este caso, la rotacion es irrelevante, por lo que se simplifican los calculos
                blendA = 1.0f - t;
                blendB = t;
            }

            // calcula el resultado de la interpolacion en base a los pesos de cada uno en base de t
            VandalQuaternion result = new VandalQuaternion(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
            
            // si el resultado tiene una longtud mayor a 0, se normaliza para obtener unicamente la rotacion
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            
            // si es menor a 0, se devuleve un quaternion sin rotacion
            return identity;
        }

        public static VandalQuaternion Slerp(VandalQuaternion a, VandalQuaternion b, float t)
        {
            // clampea t entre 0 y 1
            Mathf.Clamp(t, 0, 1);
            
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
            // se utiliza para obtener la representacion de un eje con angulos
            
            // Calcula el eje de rotacion en base a w
            // el coseno de la mitad el angulo es igual a w
            // por lo que se realiza el arco coseno de w y se multiplica por 2 para obtener el angulo
            angle = 2.0f * Mathf.Acos(w);
            // obtiene la magnitud vectorial del quaternion
            // ya que w es el componente escala del quaternion, con la sigueinte formula se obtiene la magnitud
            float mag = Mathf.Sqrt(1.0f - w * w);
            
            // si la magnitud es relevante
            if (mag > 0.0001f)
            {
                //se obtiene el eje de rotacion normalizado
                axis = new Vec3(x, y, z) / mag;
            }
            else
            {
                // una rotacion nula
                axis = new Vec3(1, 0, 0);
            }
        }
        public void ToAngleAxis(out float angle, out Vec3 axis)
        {
            ToAxisAngle(out axis, out angle);
            angle *= Mathf.Rad2Deg;
        }
        
        #region Operators
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
        #endregion
    }

}
