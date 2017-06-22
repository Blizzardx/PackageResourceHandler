using System;
using System.Collections;
using UnityEngine;
namespace PackageResourceHandler
{
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
            Debug.Log(url);

            PackageResourceWWWElement loader = new PackageResourceWWWElement(url);

            yield return loader.GetRequest();

            if (!string.IsNullOrEmpty(loader.GetRequest().error))
            {
                Exception e = new Exception(loader.GetRequest().error);
                loader.GetRequest().Dispose();
                // done with error
                if (null != m_DoneCallback)
                {
                    m_DoneCallback(null, e);
                }
                yield break;
            }
            if (null != m_DoneCallback)
            {
                m_DoneCallback(loader.GetRequest().text, null);
                loader.GetRequest().Dispose();
            }
        }
        private string GetStreamingAssetPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "jar:file://" + Application.dataPath + "!/assets/";
                case RuntimePlatform.IPhonePlayer:
                    return "file://" + Application.dataPath + "/Raw/";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "file://" + Application.dataPath + "/StreamingAssets/";

            }
            return Application.dataPath + "/StreamingAssets/";
        }
    }
}