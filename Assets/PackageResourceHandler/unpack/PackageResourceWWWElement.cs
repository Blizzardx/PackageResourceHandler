using System;
using System.Collections;
using UnityEngine;
namespace PackageResourceHandler
{
    public class PackageResourceWWWElement
    {
        private WWW m_Loader;
        private float m_fStartTime;
        private float m_fTimeOut;

        public PackageResourceWWWElement(string url, float timeOut = 10.0f)
        {
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
    }
}