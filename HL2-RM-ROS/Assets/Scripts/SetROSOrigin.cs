using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;



public class SetROSOrigin : MonoBehaviour
{
    public GameObject originSeedObject;
    private GameObject world;

    private ROSConnection ros;
    private string registeredTopic = "alignment_registered";
    private bool set;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseStampedMsg>(registeredTopic, AlignmentCallback);
        set = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!set && GameObject.Find("base_link") != null)
        {
            // Get Object
            world = GameObject.Find("base_link");
            // set its parent to the guess transform
            world.transform.parent = originSeedObject.transform;

            // set the initial local transform to be coincident with guess
            Vector3 initial_pos = new Vector3(0, 0, 0);
            Quaternion initial_rot = new Quaternion(0, 0, 0, 1);
            world.transform.localPosition = initial_pos;
            world.transform.localRotation = initial_rot;

            set = true;
        }
    }

    void AlignmentCallback(PoseStampedMsg pose_aligned)
    {
        if (set)
        {
            // update the world local transform with the correction transform
            //var pose = pose_aligned.pose;
            //Vector3 pos = new Vector3((float)pose.position.x, (float)pose.position.y, (float)pose.position.z);
            //Quaternion rot = new Quaternion((float)pose.orientation.x, (float)pose.orientation.y, (float)pose.orientation.z, (float)pose.orientation.w);
            //world.transform.localPosition = pos;
            //world.transform.localRotation = rot;

            // testing
            Vector3 dummypos = new Vector3(0, 0, 0);
            Quaternion dummyrot = new Quaternion(0, 0, 0, 1);
            world.transform.localPosition = dummypos;
            world.transform.localRotation = dummyrot;
        }
    }
}
