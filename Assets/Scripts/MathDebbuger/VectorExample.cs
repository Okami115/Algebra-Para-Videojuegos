using System;
using CustomMath;
using UnityEngine;

public class VectorExample : MonoBehaviour
{
    [SerializeField] private Transform a;
    [SerializeField] private Transform b;
    [SerializeField] private Transform aux;

    private Vec3 vecA;
    private Vec3 vecB;
    private Vec3 vecAux;

    [SerializeField] private example index;

    [SerializeField] private float velocity = 500f;
    private float t = 1;

    [Serializable] private enum example
    {
        Addition,
        Subtraction,
        Multiplication,
        Cross,
        Lerp,
        Max,
        Projection,
        Distance,
        Reflect,
        LerpUnclamped,
    }

    private void Update()
    {
        vecA = new Vec3(a.position);
        vecB = new Vec3(b.position);

        switch(index)
        {
            case example.Addition:
                {
                    Addition();
                    break;
                }
            case example.Subtraction: 
                {
                    Subtraction();
                    break;
                }
            case example.Multiplication:
                {
                    Multiplication();
                    break;
                }
            case example.Cross:
                {
                    Cross();
                    break;
                }
            case example.Lerp:
                {
                    Lerp();
                    break;
                }
            case example.Max:
                {
                    Max();
                    break;
                }
            case example.Projection:
                {
                    Projection();
                    break;
                }
            case example.Distance:
                {
                    Distance();
                    break;
                }
            case example.Reflect:
                {
                    Reflect();
                    break;
                }
            case example.LerpUnclamped:
                {
                    LerpUnclaped();
                    break;
                }
        }

        aux.position = new Vector3(vecAux.x, vecAux.y, vecAux.z);
    }


    private void Addition()
    {
        vecAux = vecA + vecB;
    }

    private void Subtraction()
    {
        vecAux = vecB - vecA;
    }

    private void Multiplication()
    {
        var scale = vecA;

        scale.Scale(vecB);

        vecAux = scale;
    }

    private void Cross ()
    {
        vecAux = Vec3.Cross(vecB, vecA);
    }

    private void Lerp()
    {
        t += Time.deltaTime;

        vecAux = Vec3.Lerp(vecA, vecB, t);

        if (t >= 1)
        {
            t = 0;
        }
    }

    private void Max()
    {
        vecAux = Vec3.Max(vecA, vecB);
    }

    private void Projection()
    {
        vecAux = Vec3.Project(vecA, vecB);
    }

    private void Distance()
    {
        vecAux = vecA + vecB;

        vecAux = vecAux.normalized * Vec3.Distance(vecA, vecB);
    }

    private void Reflect()
    {
        vecAux = Vec3.Reflect(vecA, vecB.normalized);
    }

    private void LerpUnclaped()
    {
        t -= Time.deltaTime;

        vecAux = Vec3.LerpUnclamped(vecA, vecB, t);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(a.position, Vector3.zero);
        Gizmos.DrawLine(b.position, Vector3.zero);
        Gizmos.DrawLine(aux.position, Vector3.zero);
    }
}
