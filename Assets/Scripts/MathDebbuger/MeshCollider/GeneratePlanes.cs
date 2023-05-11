using CustomMath;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneratePlanes : MonoBehaviour
{
    [SerializeField] private Mesh mesh;

    [SerializeField] private List<Llano> listLlanos;


    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        listLlanos = new List<Llano>();
    }

    // Update is called once per frame
    void Update()
    {
        SetLlano();
    }

    private void SetLlano()
    { 
        listLlanos.Clear();

        for(int i = 0; i < mesh.GetIndexCount(0); i+= 3) 
        {
           Vec3 a = new Vec3 (transform.TransformPoint(mesh.vertices[mesh.GetIndices(0)[i]]));
           Vec3 b = new Vec3 (transform.TransformPoint(mesh.vertices[mesh.GetIndices(0)[i + 1]]));
           Vec3 c = new Vec3 (transform.TransformPoint(mesh.vertices[mesh.GetIndices(0)[i + 2]]));
            
            Llano aux = new Llano(a ,b ,c);

            aux.normal *= -1;
            aux.Flip();
            
           listLlanos.Add(aux);
        }

    }

    private void OnDrawGizmos()
    {
        var color = Color.red;
        foreach (var VARIABLE in listLlanos)
        {
            DrawPlane(VARIABLE.normal * VARIABLE.distance, VARIABLE.normal, Color.red);
        }
    }

    public void DrawPlane(Vec3 position, Vec3 normal, Color color)
    {
        Vector3 v3;
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;
        Debug.DrawLine(corner0, corner2, color);
        Debug.DrawLine(corner1, corner3, color);
        Debug.DrawLine(corner0, corner1, color);
        Debug.DrawLine(corner1, corner2, color);
        Debug.DrawLine(corner2, corner3, color);
        Debug.DrawLine(corner3, corner0, color);
        Debug.DrawRay(position, normal, Color.magenta);
    }
}
