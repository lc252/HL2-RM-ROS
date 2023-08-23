using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
using System.Linq;

//using Unity.Robotics.ROSTCPConnector;
//using RosMessageTypes.Sensor;
//using RosMessageTypes.Std;
//using RosMessageTypes.BuiltinInterfaces;



public class PVCameraStream : MonoBehaviour
{
    private PhotoCapture photoCaptureObject = null;

    public Text text;

    private bool photoProcessed = true;

    // Start is called before the first frame update
    void Start()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            List<byte> imageBufferList = new List<byte>();
            Matrix4x4 intrinsics = new Matrix4x4();
            // Copy the raw IMFMediaBuffer data into our empty byte list.
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
            byte[] data = imageBufferList.ToArray();
            photoCaptureFrame.TryGetProjectionMatrix(out intrinsics);

            // In this example, we captured the image using the BGRA32 format.
            // So our stride will be 4 since we have a byte for each rgba channel.
            // The raw image data will also be flipped so we access our pixel data
            // in the reverse order.
            int stride = 4;
            float denominator = 1.0f / 255.0f;
            List<Color> colorArray = new List<Color>();
            List<int> colour = new List<int>();
            for (int i = imageBufferList.Count - 1; i >= 0; i -= stride)
            {
                float a = (int)(imageBufferList[i - 0]) * denominator;
                float r = (int)(imageBufferList[i - 1]) * denominator;
                float g = (int)(imageBufferList[i - 2]) * denominator;
                float b = (int)(imageBufferList[i - 3]); // * denominator;

                colour.Add(imageBufferList[i-1]);
                colour.Add(imageBufferList[i-2]);
                colour.Add(imageBufferList[i-3]);

                text.text = $"b: {b}";

                colorArray.Add(new Color(r, g, b, a));
            }
            // Now we could do something with the array such as texture.SetPixels() or run image processing on the list
        }
        photoProcessed = true;
        // take next photo (Async?)
        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    }
}
