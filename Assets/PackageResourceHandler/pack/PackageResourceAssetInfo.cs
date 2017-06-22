
using System;
using System.Collections.Generic;

public class PackageResourceAssetInfo
{
    private string m_strSourcePath;
    private string m_strOutputPath;
    private string m_strSubPath;

    public PackageResourceAssetInfo(string sourcePath,string sourceDirectory,string targetDirectory)
    {
        sourcePath = FixPath(sourcePath);
        sourceDirectory = FixPath(sourceDirectory);
        targetDirectory = FixPath(targetDirectory);

        m_strSourcePath = sourcePath;
        m_strSubPath = sourcePath.Substring(sourceDirectory.Length);

        m_strOutputPath = targetDirectory + m_strSubPath;

    }
    public bool IsInSuffixIngoreList(string[] ignoreList)
    {
        for(int i=0;i<ignoreList.Length;++i)
        {
            if(m_strSourcePath.EndsWith(ignoreList[i]))
            {
                return true;
            }
        }
        return false;
    }
    public string GetOutputPath()
    {
        return m_strOutputPath;
    }
    public string GetSourcePath()
    {
        return m_strSourcePath;
    }

    public string GetSubpath()
    {
        return m_strSubPath;
    }
    private string FixPath(string path)
    {
        return path.Replace('\\', '/');
    }
}