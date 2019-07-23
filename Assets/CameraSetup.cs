using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    private GameObject maincamera;
    private Camera cameraobj;

    public float focalLength = 6.33f;


    

    // Start is called before the first frame update
    void Start()
    {
        maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject.Find("BackgroundPlane");
        cameraobj = maincamera.GetComponents<Camera>()[0];
    }
    
    // Update is called once per frame
    void Update()
    {
        cameraobj.usePhysicalProperties = true;
        cameraobj.focalLength = focalLength;
    }
}
