namespace PackageResourceHandler
{
    public class PackageResourceLoadErrorInfo
    {
        public PackageResourceLoadErrorInfo(string name, string error)
        {
            m_strName = name;
            m_strError = error;
        }

        public string GetName()
        {
            return m_strName;
        }
        public string GetError()
        {
            return m_strError;
        }
        private string m_strName;
        private string m_strError;
    }
}