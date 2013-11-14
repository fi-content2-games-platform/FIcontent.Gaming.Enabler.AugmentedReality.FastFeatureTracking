using UnityEngine;
using System.Collections;

public class BorderTexture : MonoBehaviour
{

    //public enum DeviceType { standalone,iphone, iphone3g, iphone3gs, iphone4, iphone4s, ipad, ipad2, ipad3 }

    //public DeviceType deviceType;

    public bool lockX;
    public bool lockY;
    public bool lockZ;

    // Use this for initialization
    void Start()
    {

        //string deviceModel = SystemInfo.deviceModel;

        //if (deviceModel.StartsWith("iPhone3")) //iPhone 4
        //    deviceType = DeviceType.iphone4;
        //else if (deviceModel.StartsWith("iPhone4")) //iPhone 4s
        //    deviceType = DeviceType.iphone4s;
        //else if (deviceModel.StartsWith("iPhone2")) //iPhone 3gs
        //    deviceType = DeviceType.iphone3gs;
        //else if (deviceModel.StartsWith("iPhone1,2")) //iPhone 3g
        //    deviceType = DeviceType.iphone3g;
        //else if (deviceModel.StartsWith("iPhone1,1")) //iPhone
        //    deviceType = DeviceType.iphone;
        //else if (deviceModel.StartsWith("iPad3"))
        //    deviceType = DeviceType.ipad3;
        //else if (deviceModel.StartsWith("iPad2"))
        //    deviceType = DeviceType.ipad2;
        //else if (deviceModel.StartsWith("iPad1"))
        //    deviceType = DeviceType.ipad;
        //else
        //    deviceType = DeviceType.standalone;

        this.transform.localScale = new Vector3(
            lockX ? this.transform.localScale.x : Screen.width,
            lockY ? this.transform.localScale.y : Screen.height,
            lockZ ? this.transform.localScale.z : 0 );

    }

    // Update is called once per frame
    void Update()
    {

    }
}
