using System;
namespace PackageResourceHandler
{

    public class PackageResourceLoaderElement
    {
        private string m_strOutputPath;
        private string m_strUrl;
        private object m_Parmater;
        private string m_strErrorInfo;
        private string m_strName;
        private Action<PackageResourceLoaderElement> m_Callback;

        public PackageResourceLoaderElement(string name, string url, string outputPath)
        {
            m_strName = name;
            m_strUrl = url;
            m_strOutputPath = outputPath;
            m_strErrorInfo = null;
        }

        internal string GetOutputPath()
        {
            return m_strOutputPath;
        }
        internal string GetUrl()
        {
            return m_strUrl;
        }

        internal void SetErrorInfo(string errorInfo)
        {
            m_strErrorInfo = errorInfo;
        }

        internal string GetName()
        {
            return m_strName;
        }

        internal string GetError()
        {
            return m_strErrorInfo;
        }
        internal bool HavError()
        {
            return !string.IsNullOrEmpty(m_strErrorInfo);
        }
        internal void ClearError()
        {
            m_strErrorInfo = null;
        }
    }
}