using Common.Config;
using System.Collections.Generic;
namespace PackageResourceHandler
{
    public class PackageResourceConfig : XmlConfigBase
    {
        public string version;

        public int compressType;

        public List<string> fileList;

    }
}