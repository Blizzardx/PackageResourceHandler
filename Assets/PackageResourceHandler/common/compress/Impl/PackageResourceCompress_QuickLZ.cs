namespace PackageResourceHandler
{
    public class PackageResourceCompress_QuickLZ : IPackageResourceCompressor
    {
        public byte[] Compress(byte[] source)
        {
            return QuickLZ.compress(source, 1);
        }

        public byte[] Decompress(byte[] source)
        {
            return QuickLZ.decompress(source);
        }
    }
}