using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetROSOrigin : MonoBehaviour
{
    public GameObject origin;
    private GameObject world;
    private bool set;

    // Start is called before the first frame update
    void Start()
    {
        set = false;
    }

    // Update is called once per frame
    void Update()
    {      
        if (!set && GameObject.Find("world") != null)
        {
            // Get Object
            world = GameObject.Find("world");
            world.transform.parent = origin.transform;

            world.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            world.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            set = true;
        }
    }
}
