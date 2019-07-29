using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    private static GameObject maincamera;
    private static GameObject backgroundplane;
    private static Camera cameraobj;

    public float focalLength = 6.33f;
    public float backgroundPlaneDistance = 1274;

    private static bool programStarted = false;


    

    // Start is called before the first frame update
    void Start()
    {
        maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraobj = maincamera.GetComponents<Camera>()[0];
    }
    
    // Update is called once per frame
    void Update()
    {
        if(!programStarted)
        {
            backgroundplane = GameObject.Find("BackgroundPlane");
            programStarted = true;
        }
        cameraobj.usePhysicalProperties = true;
        cameraobj.focalLength = focalLength;
        backgroundplane.transform.localPosition = new Vector3(backgroundplane.transform.localPosition.x, backgroundplane.transform.localPosition.y, backgroundPlaneDistance);
    }
}
