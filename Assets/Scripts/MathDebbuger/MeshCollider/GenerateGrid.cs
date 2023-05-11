using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GenerateGrid : MonoBehaviour
{

    [SerializeField] private float delta;
    [SerializeField] private GameObject sphere;
    [SerializeField] private int size;

    [SerializeField] private List<GameObject> list;
    private Vector3 newPos;
    // Start is called before the first frame update
    void Start()
    {
        GeneretaGrid();
    }

    // Update is called once per frame
    void Update()
    {
        //list.Add(Instantiate(sphere));
    }

    private void GeneretaGrid()
    {
        newPos = sphere.transform.position;

        for (int i = 0; i < size; i++) 
        {
            newPos.x += delta;

            for (int j = 0; j < size; j++)
            {
                newPos.y += delta;

                for (int k = 0; k < size; k++)
                {
                    newPos.z += delta;

                    list.Add(Instantiate(sphere, newPos, sphere.transform.rotation));
                }
                newPos.z = sphere.transform.position.z;
            }
            newPos.y = sphere.transform.position.y;
        }
    }
}
