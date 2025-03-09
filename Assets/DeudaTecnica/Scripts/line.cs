using System.Collections.Generic;
using UnityEngine;

public struct line
{
    public Vector3 start;
    public Vector3 end;

    public line(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }

    public (List<Vector3> result, bool intersect) GetIntersections(line[] lines)
    {
        List<Vector3> result = new List<Vector3>();
        bool intersect = false;

        foreach (var other in lines)
        {
            if (this.Equals(other))
                continue;

            if (FindIntersection(this, other, out Vector3 intersectionPoint))
            {
                result.Add(RoundVector(intersectionPoint, 3));
                Debug.Log($"Has intersection : {intersectionPoint}");
                
                intersect = true;
            }
        }
        
        
        return (result, intersect);
    }
    
    
    // Función que verifica si dos líneas son colineales (en la misma dirección)
    public static bool AreCollinear(line l1, line l2)
    {
        Vector3 direction1 = l1.end - l1.start;
        Vector3 direction2 = l2.end - l2.start;

        // Producto cruzado debe ser cero para que las líneas sean colineales
        return Mathf.Approximately(Vector3.Cross(direction1, direction2).magnitude, 0f);
    }
    private static bool FindIntersection(line line1, line line2, out Vector3 intersection)
    {
        intersection = Vector3.zero;

        Vector3 A = line1.start, B = line1.end;
        Vector3 C = line2.start, D = line2.end;

        float denominator = (A.x - B.x) * (C.y - D.y) - (A.y - B.y) * (C.x - D.x);

        // Si el denominador es 0, las líneas son paralelas o coincidentes
        if (Mathf.Abs(denominator) < 0.0001f)
        {
            return false;
        }

        float t = ((A.x - C.x) * (C.y - D.y) - (A.y - C.y) * (C.x - D.x)) / denominator;
        float u = ((A.x - C.x) * (A.y - B.y) - (A.y - C.y) * (A.x - B.x)) / denominator;

        // Verificar si la intersección está dentro de los segmentos (0 <= t, u <= 1)
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = new Vector3(A.x + t * (B.x - A.x), A.y + t * (B.y - A.y), 0);
            return true;
        }

        return false;
    }
    
    Vector3 RoundVector(Vector3 vector, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        float x = Mathf.Round(vector.x * factor) / factor;
        float y = Mathf.Round(vector.y * factor) / factor;
        float z = Mathf.Round(vector.z * factor) / factor;
        return new Vector3(x, y, z);
    }
    
    // Función que verifica si un punto está dentro de un segmento
    public static bool IsPointOnSegment(Vector3 point, line l)
    {
        // Verificar si el punto está en el segmento de la línea
        float cross1 = CrossProduct(l.start, l.end, point);
        float cross2 = CrossProduct(l.start, point, l.end);

        // Si ambos productos cruzados son cero y el punto está dentro de los extremos de la línea
        return Mathf.Approximately(cross1, 0) && Mathf.Approximately(cross2, 0) &&
               Mathf.Min(l.start.x, l.end.x) <= point.x && point.x <= Mathf.Max(l.start.x, l.end.x) &&
               Mathf.Min(l.start.y, l.end.y) <= point.y && point.y <= Mathf.Max(l.start.y, l.end.y);
    }

    // Producto cruzado 2D
    public static float CrossProduct(Vector3 a, Vector3 b, Vector3 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }
}