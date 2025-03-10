using CustomMath;

public struct Llano
{
    internal const int size = 16;

    private Vec3 m_Normal;

    private float m_Distance;

    public Vec3 a;
    public Vec3 b;
    public Vec3 c;

    public Vec3 normal
    {
        get { return m_Normal; }
        set { m_Normal = value; }
    }

    public float distance
    {
        get { return m_Distance; }
        set { m_Distance = value; }
    }

    public Llano flipped => new Llano(-m_Normal, 0f - m_Distance);

    public Llano (Vec3 inNormal, Vec3 inPoint) 
    {
        m_Normal = Vec3.Normalize(inNormal);
        // calcula la distancia del plano sobre el origen utilizando la formula del plano 3D
        // donde d = -(normal * punto)
        m_Distance = 0f - Vec3.Dot(m_Normal, inPoint);
        this.a = Vec3.Zero;
        this.b = Vec3.Zero;
        this.c = Vec3.Zero;
    }

    public Llano(Vec3 inNormal, float d)
    {
        m_Normal = Vec3.Normalize(inNormal);
        m_Distance = d;
        this.a = Vec3.Zero;
        this.b = Vec3.Zero;
        this.c = Vec3.Zero;
    }

    public Llano(Vec3 a, Vec3 b, Vec3 c)
    {
        // primaero calcula 2 vectores dentro del plano (b - a) y (c - a)
        // despues en base a esos 2 vectores obtiene un vector perpendicular, lo que es la normal
        m_Normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
        m_Distance = 0f - Vec3.Dot(m_Normal, a);
        this.a = a;
        this.b = b;
        this.c = c;
        
        // nota : se puede cambiar la direccion de la normal del plano si cambias el orden en el que pasas los vectores
        // nota : solo puede existir un plano que pase por estos 3 puntos a la vez
    }

    public void SetNormalAndPosition(Vec3 inNormal, Vec3 inPoint)
    {
        m_Normal = Vec3.Normalize(inNormal);
        m_Distance = -Vec3.Dot(inNormal, inPoint); 
    }

    public void Set3Points(Vec3 a, Vec3 b, Vec3 c)
    {
        m_Normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
        m_Distance = -Vec3.Dot(m_Normal, a);

    }

    public void Flip()
    {
        m_Normal = -m_Normal;
        m_Distance = -m_Distance;
    }

    public void Translate(Vec3 translation)
    {
        m_Distance += Vec3.Dot(m_Normal, translation);
    }

    public static Llano Translate(Llano llano, Vec3 translation) 
    {
        return new Llano(llano.m_Normal, llano.m_Distance += Vec3.Dot(llano.m_Normal, translation));
    }

    public Vec3 ClosestPointOnPlane(Vec3 point)
    {
        float num = GetDistanceToPoint(point);
        // esto mueve el punto hacia el plano a lo largo de la normal
        return point - m_Normal * num; // la multiplicacion es el vector de movimiento desde el punto hacia el plano
    }

    public float GetDistanceToPoint(Vec3 point)
    {
        // calcula la distancia del punto hacia el plano
        return Vec3.Dot(m_Normal, point) + m_Distance;
    }

    public bool GetSide(Vec3 point)
    {
        // esto devuevle si el punto esta del lado positivo del plano o no
        return Vec3.Dot(m_Normal, point) + m_Distance > 0f;
        
        // nota : estar en el lado negativo, indica que esta en direccion opuesta a la normal
    }

    public bool SameSide(Vec3 inPt0, Vec3 inPt1)
    {
        float distanceToPoint = GetDistanceToPoint(inPt0);
        float distanceToPoint2 = GetDistanceToPoint(inPt1);
        return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
        // esto devuelve si los 2 puntos estan del mismo lado del plano
    }
}
