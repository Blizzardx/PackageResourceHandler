namespace PackageResourceHandler
{
    public enum PackageResourceCompressType
    {
        None = 0,
        QuickLZ = 1,
        GZip = 2,
    }
    public class PackageResourceCompressHelper
    {
        static public IPackageResourceCompressor GetCompress(PackageResourceCompressType type)
        {
            switch (type)
            {
                case PackageResourceCompressType.None:
                    return new PackageResourceCompress_None();
                case PackageResourceCompressType.GZip:
                    return new PackageResourceCompress_GZip();
                case PackageResourceCompressType.QuickLZ:
                    return new PackageResourceCompress_QuickLZ();
            }
            return null;
        }

    }
}