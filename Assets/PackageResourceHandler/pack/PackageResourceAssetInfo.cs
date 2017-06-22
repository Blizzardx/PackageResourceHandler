
using System;
using System.Collections.Generic;

public class PackageResourceAssetInfo
{
    private string m_strSourcePath;
    private string m_strOutputPath;

    public PackageResourceAssetInfo(string fullPath,string sourcePath,string outputPath)
    {

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
    internal string GetOutputPath()
    {
        return m_strOutputPath;
    }
    internal string GetSourcePath()
    {
        return m_strSourcePath;
    }
}