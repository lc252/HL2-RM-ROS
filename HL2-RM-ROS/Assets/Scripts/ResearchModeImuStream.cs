using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

#if ENABLE_WINMD_SUPPORT
using HL2UnityPlugin;
#endif

public class ResearchModeImuStream : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    HL2ResearchMode researchMode;
#endif
    private float[] accelSampleData;
    private Vector3Msg accelMsg = new Vector3Msg();

    private float[] gyroSampleData;
    private Vector3Msg gyroMsg = new Vector3Msg();

    private float[] magSampleData;
    private Vector3Msg magMsg = new Vector3Msg();

    public Text AccelText = null;
    public Text GyroText = null;
    public Text MagText = null;

    private ROSConnection ros;
    public string imuTopic;
    public string magTopic;

    // public ImuVisualize RefImuVisualize = null;

    void Start()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode = new HL2ResearchMode();
        researchMode.InitializeAccelSensor();
        researchMode.InitializeGyroSensor();
        researchMode.InitializeMagSensor();

        researchMode.StartAccelSensorLoop();
        researchMode.StartGyroSensorLoop();
        researchMode.StartMagSensorLoop();
#endif
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImuMsg>(imuTopic);
        ros.RegisterPublisher<MagneticFieldMsg>(magTopic);
    }

    void LateUpdate()
    {
#if ENABLE_WINMD_SUPPORT
            // update Accel Sample
            if (researchMode.AccelSampleUpdated())
            {
                accelSampleData = researchMode.GetAccelSample();
                if (accelSampleData.Length == 3)
                {
                    AccelText.text = $"Accel : {accelSampleData[0]:F3}, {accelSampleData[1]:F3}, {accelSampleData[2]:F3}";
                    accelMsg.x = accelSampleData[2];
                    accelMsg.y = -1.0f * accelSampleData[0];
                    accelMsg.z = -1.0f * accelSampleData[1];
                }
            }

            // update Gyro Sample
            if (researchMode.GyroSampleUpdated())
            {
                gyroSampleData = researchMode.GetGyroSample();
                if (gyroSampleData.Length == 3)
                {
                    GyroText.text = $"Gyro  : {gyroSampleData[0]:F3}, {gyroSampleData[1]:F3}, {gyroSampleData[2]:F3}";
                    gyroMsg.x = gyroSampleData[2];
                    gyroMsg.y = gyroSampleData[0];
                    gyroMsg.z = gyroSampleData[1];
                }
            }

            // update Mag Sample
            if (researchMode.MagSampleUpdated())
            {
                magSampleData = researchMode.GetMagSample();
                if (magSampleData.Length == 3)
                {
                    MagText.text = $"Mag   : {magSampleData[0]:F3}, {magSampleData[1]:F3}, {magSampleData[2]:F3}";
                    magMsg.x = magSampleData[0];
                    magMsg.y = magSampleData[1];
                    magMsg.z = magSampleData[2];
                }
            }

            // use Unity time for now
            uint timeSec = (uint)Time.timeAsDouble;
            uint timeNanoSec = (uint)((Time.timeAsDouble - timeSec)*1e9);

            // construct ROS messages
            double[] cov = { 0.01, 0, 0, 0, 0.01, 0, 0, 0, 0.01 };
            HeaderMsg header = new HeaderMsg(0, new TimeMsg(timeSec, timeNanoSec), "map");
            QuaternionMsg nullQ = new QuaternionMsg(-1, 0, 0, 0);

            ImuMsg imu = new ImuMsg(
                header,                   // header
                nullQ,                    // orientation est
                cov,                      // orientation cov
                gyroMsg,                  // angular vel
                cov,                      // angular cov
                accelMsg,                 // linear acc
                cov);                     // linear cov


            MagneticFieldMsg mag = new MagneticFieldMsg(
                header,
                magMsg,
                cov);

            // Publish
            ros.Publish(imuTopic, imu);
            ros.Publish(magTopic, mag);
#endif
    }

    public void StopSensorsEvent()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode.StopAllSensorDevice();
#endif
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus) StopSensorsEvent();
    }
}