using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackageResourcePacker
{
    private string[] m_IgnoreSuffixList = new string[]
    {
        ".svn",
        ".git",
        ".meta",
    };
	public void PackPackageResource(PackageResourceCompressType compressType,string sourceDirectory,string targetDirectory)
    {
        // get compressor 
        var compressor = PackageResourceCompressHelper.GetCompress(compressType);
        if(null == compressor)
        {
            Debug.LogError("Unsupport compress type " + compressType);
            return;
        }

        // create file list
        var fileList = GetFileList(sourceDirectory);

        for(int i=0;i<fileList.Count;++i)
        {
            var elem = fileList[i];

            // load file to memory
            byte[] content = File.ReadAllBytes(elem.GetSourcePath());

            // compess file with compress type
            content = CompressFile(content, compressor);

            // get output path
            string outputPath = elem.GetOutputPath();

            // ensure folder
            PackageResourceTool.EnsureFolder(outputPath);

            // write file to target directory
            File.WriteAllBytes(outputPath, content);

            // update  report

        }
    }
    List<PackageResourceAssetInfo> GetFileList(string path)
    {
        return null;
    }
    byte[] CompressFile(byte[] source, IPackageResourceCompressor compressType)
    {
        return compressType.Compress(source);
    }

}
