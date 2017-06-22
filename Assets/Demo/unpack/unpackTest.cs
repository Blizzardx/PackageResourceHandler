using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unpackTest : MonoBehaviour {

    private float process;
    private string file;

	// Use this for initialization
	void Start () {

        PackageResourceUnpacker tool = new PackageResourceUnpacker();

        tool.UnpackPackageResource(OnDoneCallback, OnCrashCallback, ProcessCallback);
    }

    private void OnDoneCallback()
    {
        Debug.Log("Done");
    }

    private void OnCrashCallback(Exception e)
    {
        Debug.LogException(e);
    }

    private void ProcessCallback(float process, string file)
    {
        Debug.LogFormat("process : {0} file : {1} ", process, file);
        this.process = process;
        this.file = file;
    }
    void OnGUI()
    {
        string text = string.Format("process : {0} file : {1} ", process, file);
        GUI.TextArea(new Rect(0, 0, 300, 300),text);
    }
    // Update is called once per frame
    void Update () {
	}
}
