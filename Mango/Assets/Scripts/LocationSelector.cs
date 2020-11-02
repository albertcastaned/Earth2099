using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using UnityEngine.Assertions;
using UnityEngine;

public class LocationSelector : MonoBehaviour
{
    
    public Vector2d[] locations;
    [SerializeField] private int zoom = 16;

    private void Awake(){
        Assert.IsNotNull(locations);
    }

    // Start is called before the first frame update
    void Start(){
        AbstractMap map = FindObjectOfType<AbstractMap>();
        if(map != null){
            int index = Random.Range(0, locations.Length);
            Vector2d location = locations[index];
            map.Initialize(location, zoom);
        }
    }
}
