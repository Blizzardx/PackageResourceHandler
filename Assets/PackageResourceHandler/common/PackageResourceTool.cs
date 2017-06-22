using System.IO;

public class PackageResourceTool
{
    public static void EnsureFolder(string path)
    {
        string folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }
}
