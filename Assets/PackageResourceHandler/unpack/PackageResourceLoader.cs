using System;
using System.Collections;
using UnityEngine;

public class PackageResourceLoader : MonoBehaviour
{
    private Action<string, Exception> m_DoneCallback;

    public void BeginLoadResourceFromStreamingAssets(Action<string, Exception> donecallback, string url)
    {
        m_DoneCallback = donecallback;
        StartCoroutine(BeginLoad(GetStreamingAssetPath() + url));
    }
    private IEnumerator BeginLoad(string url)
    {
        WWW loader = new WWW(url);

        yield return loader;

        if (!string.IsNullOrEmpty(loader.error))
        {
            Exception e = new Exception(loader.error);
            loader.Dispose();
            // done with error
            if (null != m_DoneCallback)
            {
                m_DoneCallback(null, e);
            }
            yield break;
        }
        if (null != m_DoneCallback)
        {
            m_DoneCallback(loader.text, null);
            loader.Dispose();
        }
    }
    private string GetStreamingAssetPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "jar:file://" + Application.dataPath + "!/assets/";
            case RuntimePlatform.IPhonePlayer:
                return Application.dataPath + "/Raw/";
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return Application.dataPath + "/StreamingAssets/";

        }
        return Application.dataPath + "/StreamingAssets/";
    }
}
