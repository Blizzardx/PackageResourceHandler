using System;
using System.Collections;
using UnityEngine;
namespace PackageResourceHandler
{
    public class PackageResourceWWWElement : IEnumerator
    {
        private WWW m_Loader;
        private float m_fStartTime;
        private float m_fTimeOut;
        private string m_strError;
        private string m_strUrl;

        public PackageResourceWWWElement(string url, float timeOut = 10.0f)
        {
            m_strUrl = url;
            m_Loader = new WWW(url);
            m_fStartTime = Time.realtimeSinceStartup;
            m_fTimeOut = timeOut;
        }
        public WWW GetRequest()
        {
            return m_Loader;
        }
        public bool IsTimeOut()
        {
            return Time.realtimeSinceStartup - m_fStartTime > m_fTimeOut;
        }
        public string GetError()
        {
            if(!string.IsNullOrEmpty(m_strError))
            {
                return m_strError;
            }
            return m_Loader.error;
        }
        public object Current { get; private set; }
        public bool MoveNext()
        {
            //return false;
            if(m_Loader.isDone)
            {
                return false;
            }
            if(!string.IsNullOrEmpty(m_Loader.error))
            {
                return false;
            }
            if(IsTimeOut())
            {
                m_strError = "resource time : " + m_strUrl;
                return false;
            }
            return true;
        }
        public void Reset()
        {
            
        }
    }
}