using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class BakeRuntime : MonoBehaviour
{

    private AbstractMap map;
    public bool loaded = false;
    void Start()
    {
        map = GetComponent<AbstractMap>();
        StartCoroutine("WaitForMap");
    }

    IEnumerator WaitForMap()
    {
        Debug.Log(loaded);
        yield return new WaitUntil(() => map.MapVisualizer.State == ModuleState.Finished);
        print("Map loaded, baking navmesh...");

        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        if (surface.navMeshData != null)
        {
            Debug.Log("Navmesh already built");
        }
        else
        {
            surface.BuildNavMesh();
            Debug.Log("Navmesh succesfully baked.");
        }
        loaded = true;
    }
}
