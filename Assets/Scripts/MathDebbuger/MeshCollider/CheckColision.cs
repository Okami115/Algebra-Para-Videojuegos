using CustomMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class CheckColision : MonoBehaviour
{

    [SerializeField] private GameObject a;
    [SerializeField] private GameObject b;

    [SerializeField] private GameObject grid;

    [SerializeField] private List<GameObject> pointList;
    [SerializeField] private List<Llano> llanoListA;
    [SerializeField] private List<Llano> llanoListB;

    private List<Vec3> colisionPointsA;
    private List<Vec3> colisionPointsB;

    private GeneratePlanes aScript;
    private GeneratePlanes bScript;

    private Vec3 direction = Vec3.Forward * 100;

    void Start()
    {
        pointList = grid.GetComponent<GenerateGrid>().GetList();
        llanoListA = new List<Llano>();
        llanoListB = new List<Llano>();
        colisionPointsA = new List<Vec3>();
        colisionPointsB = new List<Vec3>();

        aScript = a.GetComponent<GeneratePlanes>();
        bScript = b.GetComponent<GeneratePlanes>();

    }


    void Update()
    {
        llanoListA = aScript.GetPlaneList();
        llanoListB = bScript.GetPlaneList();

        CheckLlanoCoslision(llanoListA, colisionPointsA);
        a.GetComponent<GeneratePlanes>().SetColisionpoints(colisionPointsA);

        CheckLlanoCoslision(llanoListB, colisionPointsB);
        b.GetComponent<GeneratePlanes>().SetColisionpoints(colisionPointsB);

        CompareList();
    }

    private void CheckLlanoCoslision(List<Llano> llanos, List<Vec3> list)
    {
        list.Clear();

        for (int i = 0; i < pointList.Count; i++)
        {
            int count = 0;

            for(int j = 0; j < llanos.Count; j++)
            {
                if (IsPointInPlane(llanos[j], new Vec3(pointList[i].transform.position), out Vec3 point))
                {
                    if (TrianglePointColision(point, llanos[j]))
                    {
                        count++;

                    }
                }
            }

            if(count%2 == 1)
            {
                list.Add(new Vec3(pointList[i].transform.position));
                Debug.Log("Check!");
            }
        }
    }

    bool IsPointInPlane(Llano llano, Vec3 origin, out Vec3 point)
    {
        point = Vec3.Zero;

        float denom = Vec3.Dot(llano.normal, direction);
        if (Mathf.Abs(denom) > Vec3.epsilon)
        {
            float t = Vec3.Dot((llano.normal * llano.distance - origin), llano.normal) / denom;
            if (t >= Vec3.epsilon)
            {
                point = origin + direction * t; 
                return true;
            }
        }
        return false;
    }

    private bool TrianglePointColision(Vec3 point, Llano llano)
    {
        float x1 = llano.a.x; float y1 = llano.a.y; float z1 = llano.a.z;
        float x2 = llano.b.x; float y2 = llano.b.y; float z2 = llano.b.z;
        float x3 = llano.c.x; float y3 = llano.c.y; float z3 = llano.c.z;

        // Area del triangulo
        float areaOrig = Mathf.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1));

        // Areas de los 3 triangulos hechos con el punto y las esquinas
        float area1 = Mathf.Abs((x1 - point.x) * (y2 - point.y) - (x2 - point.x) * (y1 - point.y));
        float area2 = Mathf.Abs((x2 - point.x) * (y3 - point.y) - (x3 - point.x) * (y2 - point.y));
        float area3 = Mathf.Abs((x3 - point.x) * (y1 - point.y) - (x1 - point.x) * (y3 - point.y));


        // Si la suma del area de los 3 triangulos es igual a la del original estamos adentro
        return Math.Abs(area1 + area2 + area3 - areaOrig) < Vec3.epsilon;
    }

    private void CompareList()
    {
        for (int i = 0; i < colisionPointsA.Count; i++)
        {
            for (int j = 0; j < colisionPointsB.Count; j++)
            {
                if (colisionPointsA[i] == colisionPointsB[j])
                {
                    Debug.Log("COLISION!");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pointList.Count; i++)
        {
            Gizmos.DrawLine(pointList[i].transform.position, direction);
        }
    }
}
