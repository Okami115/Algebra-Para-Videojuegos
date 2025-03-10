using BridgeLeg;
using CustomMath;
using System;
using UnityEngine;


namespace ToyotaHylux
{
    struct ToyotaHylux
    {
        // Veriables definida para una matriz 4x4, 16 variable en total
        // Y es lo mismo que 4 Vectores 4
        public float m00;
        public float m10;
        public float m20;
        public float m30;
        public float m01;
        public float m11;
        public float m21;
        public float m31;
        public float m02;
        public float m12;
        public float m22;
        public float m32;
        public float m03;
        public float m13;
        public float m23;
        public float m33;

        // Constructor que recibe 4 vectores 4 para definir la matriz
        public ToyotaHylux(UnityEngine.Vector4 M0, UnityEngine.Vector4 M1, UnityEngine.Vector4 M2, UnityEngine.Vector4 M3)
        {
            m00 = M0.x;
            m01 = M1.x;
            m02 = M2.x;
            m03 = M3.x;
            m10 = M0.y;
            m11 = M1.y;
            m12 = M2.y;
            m13 = M3.y;
            m20 = M0.z;
            m21 = M1.z;
            m22 = M2.z;
            m23 = M3.z;
            m30 = M0.w;
            m31 = M1.w;
            m32 = M2.w;
            m33 = M3.w;
        }

