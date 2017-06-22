using Common.Component;
using Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PackageResourcePacker
{
    private string[] m_IgnoreSuffixList = new string[]
    {
        ".svn",
        ".git",
        ".meta",
    };
    private const string m_strPackageResourceConfigName = "PackageResourceConfig";
    private PackageResourceConfig m_ReportConfig;

    public void PackPackageResource(PackageResourceCompressType compressType, string sourceDirectory, string targetDirectory)
    {
        try
        {
            BeginPackPackageResource(compressType,sourceDirectory,targetDirectory);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private void BeginPackPackageResource(PackageResourceCompressType compressType,string sourceDirectory,string targetDirectory)
    {
        m_ReportConfig = new PackageResourceConfig();

        m_ReportConfig.compressType = (int)compressType;
        m_ReportConfig.fileList = new List<string>();

        // get compressor 
        var compressor = PackageResourceCompressHelper.GetCompress(compressType);
        if(null == compressor)
        {
            Debug.LogError("Unsupport compress type " + compressType);
            return;
        }
        // sign list
        StringBuilder allFileCRC32Code = new StringBuilder();

        // create file list
        var fileList = GetFileList(sourceDirectory,targetDirectory);

        for(int i=0;i<fileList.Count;++i)
        {
            var elem = fileList[i];

            // load file to memory
            byte[] content = File.ReadAllBytes(elem.GetSourcePath());

            // compess file with compress type
            content = CompressFile(content, compressor);

            // get crc32
            allFileCRC32Code.Append(CRC32.GetCRC32byte(content));

            // get output path
            string outputPath = elem.GetOutputPath();

            // ensure folder
            PackageResourceTool.EnsureFolder(outputPath);

            // write file to target directory
            File.WriteAllBytes(outputPath, content);

            // update  report
            m_ReportConfig.fileList.Add(elem.GetSubpath());
        }

        // update report version
        m_ReportConfig.version = CRC32.GetCRC32Str(allFileCRC32Code.ToString()).ToString();

        // save report file
        var reportFile = XmlConfigBase.Serialize(m_ReportConfig);
        File.WriteAllText(targetDirectory + m_strPackageResourceConfigName, reportFile);
    }
    private List<PackageResourceAssetInfo> GetFileList(string sourcePath,string outputPath)
    {
        var dir = new DirectoryInfo(sourcePath);
        var files = dir.GetFiles("*", SearchOption.AllDirectories);

        List<PackageResourceAssetInfo> fileList = new List<PackageResourceAssetInfo>();

        for(int i=0;i<files.Length;++i)
        {
            PackageResourceAssetInfo elem = new PackageResourceAssetInfo(files[i].FullName, sourcePath, outputPath);

            if(elem.IsInSuffixIngoreList(m_IgnoreSuffixList))
            {
                continue;
            }

            fileList.Add(elem);
        }

        return fileList;
    }
    private byte[] CompressFile(byte[] source, IPackageResourceCompressor compressType)
    {
        return compressType.Compress(source);
    }
}
