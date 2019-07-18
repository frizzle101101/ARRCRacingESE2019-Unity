using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class InitUVCCAmera : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
       // GameObject maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        //VuforiaRuntime.Instance.Deinit();
        //maincamera.GetComponent<VuforiaBehaviour>().enabled = false;
        //maincamera.GetComponent<DefaultInitializationErrorHandler>().enabled = false;

        VuforiaUnity.SetDriverLibrary("libUVCDriver.so");
        
        //maincamera.GetComponent<VuforiaBehaviour>().enabled = true;
        //maincamera.GetComponent<DefaultInitializationErrorHandler>().enabled = true;
        VuforiaRuntime.Instance.InitVuforia();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
