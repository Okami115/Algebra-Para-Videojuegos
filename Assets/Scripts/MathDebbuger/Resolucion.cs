using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Resolucion : MonoBehaviour
{
    [SerializeField] private Transform a;
    [SerializeField] private Transform b;
    [SerializeField] private Transform aux;

    [SerializeField] private float velocity = 500f;
    private float t = 1;
    private void Start()
    {
        //aux.position = a.position; //Cinco
    }

    private void Update()
    {
        //Uno();
        //Dos();
        //Tres();
        //Cuatro();
        //Cinco();
        //Seis();
        //Siete();
        //Ocho();
        //Nueve();
        //Diez();
    }


    private void Uno()
    {
        aux.position = a.position + b.position;
    }

    private void Dos()
    {
        aux.position = b.position - a.position;
    }

    private void Tres()
    {
        var scale = a.position;

        scale.Scale(b.position);

        aux.position = scale;
    }

    private void Cuatro ()
    {
        aux.position = Vector3.Cross(b.position, a.position);
    }

    private void Cinco()
    {
        var diff = aux.position - a.position;

        if (diff == Vector3.zero)
        {
            aux.position = b.position;
        }
        else
        {
            aux.position -= diff.normalized * velocity * Time.deltaTime;    
        }
    }

    private void Seis()
    {
        aux.position = Vector3.Max(a.position, b.position);
    }

    private void Siete()
    {
        aux.position = Vector3.Project(a.position, b.position);
    }

    private void Ocho()
    {
        aux.position = a.position + b.position;

        aux.position = aux.position.normalized * Vector3.Distance(a.position, b.position);
    }

    private void Nueve()
    {
        aux.position = Vector3.Reflect(a.position, b.position.normalized);
    }

    private void Diez()
    {
        t -= Time.deltaTime;

        aux.position = Vector3.LerpUnclamped(a.position, b.position, t);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(a.position, Vector3.zero);
        Gizmos.DrawLine(b.position, Vector3.zero);
        Gizmos.DrawLine(aux.position, Vector3.zero);
    }
}