        public float this[int row, int col]
        {
            get
            {
                return this[row + col * 4];
            }
            set
            {
                this[row + col * 4] = value;
            }
        }
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return m00;
                    case 1:
                        return m10;
                    case 2:
                        return m20;
                    case 3:
                        return m30;
                    case 4:
                        return m01;
                    case 5:
                        return m11;
                    case 6:
                        return m21;
                    case 7:
                        return m31;
                    case 8:
                        return m02;
                    case 9:
                        return m12;
                    case 10:
                        return m22;
                    case 11:
                        return m32;
                    case 12:
                        return m03;
                    case 13:
                        return m13;
                    case 14:
                        return m23;
                    case 15:
                        return m33;
                    default:
                        throw new IndexOutOfRangeException("Index out of Range!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m20 = value;
                        break;
                    case 3:
                        m30 = value;
                        break;
                    case 4:
                        m01 = value;
                        break;
                    case 5:
                        m11 = value;
                        break;
                    case 6:
                        m21 = value;
                        break;
                    case 7:
                        m31 = value;
                        break;
                    case 8:
                        m02 = value;
                        break;
                    case 9:
                        m12 = value;
                        break;
                    case 10:
                        m22 = value;
                        break;
                    case 11:
                        m32 = value;
                        break;
                    case 12:
                        m03 = value;
                        break;
                    case 13:
                        m13 = value;
                        break;
                    case 14:
                        m23 = value;
                        break;
                    case 15:
                        m33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Index out of Range!");
                }
            }
        }

        public static ToyotaHylux zero
        {
            get
            {
                return new ToyotaHylux()
                {
                    m00 = 0.0f,
                    m01 = 0.0f,
                    m02 = 0.0f,
                    m03 = 0.0f,
                    m10 = 0.0f,
                    m11 = 0.0f,
                    m12 = 0.0f,
                    m13 = 0.0f,
                    m20 = 0.0f,
                    m21 = 0.0f,
                    m22 = 0.0f,
                    m23 = 0.0f,
                    m30 = 0.0f,
                    m31 = 0.0f,
                    m32 = 0.0f,
                    m33 = 0.0f
                };
            }
        }
        public static ToyotaHylux identity
        {
            // devuelve una matriz en Zero, menos la diagonal, la cual seria la escala del objeto.
            get
            {
                ToyotaHylux m = zero;
                m.m00 = 1.0f;
                m.m11 = 1.0f;
                m.m22 = 1.0f;
                m.m33 = 1.0f;
                return m;
            }
        }
        private bool IsIdentity()
        {
            return this == identity;
        }

        public Vec3 lossyScale
        {
            // obtiene la escala de la digonal de la matriz con todas las modificaciones aplicadas
            // y luego se calcula la magnitud de estos vectores que nos da la cantidad de escala aplicada a estos ejes
            get
            {
                return new Vec3(GetColumn(0).magnitude, GetColumn(1).magnitude, GetColumn(2).magnitude);
            }
        }

        public static ToyotaHylux Transpose(ToyotaHylux m)
        {
            // devuelve la matriz con las filas y columnas trasnpuestas
            return new ToyotaHylux()
            {
                m01 = m.m10,
                m02 = m.m20,
                m03 = m.m30,
                m10 = m.m01,
                m12 = m.m21,
                m13 = m.m31,
                m20 = m.m02,
                m21 = m.m12,
                m23 = m.m32,
                m30 = m.m03,
                m31 = m.m13,
                m32 = m.m23,
            };
            // nota : hacerlo de esta manera es mas eficiente
        }

        // invierte una matriz 4x4 sobre su diagonal y eso solo se puede hace si su determinate es valida
        public static ToyotaHylux Inverse(ToyotaHylux m)
        {
            // determina si la matriz es reversible en base al resultado de la determinate
            float detA = Determinant(m);
            if (detA == 0)
                return zero;

            ToyotaHylux aux = new ToyotaHylux()
            {
                // calcula los cofactores de la matriz m
                m00 = m.m11 * m.m22 * m.m33 + m.m12 * m.m23 * m.m31 + m.m13 * m.m21 * m.m32 - m.m11 * m.m23 * m.m32 - m.m12 * m.m21 * m.m33 - m.m13 * m.m22 * m.m31,
                m01 = m.m01 * m.m23 * m.m32 + m.m02 * m.m21 * m.m33 + m.m03 * m.m22 * m.m31 - m.m01 * m.m22 * m.m33 - m.m02 * m.m23 * m.m31 - m.m03 * m.m21 * m.m32,
                m02 = m.m01 * m.m12 * m.m33 + m.m02 * m.m13 * m.m32 + m.m03 * m.m11 * m.m32 - m.m01 * m.m13 * m.m32 - m.m02 * m.m11 * m.m33 - m.m03 * m.m12 * m.m31,
                m03 = m.m01 * m.m13 * m.m22 + m.m02 * m.m11 * m.m23 + m.m03 * m.m12 * m.m21 - m.m01 * m.m12 * m.m23 - m.m02 * m.m13 * m.m21 - m.m03 * m.m11 * m.m22,

                m10 = m.m10 * m.m23 * m.m32 + m.m12 * m.m20 * m.m33 + m.m13 * m.m22 * m.m30 - m.m10 * m.m22 * m.m33 - m.m12 * m.m23 * m.m30 - m.m13 * m.m20 * m.m32,
                m11 = m.m00 * m.m22 * m.m33 + m.m02 * m.m23 * m.m30 + m.m03 * m.m20 * m.m32 - m.m00 * m.m23 * m.m32 - m.m02 * m.m20 * m.m33 - m.m03 * m.m22 * m.m30,
                m12 = m.m00 * m.m13 * m.m32 + m.m02 * m.m10 * m.m33 + m.m03 * m.m12 * m.m30 - m.m00 * m.m12 * m.m33 - m.m02 * m.m13 * m.m30 - m.m03 * m.m10 * m.m32,
                m13 = m.m00 * m.m12 * m.m23 + m.m02 * m.m13 * m.m20 + m.m03 * m.m10 * m.m22 - m.m00 * m.m13 * m.m22 - m.m02 * m.m10 * m.m23 - m.m03 * m.m12 * m.m20,

                m20 = m.m10 * m.m21 * m.m33 + m.m11 * m.m23 * m.m30 + m.m13 * m.m20 * m.m31 - m.m10 * m.m23 * m.m31 - m.m11 * m.m20 * m.m33 - m.m13 * m.m31 * m.m30,
                m21 = m.m00 * m.m23 * m.m31 + m.m01 * m.m20 * m.m33 + m.m03 * m.m21 * m.m30 - m.m00 * m.m21 * m.m33 - m.m01 * m.m23 * m.m30 - m.m03 * m.m20 * m.m31,
                m22 = m.m00 * m.m11 * m.m33 + m.m01 * m.m13 * m.m31 + m.m03 * m.m10 * m.m31 - m.m00 * m.m13 * m.m31 - m.m01 * m.m10 * m.m33 - m.m03 * m.m11 * m.m30,
                m23 = m.m00 * m.m13 * m.m21 + m.m01 * m.m10 * m.m23 + m.m03 * m.m11 * m.m31 - m.m00 * m.m11 * m.m23 - m.m01 * m.m13 * m.m20 - m.m03 * m.m10 * m.m21,

                m30 = m.m10 * m.m22 * m.m31 + m.m11 * m.m20 * m.m32 + m.m12 * m.m21 * m.m30 - m.m00 * m.m21 * m.m32 - m.m11 * m.m22 * m.m30 - m.m12 * m.m20 * m.m31,
                m31 = m.m00 * m.m21 * m.m32 + m.m01 * m.m22 * m.m30 + m.m02 * m.m20 * m.m31 - m.m00 * m.m22 * m.m31 - m.m01 * m.m20 * m.m32 - m.m02 * m.m21 * m.m30,
                m32 = m.m00 * m.m12 * m.m31 + m.m01 * m.m10 * m.m32 + m.m02 * m.m11 * m.m30 - m.m00 * m.m11 * m.m32 - m.m01 * m.m12 * m.m30 - m.m02 * m.m10 * m.m31,
                m33 = m.m00 * m.m11 * m.m22 + m.m01 * m.m12 * m.m20 + m.m02 * m.m10 * m.m21 - m.m00 * m.m12 * m.m21 - m.m01 * m.m10 * m.m22 - m.m02 * m.m11 * m.m20
            };
            
            // luego de los cofactores, se debe transponer los valores de la matriz

            ToyotaHylux ret = new ToyotaHylux()
            {
                // normaliza la diagonal para evitar perdida de informacion
                
                m00 = aux.m00 / detA,
                m01 = aux.m01 / detA,
                m02 = aux.m02 / detA,
                m03 = aux.m03 / detA,
                m10 = aux.m10 / detA,
                m11 = aux.m11 / detA,
                m12 = aux.m12 / detA,
                m13 = aux.m13 / detA,
                m20 = aux.m20 / detA,
                m21 = aux.m21 / detA,
                m22 = aux.m22 / detA,
                m23 = aux.m23 / detA,
                m30 = aux.m30 / detA,
                m31 = aux.m31 / detA,
                m32 = aux.m32 / detA,
                m33 = aux.m33 / detA
                
                // finalmente cada elemeto de la matriz se tiene que dividir por la determinante de la matriz original
                // y con eso se termina la fromula de reversivilidad de las matrices

            };
            return ret;
        }

        public UnityEngine.Vector4 GetColumn(int i)
        {
            return new UnityEngine.Vector4(this[0, i], this[1, i], this[2, i], this[3, i]);
        }
        public void SetColumn(int index, UnityEngine.Vector4 col)
        {
            this[0, index] = col.x;
            this[1, index] = col.y;
            this[2, index] = col.z;
            this[3, index] = col.w;
        }
        public UnityEngine.Vector4 GetRow(int i)
        {
            switch (i)
            {
                case 0:
                    return new UnityEngine.Vector4(m00, m01, m02, m03);
                case 1:
                    return new UnityEngine.Vector4(m10, m11, m12, m13);
                case 2:
                    return new UnityEngine.Vector4(m20, m21, m22, m23);
                case 3:
                    return new UnityEngine.Vector4(m30, m31, m32, m33);
                default:
                    throw new IndexOutOfRangeException("Index out of Range!");
            }
        }
        public void SetRow(int index, UnityEngine.Vector4 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
        }

        // Obtiene la rotacion guardada dentro de una matriz 4x4
        private VandalQuaternion GetRotation()
        {
            ToyotaHylux m = this;
            VandalQuaternion q = new VandalQuaternion();

            // se obtienen los datos del quaternion en base a la digonal de la matriz
            // nota : el 1 es para asegurarse de que el valor sea positivo y se utiliza MAX para evitar numeros complejos
            // y se divide en 2 para normalizarlo
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2; // Se calcula W que es la parte real del quaternion
            // en cada ecuacion se refueza un valor epecifico que es el que queremos obtener y el resto se resta
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2; // Se calcula el componente real de X
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2; // Se calcula el componente real de Y
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2; // Se calcula el componente real de Z
            
            // la diferencia entre los elementos fuera de la digonal determinan la direccion de la rotacion
            // y ademas de ajustar los signos, Mathf.Sing asegura que tengan la orientacion correcta 
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2])); // Se lo multiplica por el componente complejo(i) para formal el valor complejo de X
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0])); // Se lo multiplica por el componente complejo(j) para formal el valor complejo de Y
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1])); // Se lo multiplica por el componente complejo(k) para formal el valor complejo de Z

            return q;
            
            // nota : la formula aplicada es la formula de conversion de Shoemake
        }
        public Vec3 GetPosition()
        {
            return new Vec3(m03, m13, m23);
        }

        // Saca el valor de la determinante y define si es reversible o no la matriz
        public static float Determinant(ToyotaHylux m)
        {
            // es una operacion matematica la cual calcula la cantidad de informacion que se pierde con la inversion de la matriz
            // Si es igual o mayor a Cero, significa que la perdida de datos es nula, sin embargo, si da algun numero negativo
            // significaria que la matriz no puede ser reversible ya que se estarian perdiendo datos
            return
                m[0, 3] * m[1, 2] * m[2, 1] * m[3, 0] - m[0, 2] * m[1, 3] * m[2, 1] * m[3, 0] -
                m[0, 3] * m[1, 1] * m[2, 2] * m[3, 0] + m[0, 1] * m[1, 3] * m[2, 2] * m[3, 0] +
                m[0, 2] * m[1, 1] * m[2, 3] * m[3, 0] - m[0, 1] * m[1, 2] * m[2, 3] * m[3, 0] -
                m[0, 3] * m[1, 2] * m[2, 0] * m[3, 1] + m[0, 2] * m[1, 3] * m[2, 0] * m[3, 1] +
                m[0, 3] * m[1, 0] * m[2, 2] * m[3, 1] - m[0, 0] * m[1, 3] * m[2, 2] * m[3, 1] -
                m[0, 2] * m[1, 0] * m[2, 3] * m[3, 1] + m[0, 0] * m[1, 2] * m[2, 3] * m[3, 1] +
                m[0, 3] * m[1, 1] * m[2, 0] * m[3, 2] - m[0, 1] * m[1, 3] * m[2, 0] * m[3, 2] -
                m[0, 3] * m[1, 0] * m[2, 1] * m[3, 2] + m[0, 0] * m[1, 3] * m[2, 1] * m[3, 2] +
                m[0, 1] * m[1, 0] * m[2, 3] * m[3, 2] - m[0, 0] * m[1, 1] * m[2, 3] * m[3, 2] -
                m[0, 2] * m[1, 1] * m[2, 0] * m[3, 3] + m[0, 1] * m[1, 2] * m[2, 0] * m[3, 3] +
                m[0, 2] * m[1, 0] * m[2, 1] * m[3, 3] - m[0, 0] * m[1, 2] * m[2, 1] * m[3, 3] -
                m[0, 1] * m[1, 0] * m[2, 2] * m[3, 3] + m[0, 0] * m[1, 1] * m[2, 2] * m[3, 3];
            
            //al multiplicar la matriz en esta secuencia queda reflejada la "expancion"
            
            // nota : este metodo es el calculo de la determinante por Cofactores
            // La expansión por cofactores toma una fila o una columna de la matriz y
            // la "expande" en una suma de productos de los elementos de esa fila o columna y
            // sus determinantes correspondientes de matrices más pequeñas
        }

        // Almacena la escala de un objeto dentro de la diagonal de una matriz
        public static ToyotaHylux Scale(Vec3 v)
        {
            ToyotaHylux m;
            m.m00 = v.x;
            m.m01 = 0.0f;
            m.m02 = 0.0f;
            m.m03 = 0.0f;
            m.m10 = 0.0f;
            m.m11 = v.y;
            m.m12 = 0.0f;
            m.m13 = 0.0f;
            m.m20 = 0.0f;
            m.m21 = 0.0f;
            m.m22 = v.z;
            m.m23 = 0.0f;
            m.m30 = 0.0f;
            m.m31 = 0.0f;
            m.m32 = 0.0f;
            m.m33 = 1f;
            return m;
        }

        // Alamacena la rotacion del objeto dentro de la matriz
        public static ToyotaHylux Rotate(VandalQuaternion q)
        {
            double num1 = q.x * 2f;
            double num2 = q.y * 2f; // se multiplica por 2 a los componentes iamginarios del quaternion por la cantidad de diemnciones en la que se trabaja
            double num3 = q.z * 2f;

            // se calcula los temino que aparecen en la diagon de la matriz
            double num4 = q.x * num1;
            double num5 = q.y * num2;
            double num6 = q.z * num3;
            
            // se calculan los terminos de manera curzada que aparecen fuera de la matriz
            double num7 = q.x * num2;
            double num8 = q.x * num3;
            double num9 = q.y * num3;
            
            // se calculan los terminos que dependen de w que afectan la orientacion de la rotacion
            double num10 = q.w * num1;
            double num11 = q.w * num2;
            double num12 = q.w * num3;
            
            ToyotaHylux m;

            // El 1- es porque las posciones de correspondientes pertenece a la diagonal, que a su vez pertenecen a la escala y son inicializadas en 1.
            m.m00 = (float)(1.0 - num5 + num6);//Almacena el coseno de la parte real de Y y Z
            m.m10 = (float)(num7 + num12);// Alamcena el Seno Positivo de la parte compleja de Z
            m.m20 = (float)(num8 - num11);// Almacena el Seno negativo de la parte compleja de Y
            m.m30 = 0.0f;
            m.m01 = (float)(num7 - num12);// Alamacena el seno negativo de la parte compleja de Z
            m.m11 = (float)(1.0 - num4 + num6);// Almacena el coseno de la parte real de Z y X 
            m.m21 = (float)(num9 + num10);// Alamacena el Seno negativa de la parte compleja de X
            m.m31 = 0.0f;
            m.m02 = (float)(num8 + num11);// Alamacena el Seno postiva de la parte compleja de Y
            m.m12 = (float)(num9 - num10);// Alamacena el Seno negativa de la parte compleja de X
            m.m22 = (float)(1.0 - num4 + num5);// Almacena el coseno de la parte real de Y y X 
            m.m32 = 0.0f;
            m.m03 = 0.0f;
            m.m13 = 0.0f;
            m.m23 = 0.0f;
            m.m33 = 1f;
            return m;
        }

        // Guarda la poscicion dentro de la matriz en la comlumna mas a la derecha de la misma.
        public static ToyotaHylux Translate(Vec3 v)
        {
            ToyotaHylux m;
            m.m00 = 1f;
            m.m01 = 0.0f;
            m.m02 = 0.0f;
            m.m03 = v.x;
            m.m10 = 0.0f;
            m.m11 = 1f;
            m.m12 = 0.0f;
            m.m13 = v.y;
            m.m20 = 0.0f;
            m.m21 = 0.0f;
            m.m22 = 1f;
            m.m23 = v.z;
            m.m30 = 0.0f;
            m.m31 = 0.0f;
            m.m32 = 0.0f;
            m.m33 = 1f;
            return m;
        }

        //Una matriz TRS es una matriz 4x4 que contiene los valores de posicion, rotacion y escala de un objeto.
        public static ToyotaHylux TRS(Vec3 pos, VandalQuaternion q, Vec3 s)
        { 
            return (Translate(pos) * Rotate(q) * Scale(s));
            
            // primero se escala el objeto, ya que de otra forma al escalarlo despues podri terminar en otra posicion
            
            // segundo rotacion, ya que la rotacion ocurre siempre entorno al origen, pera evitar movimientos inesperados
            
            // y por ultimo traslacion una vez ya esta el objeto escalado y rotado

        }
        public void SetTRS(Vec3 pos, VandalQuaternion q, Vec3 s)
        {
            this = TRS(pos, q, s);
        }
        
        // Esta funcion chequea que la matriz sea una matriz TRS valida 
        // Si la posiciones 
        public bool ValidTRS()
        {
            // cheque que la diagonal o la escala no sea Cero
            if (lossyScale == Vec3.Zero)
                return false;
            
            if (m00 == double.NaN && m10 == double.NaN && m20 == double.NaN && m30 == double.NaN &&
                m01 == double.NaN && m11 == double.NaN && m21 == double.NaN && m31 == double.NaN && // Cheque que sus componentesd sean numeros
                m02 == double.NaN && m12 == double.NaN && m22 == double.NaN && m32 == double.NaN &&
                m03 == double.NaN && m13 == double.NaN && m23 == double.NaN && m33 == double.NaN)
                return false;
            
            // Chequea que la rotacion sea valida
            if (Mathf.Abs(GetRotation().x) > 1 || 
                Mathf.Abs(GetRotation().y) > 1 || 
                Mathf.Abs(GetRotation().z) > 1 || 
                Mathf.Abs(GetRotation().w) > 1)
                return false;
            
            // Chequea que la traslacion sea valida
            if (m30 != 0 || m31 != 0 || m32 != 0 || m33 != 1)
                return false;
            
            return true;
        }
        public Vec3 MultiplyVector(Vec3 v) 
        {
            Vec3 v3;
            v3.x = (float)((double)m00 * (double)v.x + (double)m01 * (double)v.y + (double)m02 * (double)v.z);
            v3.y = (float)((double)m10 * (double)v.x + (double)m11 * (double)v.y + (double)m12 * (double)v.z);
            v3.z = (float)((double)m20 * (double)v.x + (double)m21 * (double)v.y + (double)m22 * (double)v.z);
            return v3;
        }
        public Vec3 MultiplyPoint3x4(Vec3 p)
        {
            Vec3 v3;
            v3.x = (float)((double)m00 * (double)p.x + (double)m01 * (double)p.y + (double)m02 * (double)p.z) + m03;
            v3.y = (float)((double)m10 * (double)p.x + (double)m11 * (double)p.y + (double)m12 * (double)p.z) + m13;
            v3.z = (float)((double)m20 * (double)p.x + (double)m21 * (double)p.y + (double)m22 * (double)p.z) + m23;
            return v3;
        }
        public Vec3 MultiplyPoint(Vec3 p)
        {
            Vec3 v3;

            v3.x = (float)((double)m00 * (double)p.x + (double)m01 * (double)p.y + (double)m02 * (double)p.z) + m03;
            v3.y = (float)((double)m10 * (double)p.x + (double)m11 * (double)p.y + (double)m12 * (double)p.z) + m13;
            v3.z = (float)((double)m20 * (double)p.x + (double)m21 * (double)p.y + (double)m22 * (double)p.z) + m23;
            float num = 1f / ((float)((double)m30 * (double)p.x + (double)m31 * (double)p.y + (double)m32 * (double)p.z) + m33);
            v3.x *= num;
            v3.y *= num;
            v3.z *= num;
            return v3;
        }

        public float determinant => Determinant(this);
        public VandalQuaternion rotation => GetRotation();
        public ToyotaHylux inverse => Inverse(this);

        public static UnityEngine.Vector4 operator *(ToyotaHylux a, UnityEngine.Vector4 v)
        {
            UnityEngine.Vector4 ret;
            ret.x = (float)((double)a.m00 * (double)v.x + (double)a.m01 * (double)v.y + (double)a.m02 * (double)v.z + (double)a.m03 * (double)v.w);
            ret.y = (float)((double)a.m10 * (double)v.x + (double)a.m11 * (double)v.y + (double)a.m12 * (double)v.z + (double)a.m13 * (double)v.w);
            ret.z = (float)((double)a.m20 * (double)v.x + (double)a.m21 * (double)v.y + (double)a.m22 * (double)v.z + (double)a.m23 * (double)v.w);
            ret.w = (float)((double)a.m30 * (double)v.x + (double)a.m31 * (double)v.y + (double)a.m32 * (double)v.z + (double)a.m33 * (double)v.w);
            return ret;
        }
        
        // Porque multiplicarias 2 matrices TRS? 
        // Porque de esta forma podemos lograr que una (a) sea dependiente de la otra (b)
        // Esto da a lugar a que un objeto rote porque mas que su rotacion sea 0
        // Ya que la rotacion del padre(b) estaria afectando al hijo(a)
        public static ToyotaHylux operator *(ToyotaHylux a, ToyotaHylux b)
        {
            ToyotaHylux ret = zero;
            for (int i = 0; i < 4; i++)
            {
                ret.SetColumn(i, a * b.GetColumn(i));
            }

            return ret;
        }

        public static bool operator ==(ToyotaHylux a, ToyotaHylux b) => a.GetColumn(0) == b.GetColumn(0) && a.GetColumn(1) == b.GetColumn(1) && a.GetColumn(2) == b.GetColumn(2) && a.GetColumn(3) == b.GetColumn(3);

        public static bool operator !=(ToyotaHylux a, ToyotaHylux b) => !(a == b);
    }
}

