using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public class CalculateQuadrilateral : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public LineRenderer lineRenderer;

    private List<Vector3> points = new List<Vector3>();
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<GameObject> quadrilaterals = new List<GameObject>();
    private line[] lines = { new line(), new line(), new line(), new line() };
    private bool isDrawing = false;
    private int counter = 0;

    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            if (counter == 4)
            {
                counter = 0;

                foreach (LineRenderer line in lineRenderers)
                {
                    Destroy(line.gameObject);
                }

                foreach (GameObject quadrilateral in quadrilaterals)
                {
                    Destroy(quadrilateral);
                }

                lineRenderers.Clear();
                quadrilaterals.Clear();
                points.Clear();
                resultText.text = "";
            }

            if (!isDrawing)
            {
                lineRenderer.positionCount = 0;
            }

            points.Add(worldPos);
            lines[counter].start = worldPos;
            isDrawing = true;
        }

        if (isDrawing)
        {
            if (points.Count > 0)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, points[points.Count - 1]);
                lineRenderer.SetPosition(1, worldPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            points.Add(worldPos);

            lineRenderers.Add(Instantiate(lineRenderer));
            lines[counter].end = worldPos;

            counter++;

            if (counter == 4)
            {
                CheckCuadrilateral();
            }
        }
    }

    private void CheckCuadrilateral()
    {
        HashSet<Vector3> intersectionPoints = new HashSet<Vector3>();

        // Obtener todas las intersecciones
        foreach (var line in lines)
        {
            var (points, hasIntersections) = line.GetIntersections(lines);
            if (hasIntersections)
            {
                foreach (var point in points)
                {
                    intersectionPoints.Add(point);
                }
            }
        }

        List<Vector3> pointsList = new List<Vector3>(intersectionPoints); // Lista de puntos de intersección

        // Recorrer la lista de puntos y probar diferentes combinaciones de 4 puntos
        for (int i = 0; i <= pointsList.Count - 4; i++)
        {
            // Seleccionar un conjunto de 4 puntos consecutivos
            List<Vector3> quadPoints = pointsList.GetRange(i, 4);

            quadPoints = SortClockwise(quadPoints);
            
            // Comprobar si el cuadrilátero formado tiene un ángulo de 180° en alguno de sus vértices
            if (HasValidAngles(quadPoints) && HasValidCollinearity(quadPoints))
            {
                // Si el cuadrilátero es válido, lo procesamos
                AllLinesAreOverlapping(quadPoints);
                Debug.Log("Se detectó un cuadrilátero válido.");
                foreach (Vector3 p in quadPoints)
                {
                    quadrilaterals.Add(new GameObject());
                    quadrilaterals[quadrilaterals.Count - 1].transform.position = p;
                }

                resultText.text = $"Area total : {CalculateArea(quadPoints)}";
                
                return; // Si encontramos un cuadrilátero válido, salimos de la función
            }
        }

        Debug.Log("No se detectó un cuadrilátero válido.");
    }
    private bool HasValidCollinearity(List<Vector3> points)
    {
        // Generar todas las combinaciones posibles de 3 puntos dentro de los 4
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                for (int k = j + 1; k < points.Count; k++)
                {
                    if (AreCollinear(points[i], points[j], points[k]))
                    {
                        return false;  // Si hay tres puntos colineales, no es un cuadrilátero válido
                    }
                }
            }
        }

        Debug.Log("No tiene colinealidad, valido");
        return true;  // Si no encontramos colinealidad, es un cuadrilátero válido
    }
    
    private static bool AreCollinear(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Vectores formados por los puntos
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p2;

        // Producto cruzado. Si es cero, los puntos son colineales
        return Mathf.Approximately(Vector3.Cross(v1, v2).magnitude, 0f);
    }

    // Función para calcular el ángulo entre tres puntos.
    private static float AngleBetweenVectors(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 v1 = p1 - p2;
        Vector3 v2 = p3 - p2;

        float angle = Vector3.Angle(v1, v2);
        return angle;
    }

    private bool HasValidAngles(List<Vector3> points)
    {
        float totalAngle = 0f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p1 = points[i];
            Vector3 p2 = points[(i + 1) % points.Count];
            Vector3 p3 = points[(i + 2) % points.Count];

            float angle = AngleBetweenVectors(p1, p2, p3);
            Debug.Log($"ángulo n{i} : {angle}°.");
            totalAngle += angle;
        }

        // Verificar si la suma de los 4 ángulos es igual a 360°
        if (Mathf.Approximately(totalAngle, 360f))
        {
            Debug.Log($"Los ángulos suman {totalAngle}°.");
            return true;
        }

        Debug.Log("Los ángulos no suman 360°.");
        return false; // Todos los ángulos son válidos
    }
    
    private List<Vector3> SortClockwise(List<Vector3> points)
    {
        Vector3 centroid = Vector3.zero;
        foreach (var p in points)
        {
            centroid += p;
        }
        centroid /= points.Count;

        // Ordenar los puntos por el ángulo respecto al centroide
        points.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.y - centroid.y, a.x - centroid.x);
            float angleB = Mathf.Atan2(b.y - centroid.y, b.x - centroid.x);
            return angleA.CompareTo(angleB);
        });

        return points;
    }

    public bool AllLinesAreOverlapping(List<Vector3> intersectionPoints)
    {
        int count = 0;

        for (int i = 0; i < intersectionPoints.Count; i++)
        {
            Vector3 p1 = points[i];
            Vector3 p2 = points[(i + 1) % points.Count];
            
            line insersectionLine = new line(p1, p2);
            
            for (int j = 0; j < lines.Length; j++)
            {
                if (AreOverlapping(insersectionLine, lines[j]))
                {
                    count++;
                    break; // Si ya encontramos una coincidencia, no seguimos buscando
                }
            }
        }

        Debug.Log($"Solo {count} de los segmentos de intersección están alineados.");
        return count == 4;
    }
    
    // Función para verificar si dos líneas son colineales
    public bool AreOverlapping(line l1, line l2)
    {
        // Paso 1: Verificar que las líneas sean colineales
        if (!line.AreCollinear(l1, l2))
        {
            return false;
        }

        // Paso 2: Verificar si los puntos de las líneas están en el mismo segmento
        return line.IsPointOnSegment(l1.start, l2) && line.IsPointOnSegment(l1.end, l2);
    }
    private float CalculateArea(List<Vector3> points)
    {
        // Asegurarse de que tengamos exactamente 4 puntos
        if (points.Count != 4)
        {
            Debug.LogError("El cuadrilátero debe tener 4 puntos.");
            return 0f;
        }

        // Coordenadas de los puntos
        float x1 = points[0].x, y1 = points[0].y;
        float x2 = points[1].x, y2 = points[1].y;
        float x3 = points[2].x, y3 = points[2].y;
        float x4 = points[3].x, y4 = points[3].y;

        // Aplicar la fórmula del área para el cuadrilátero
        float area = 0.5f * Mathf.Abs(x1 * y2 + x2 * y3 + x3 * y4 + x4 * y1 - (y1 * x2 + y2 * x3 + y3 * x4 + y4 * x1));
        return area;
    }
    
    private void OnDrawGizmos()
    {
        if (quadrilaterals.Count > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(quadrilaterals[0].transform.position, quadrilaterals[1].transform.position);
            Gizmos.DrawLine(quadrilaterals[1].transform.position, quadrilaterals[2].transform.position);
            Gizmos.DrawLine(quadrilaterals[2].transform.position, quadrilaterals[3].transform.position);
            Gizmos.DrawLine(quadrilaterals[3].transform.position, quadrilaterals[0].transform.position);
        }
    }
}