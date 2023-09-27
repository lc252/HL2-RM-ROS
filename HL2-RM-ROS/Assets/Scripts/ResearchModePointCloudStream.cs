using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

#if ENABLE_WINMD_SUPPORT
using HL2UnityPlugin;
#endif

public class ResearchModePointCloudStream : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    HL2ResearchMode researchMode;
#endif

    enum DepthSensorMode
    {
        ShortThrow,
        LongThrow,
        None
    };
    [SerializeField] DepthSensorMode depthSensorMode = DepthSensorMode.ShortThrow;
    [SerializeField] bool enablePointCloud = true;

    private ROSConnection ros;
    public string pointcloud2Topic;
    // public string imgTopic;

    private DateTime k_unixEpoch = new DateTime(1970, 1, 1, 10, 0, 0, 0);

    void Start()
    {

#if ENABLE_WINMD_SUPPORT
        researchMode = new HL2ResearchMode();

        // Depth sensor should be initialized in only one mode
        if (depthSensorMode == DepthSensorMode.LongThrow) researchMode.InitializeLongDepthSensor();
        else if (depthSensorMode == DepthSensorMode.ShortThrow) researchMode.InitializeDepthSensor();
        
        researchMode.InitializeSpatialCamerasFront();
        // researchMode.SetReferenceCoordinateSystem(unityWorldOrigin);
        researchMode.SetPointCloudDepthOffset(0);

        // Depth sensor should be initialized in only one mode
        if (depthSensorMode == DepthSensorMode.LongThrow) researchMode.StartLongDepthSensorLoop(enablePointCloud);
        else if (depthSensorMode == DepthSensorMode.ShortThrow) researchMode.StartDepthSensorLoop(enablePointCloud);

        researchMode.StartSpatialCamerasFrontLoop();
#endif
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PointCloud2Msg>(pointcloud2Topic);

    }

    void LateUpdate()
    {
#if ENABLE_WINMD_SUPPORT
        // Update point cloud
        UpdatePointCloud();
#endif
    }

#if ENABLE_WINMD_SUPPORT
    private void UpdatePointCloud()
    {
        if (enablePointCloud)
        {
            if ((depthSensorMode == DepthSensorMode.LongThrow && !researchMode.LongThrowPointCloudUpdated()) ||
                (depthSensorMode == DepthSensorMode.ShortThrow && !researchMode.PointCloudUpdated())) return;

            float[] pointCloud = new float[] { };
            if (depthSensorMode == DepthSensorMode.LongThrow) pointCloud = researchMode.GetLongThrowPointCloudBuffer();
            else if (depthSensorMode == DepthSensorMode.ShortThrow) pointCloud = researchMode.GetPointCloudBuffer();

            // allocate space for byte array
            byte[] data = new byte[pointCloud.Length * sizeof(float)];
            // copy float array into byte array
            System.Buffer.BlockCopy(pointCloud, 0, data, 0, data.Length);

            // construct pointcloud message
            PointFieldMsg[] pointFields = new PointFieldMsg[3];
            pointFields[0] = new PointFieldMsg("x", 0, PointFieldMsg.FLOAT32, 1);
            pointFields[1] = new PointFieldMsg("y", 4, PointFieldMsg.FLOAT32, 1);
            pointFields[2] = new PointFieldMsg("z", 8, PointFieldMsg.FLOAT32, 1);

            var publishTime = (DateTime.Now - k_unixEpoch).TotalSeconds;
            var sec = (uint)publishTime;
            var nanosec = (uint)((publishTime - Math.Floor(publishTime)) * 1e9);
            HeaderMsg header = new HeaderMsg(0, new TimeMsg(sec, nanosec), "unity");

            // construct pointcloud message
            var pc2_msg = new PointCloud2Msg(
                header: header,
                height: 1,
                width: (uint)(pointCloud.Length / 3),
                fields: pointFields,
                is_bigendian: false,
                point_step: 12,
                row_step: (uint)(12 * (pointCloud.Length / 3)),
                data: data,
                is_dense: false
                );

            // Publish
            ros.Publish(pointcloud2Topic, pc2_msg);
        }
    }
#endif

#if WINDOWS_UWP
        private long GetCurrentTimestampUnix()
        {
            // Get the current time, in order to create a PerceptionTimestamp. 
            Windows.Globalization.Calendar c = new Windows.Globalization.Calendar();
            Windows.Perception.PerceptionTimestamp ts = Windows.Perception.PerceptionTimestampHelper.FromHistoricalTargetTime(c.GetDateTime());
            return ts.TargetTime.ToUnixTimeMilliseconds();
            //return ts.SystemRelativeTargetTime.Ticks;
        }
        private Windows.Perception.PerceptionTimestamp GetCurrentTimestamp()
        {
            // Get the current time, in order to create a PerceptionTimestamp. 
            Windows.Globalization.Calendar c = new Windows.Globalization.Calendar();
            return Windows.Perception.PerceptionTimestampHelper.FromHistoricalTargetTime(c.GetDateTime());
        }
#endif
}