public interface IPackageResourceCompressor
{
    byte[] Compress(byte[] source);

    byte[] Decompress(byte[] source);
}