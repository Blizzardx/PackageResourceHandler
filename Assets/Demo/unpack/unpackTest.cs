using System;
using UnityEngine;

namespace PackageResourceHandler
{
    public class unpackTest : MonoBehaviour
    {

        private float process;
        private string file;
        private string log;
        private string error;

        // Use this for initialization
        void Start()
        {
            error = "noerror";
            log = "Processing";
            PackageResourceUnpacker tool = new PackageResourceUnpacker();

            tool.UnpackPackageResource(OnDoneCallback, OnCrashCallback, ProcessCallback);
        }

        private void OnDoneCallback()
        {
            Debug.Log("Done");
            log = "Done ";
        }

        private void OnCrashCallback(Exception e)
        {
            Debug.LogException(e);
            error = e.Message;
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
            GUI.TextArea(new Rect(0, 0, 700, 100), text);
            GUI.TextArea(new Rect(0, 100, 700, 100), log);
            GUI.TextArea(new Rect(0, 200, 700, 100), error);
        }
        // Update is called once per frame
        void Update()
        {
            Debug.Log(Time.time);
        }
    }
}