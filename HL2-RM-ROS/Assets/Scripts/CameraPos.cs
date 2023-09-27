using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPos : MonoBehaviour
{

    public Text text;
    private Camera m_camera;
    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"{m_camera.transform.position.z}, {-m_camera.transform.position.x}, {m_camera.transform.position.y}";
    }
}
