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

public class ResearchModeABImageStream : MonoBehaviour
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

    private byte[] shortAbImageFrameData = null;

    // private byte[] longDepthFrameData = null;

    // private byte[] longAbImageFrameData = null;

    // private byte[] LFFrameData = null;

    // private byte[] RFFrameData = null;

    // public Text text;

    private ROSConnection ros;
    // public string pointcloud2Topic;
    public string imgTopic;




    void Start()
    {

#if ENABLE_WINMD_SUPPORT
        researchMode = new HL2ResearchMode();

        // Depth sensor should be initialized in only one mode
        if (depthSensorMode == DepthSensorMode.LongThrow) researchMode.InitializeLongDepthSensor();
        else if (depthSensorMode == DepthSensorMode.ShortThrow) researchMode.InitializeDepthSensor();
        
        // researchMode.InitializeSpatialCamerasFront();
        // researchMode.SetReferenceCoordinateSystem(unityWorldOrigin);
        // researchMode.SetPointCloudDepthOffset(0);

        // Depth sensor should be initialized in only one mode
        if (depthSensorMode == DepthSensorMode.LongThrow) researchMode.StartLongDepthSensorLoop(enablePointCloud);
        else if (depthSensorMode == DepthSensorMode.ShortThrow) researchMode.StartDepthSensorLoop(enablePointCloud);

        // researchMode.StartSpatialCamerasFrontLoop();
#endif
        // ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImageMsg>(imgTopic);
    }

    void LateUpdate()
    {
#if ENABLE_WINMD_SUPPORT
        // Update ABImage
        UpdateABImage();
#endif
    }

#if ENABLE_WINMD_SUPPORT
    private void UpdateABImage()
    {
        if (!researchMode.ShortAbImageTextureUpdated())
        {
            // text.text = "not updated";
            return;
        }
        // get image
        byte[] frameTexture = researchMode.GetShortAbImageTextureBuffer();
        if (frameTexture.Length > 0)
        {
            if (shortAbImageFrameData == null)
            {                    
                shortAbImageFrameData = frameTexture;
            }
            else
            {
                System.Buffer.BlockCopy(frameTexture, 0, shortAbImageFrameData, 0, shortAbImageFrameData.Length);
            }
        }

        var publishTime = (DateTime.Now - k_unixEpoch).TotalSeconds;
        var sec = (uint)publishTime;
        var nanosec = (uint)((publishTime - Math.Floor(publishTime)) * 1e9);
        HeaderMsg header = new HeaderMsg(0, new TimeMsg(sec, nanosec), "unity");

        // construct image message
        var img_msg = new ImageMsg(
            header: header,
            height: 512,
            width: 512,
            encoding: "mono8",
            is_bigendian: 0,
            step: 512,
            data: shortAbImageFrameData);

        ros.Publish(imgTopic, img_msg);
        // text.text = $"{img_msg.header.stamp.sec}, {img_msg.header.stamp.nanosec}";
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