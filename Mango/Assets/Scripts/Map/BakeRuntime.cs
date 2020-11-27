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
    public int navmeshIterationsPerFrame = 400;
    void Start()
    {
        transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        map = GetComponent<AbstractMap>();
        StartCoroutine("WaitForMap");
    }

    IEnumerator WaitForMap()
    {
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
        NavMesh.pathfindingIterationsPerFrame = navmeshIterationsPerFrame;
        loaded = true;
    }
}
