// Adapted from a script written by Hunter de Jong

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Microsoft.MixedReality.Toolkit;
// using Microsoft.MixedReality.Toolkit.Diagnostics;
// using Microsoft.MixedReality.Toolkit.Input;
// using Microsoft.MixedReality.Toolkit.UI;


public class ABBMeshProvider : MonoBehaviour
{
    public GameObject base_link_Mesh;
    public GameObject link_1_Mesh;
    public GameObject link_2_Mesh;
    public GameObject link_3_Mesh;
    public GameObject link_4_Mesh;
    public GameObject link_5_Mesh;
    public GameObject link_6_Mesh;

    public string base_link_Name;
    public string link_1_Name;
    public string link_2_Name;
    public string link_3_Name;
    public string link_4_Name;
    public string link_5_Name;
    public string link_6_Name;

    private IDictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        objects.Add(base_link_Name, base_link_Mesh);
        objects.Add(link_1_Name, link_1_Mesh);
        objects.Add(link_2_Name, link_2_Mesh);
        objects.Add(link_3_Name, link_3_Mesh);
        objects.Add(link_4_Name, link_4_Mesh);
        objects.Add(link_5_Name, link_5_Mesh);
        objects.Add(link_6_Name, link_6_Mesh);
        // CoreServices.DiagnosticsSystem.ShowProfiler = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<string, GameObject> kvp in objects)
        {
            setMesh(kvp.Key, kvp.Value);
        }
    }


    private void setMesh(string objectName, GameObject objectMesh)
    {
        if (!objectMesh.activeSelf && GameObject.Find(objectName) != null)
        {
            // Get Object
            var objectvar = GameObject.Find(objectName);

            objectMesh.transform.parent = objectvar.transform;

            float offset = (objectName == "link_4") ? -0.1f : 0.0f;

            objectMesh.transform.localPosition = new Vector3(0.0f, 0.0f, offset);
            objectMesh.transform.localRotation = new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f);

            objectMesh.SetActive(true);
        }
    }

}
