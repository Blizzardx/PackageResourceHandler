using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PackPackageResourceHelper
{
    [MenuItem("PackageResourceHelper/Pack")]
    static void test()
    {
        string sourceDirectory = "C:\\Users\\Administrator\\AppData\\LocalLow\\pwrd\\gamecqq\\";
        string targetDirectory = "E:\\Project\\unityProjcet\\PackageResourceHandler\\Assets\\StreamingAssets\\";
        PackageResourcePacker tool = new PackageResourcePacker();
        tool.PackPackageResource(PackageResourceCompressType.None, sourceDirectory, targetDirectory);

        Debug.Log("done");
        return;
        PackageResourceAssetInfo elem = new PackageResourceAssetInfo(
            "C:\\Users\\Administrator\\AppData\\LocalLow\\pwrd\\gamecqq\\Download\\config\\achieveConfig_txtpkg.bytes", 
            "C:\\Users\\Administrator\\AppData\\LocalLow\\pwrd\\gamecqq\\",
            "E:\\Project\\unityProjcet\\PackageResourceHandler\\Assets\\StreamingAssets\\");

        Debug.Log(elem.GetSubpath());
        Debug.Log(elem.GetSourcePath());
        Debug.Log(elem.GetOutputPath());
    }
}
