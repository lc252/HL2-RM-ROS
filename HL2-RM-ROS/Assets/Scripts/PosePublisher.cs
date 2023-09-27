using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;


public class PosePublisher : MonoBehaviour
{
    private ROSConnection ros;
    public string topic;
    public float frequency = 30.0f;

    private double lastPublish = 0;

    DateTime k_unixEpoch = new DateTime(1970, 1, 1, 10, 0, 0, 0);

    public Text text = null;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topic);
    }

    // Update is called once per frame
    void Update()
    {
        var publishTime = (DateTime.Now - k_unixEpoch).TotalSeconds;

        if (text != null)
        {
            text.text = $"{publishTime}, {lastPublish}";
        }

        if (publishTime > lastPublish + 1.0f/frequency)
        {
            // Publish the  tf
            Vector3 pos = gameObject.transform.position;
            Quaternion rot = gameObject.transform.rotation;
            PoseMsg pose = new PoseMsg(
                new PointMsg(pos.z, -pos.x, pos.y),
                new QuaternionMsg(rot.z, -rot.x, rot.y, -rot.w)
                );

            var sec = (uint)publishTime;
            var nanosec = (uint)((publishTime - Math.Floor(publishTime)) * 1e9);
            HeaderMsg header = new HeaderMsg(0, new TimeMsg(sec, nanosec), "unity");

            PoseStampedMsg posemsg = new PoseStampedMsg(header, pose);

            ros.Publish(topic, posemsg);
            lastPublish = publishTime;
        }
    }
}
