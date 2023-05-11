using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using CustomMath;
using System.Diagnostics;

public class Resolucion : MonoBehaviour
{
    [SerializeField] private Transform a;
    [SerializeField] private Transform b;
    [SerializeField] private Transform aux;

    private Vec3 castA;
    private Vec3 castB;
    private Vec3 castAux;

    [SerializeField] private int index;

    [SerializeField] private float velocity = 500f;
    private float t = 1;
    private void Start()
    {
        //aux.position = a.position; //Cinco
    }

    private void Update()
    {
        castA = new Vec3(a.position);
        castB = new Vec3(b.position);

        switch(index)
        {
            case 1:
                {
                    Uno();
                    break;
                }
            case 2: 
                {
                    Dos();
                    break;
                }
            case 3:
                {
                    Tres();
                    break;
                }
            case 4:
                {
                    Cuatro();
                    break;
                }
            case 5:
                {
                    Cinco();
                    break;
                }
            case 6:
                {
                    Seis();
                    break;
                }
            case 7:
                {
                    Siete();
                    break;
                }
            case 8:
                {
                    Ocho();
                    break;
                }
            case 9:
                {
                    Nueve();
                    break;
                }
            case 10:
                {
                    Diez();
                    break;
                }
        }

        aux.position = new Vector3(castAux.x, castAux.y, castAux.z);
    }


    private void Uno()
    {
        castAux = castA + castB;
    }

    private void Dos()
    {
        castAux = castB - castA;
    }

    private void Tres()
    {
        var scale = castA;

        scale.Scale(castB);

        castAux = scale;
    }

    private void Cuatro ()
    {
        castAux = Vec3.Cross(castB, castA);
    }

    private void Cinco()
    {
        var diff = castAux - castA;

        if (diff == Vec3.Zero)
        {
            castAux = castB;
        }
        else
        {
            castAux -= diff.normalized * velocity * Time.deltaTime;    
        }
    }

    private void Seis()
    {
        castAux = Vec3.Max(castA, castB);
    }

    private void Siete()
    {
        castAux = Vec3.Project(castA, castB);
    }

    private void Ocho()
    {
        castAux = castA + castB;

        castAux = castAux.normalized * Vec3.Distance(castA, castB);
    }

    private void Nueve()
    {
        castAux = Vec3.Reflect(castA, castB.normalized);
    }

    private void Diez()
    {
        t -= Time.deltaTime;

        castAux = Vec3.LerpUnclamped(castA, castB, t);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(a.position, Vector3.zero);
        Gizmos.DrawLine(b.position, Vector3.zero);
        Gizmos.DrawLine(aux.position, Vector3.zero);
    }
}
