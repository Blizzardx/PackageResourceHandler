using Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackageResourceUnpacker
{
    private const string m_strPackageResourceConfigName = "PackageResourceConfig";
    private const string m_strUnpackedResourceConfigInfoName = "UnpackedResourceConfig";

    private string m_strUnpackedVersion;
    private GameObject m_Root;
    private Action m_DoneCallback;
    private Action<Exception> m_CrashCallback;
    private Action<float, string> m_ProcessCallback;

    public void UnpackPackageResource(Action doneCallback,Action<Exception> crashCallback, Action<float, string> processCallback)
    {
        m_DoneCallback = doneCallback;
        m_CrashCallback = crashCallback;
        m_ProcessCallback = processCallback;
        try
        {
            // try load config from streaming assets and local
            string localPath = Application.persistentDataPath + "/" + m_strUnpackedResourceConfigInfoName;

            if (!File.Exists(localPath))
            {
                OnComplate();
                return;
            }
            var fileContent = File.ReadAllText(localPath);
            if (string.IsNullOrEmpty(fileContent))
            {
                OnComplate();
                return;
            }

            m_strUnpackedVersion = fileContent;

            BeginLoadPackedConfigInfo();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnComplate();
            return;
        }
        
    }

    private void BeginLoadPackedConfigInfo()
    {
        if(null == m_Root)
        {
            m_Root = new GameObject("PackageResourceUnpacker");
            m_Root.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(m_Root);
        }
        var tmpLoader = m_Root.AddComponent<PackageResourceLoader>();
        tmpLoader.BeginLoadResourceFromStreamingAssets(OnLoadPackedConfigInfoDone, m_strPackageResourceConfigName);
    }
    private void OnLoadPackedConfigInfoDone(string content, Exception e)
    {
        var tmpLoader = m_Root.GetComponent<PackageResourceLoader>();
        if(null != tmpLoader)
        {
            GameObject.Destroy(tmpLoader);
        }

        if (null != e || string.IsNullOrEmpty(content))
        {
            Debug.LogError("cant' load streaming asset info config");
            OnComplate();
            return;
        }

        try
        {
            var config = XmlConfigBase.DeSerialize<PackageResourceConfig>(content);
            if(null == config)
            {
                Debug.LogError("cant' load streaming asset info config");
                OnComplate();
                return;
            }

            DoUnpack(config);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            OnComplate();
        }
    }
    private void DoUnpack(PackageResourceConfig config)
    {
        if (config.version == m_strUnpackedVersion)
        {
            // do noting
            OnComplate();
            return;
        }
        // mark new version
        m_strUnpackedVersion = config.version;

        var tmpLoader = m_Root.GetComponent<PackageResourceUnpackerTool>();
        if(null != tmpLoader)
        {
            GameObject.Destroy(tmpLoader);
        }
        tmpLoader = m_Root.AddComponent<PackageResourceUnpackerTool>();

        tmpLoader.SetAllSucceedCallback(OnAllSucceedCallback);
        tmpLoader.SetErrorCallback(OnErrorCallback);
        tmpLoader.SetProcessCallback(OnProcessCallback);

        tmpLoader.LoadAssetToFileSystem(Application.persistentDataPath + "/", config.fileList, null);
    }
    private void OnProcessCallback(float process, string currentFile)
    {
        Debug.LogFormat("Process : {0} file : {1}", process, currentFile);
    }
    private void OnErrorCallback(List<PackageResourceLoadErrorInfo> obj)
    {
        Exception e = null;
        for(int i=0;i<obj.Count;)
        {
            e = new Exception(obj[i].GetError());
            break;
        }
        if(null ==e)
        {
            e = new Exception("unpack with error ");
        }
        OnCrash(e);
    }
    private void OnAllSucceedCallback()
    {
        // all done
        try
        {
            // write version file
            File.WriteAllText(Application.persistentDataPath + "/" + m_strUnpackedResourceConfigInfoName, m_strUnpackedVersion);

            // done call back
            OnComplate();
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            OnCrash(e);
        }
    }
    private void OnComplate()
    {
        if(null != m_Root)
        {
            GameObject.Destroy(m_Root);
        }
        if(null != m_DoneCallback)
        {
            m_DoneCallback();
        }
    }
    private void OnCrash(Exception e)
    {
        if (null != m_Root)
        {
            GameObject.Destroy(m_Root);
        }
        if (null != m_CrashCallback)
        {
            m_CrashCallback(e);
        }
    }
}