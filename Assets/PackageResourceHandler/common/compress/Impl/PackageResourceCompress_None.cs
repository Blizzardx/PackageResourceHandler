public class PackageResourceCompress_None : IPackageResourceCompressor
{
    public byte[] Compress(byte[] source)
    {
        return source;
    }

    public byte[] Decompress(byte[] source)
    {
        return source;
    }
}
