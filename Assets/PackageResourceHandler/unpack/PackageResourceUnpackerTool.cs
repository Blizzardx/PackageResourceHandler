﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageResourceUnpackerTool : MonoBehaviour
{
    private Queue<PackageResourceLoaderElement> m_TaskQueue;
    private Queue<PackageResourceLoaderElement> m_TaskErrorQueue;
    private Queue<PackageResourceLoaderElement> m_TaskSucceedQueue;
    private const int MAX_LOADER_COUNT = 10;
    private const int MAX_RETRY_LOAD_COUNT = 3;
    private int m_iCurrentLoaderCount = 0;
    private int m_iCurrentRetryCount = 0;
    private string m_strRootPathPerfix;
    private int m_iTotalLoadCount;
    private float m_fCurrentProcess;
    private Action m_AllSucceedCallback;
    private Action<List<PackageResourceLoadErrorInfo>> m_ErrorCallback;
    private Action<float, string> m_ProcessCallback;
    private bool m_bIsRunning;
    private bool m_bIsCrash;
    private IPackageResourceCompressor m_Decompressor;

    public void LoadAssetToFileSystem(string outputPath,List<string> targetFiles, IPackageResourceCompressor decompressor)
    {
        if(string.IsNullOrEmpty(outputPath) || null == targetFiles || targetFiles.Count == 0)
        {
            return;
        }
        if(m_bIsRunning)
        {
            return;
        }
        m_bIsRunning = true;

        m_Decompressor = decompressor;
        m_strRootPathPerfix = GetStreamingAssetPath();
        m_TaskQueue = new Queue<PackageResourceLoaderElement>(targetFiles.Count);

        for(int i=0;i<targetFiles.Count;++i)
        {
            string url = m_strRootPathPerfix + targetFiles[i];
            string elemOutputPath = outputPath + targetFiles[i];

            m_TaskQueue.Enqueue(new PackageResourceLoaderElement(targetFiles[i],url,elemOutputPath));
        }
        m_iTotalLoadCount = m_TaskQueue.Count;

        m_TaskErrorQueue = new Queue<PackageResourceLoaderElement>();
        m_TaskSucceedQueue = new Queue<PackageResourceLoaderElement>();

        m_iCurrentLoaderCount = 0;
        m_iCurrentRetryCount = 0;

        // start task
        StartTask();
    }
    public void SetProcessCallback(Action<float, string> callback)
    {
        m_ProcessCallback = callback;
    }
    public void SetErrorCallback(Action<List<PackageResourceLoadErrorInfo>> callback)
    {
        m_ErrorCallback = callback;
    }
    public void SetAllSucceedCallback(Action callback)
    {
        m_AllSucceedCallback = callback;
    }

    private void StartTask()
    {
        for(; m_iCurrentLoaderCount < MAX_LOADER_COUNT;++m_iCurrentLoaderCount)
        {
            var taskElem = GetTask();
            if(null == taskElem)
            {
                break;
            }
            StartCoroutine(BeginLoad(taskElem));
        }
    }
    private IEnumerator BeginLoad(PackageResourceLoaderElement taskElem)
    {
        WWW loader = new WWW(taskElem.GetUrl());

        yield return loader;

        // update loader count
        --m_iCurrentLoaderCount;

        if (m_bIsCrash)
        {
            loader.Dispose();

            if(m_iCurrentLoaderCount == 0)
            {
                m_bIsRunning = false;
            }
        }
        else
        {
            OnOneTaskDone(taskElem, loader);
        }
    }
    private void OnOneTaskDone(PackageResourceLoaderElement taskElem,WWW request)
    {        
        if (request.error != null)
        {
            taskElem.SetErrorInfo(request.error);
            // release www
            request.Dispose();

            // add to error list
            m_TaskErrorQueue.Enqueue(taskElem);
            return;
        }
        // check www content
        if(null == request.bytes)
        {
            taskElem.SetErrorInfo("load error");

            // release www
            request.Dispose();

            // add to error list
            m_TaskErrorQueue.Enqueue(taskElem);
            return;
        }
        try
        {
            byte[] content = request.bytes;
            if(null != m_Decompressor)
            {
                // decompress
                content = m_Decompressor.Decompress(content);
            }
            // ensure folder
            PackageResourceTool.EnsureFolder(taskElem.GetOutputPath());

            // try write byte files
            System.IO.File.WriteAllBytes(taskElem.GetOutputPath(), content);

            // add to succeed list
            m_TaskSucceedQueue.Enqueue(taskElem);
        }
        catch (Exception e)
        {
            // mark error msg
            taskElem.SetErrorInfo(e.Message);

            // release www
            request.Dispose();

            // add to error list
            m_TaskErrorQueue.Enqueue(taskElem);

            // check is need crash
            m_bIsCrash = true;
            CallbackWithError();
        }

        // check next
        CheckNext(taskElem);
    }
    private void CheckNext(PackageResourceLoaderElement currentDoneElement)
    {
        if(currentDoneElement.GetError() == null)
        {
            // update process
            m_fCurrentProcess = m_TaskSucceedQueue.Count / m_iTotalLoadCount;
            if (null != m_ProcessCallback)
            {
                m_ProcessCallback(m_fCurrentProcess, currentDoneElement.GetName());
            }
        }

        // check is all done
        if(m_iTotalLoadCount == (m_TaskSucceedQueue.Count + m_TaskErrorQueue.Count))
        {
            // all done check
            CheckComplate();
        }
        else
        {
            // start task
            StartTask();
        }
    }
    private void CheckComplate()
    {
        if(m_TaskErrorQueue.Count == 0)
        {
            m_bIsRunning = false;
            // all done
            if (null != m_AllSucceedCallback)
            {
                m_AllSucceedCallback();
            }
        }
        else
        {
            // retry
            if(m_iCurrentRetryCount >= MAX_RETRY_LOAD_COUNT)
            {
                // mark as done 
                m_bIsRunning = false;

                CallbackWithError();
            }
            else
            {
                ++m_iCurrentRetryCount;
                // retry
                while(m_TaskErrorQueue.Count > 0)
                {
                    m_TaskQueue.Enqueue(m_TaskErrorQueue.Dequeue());
                }
                // start task
                StartTask();
            }
        }
    }
    private void CallbackWithError()
    {
        // all done with error
        if (null != m_ErrorCallback)
        {
            List<PackageResourceLoadErrorInfo> errorList = new List<PackageResourceLoadErrorInfo>(m_TaskErrorQueue.Count);
            while (m_TaskErrorQueue.Count > 0)
            {
                var tmpElem = m_TaskErrorQueue.Dequeue();
                errorList.Add(new PackageResourceLoadErrorInfo(tmpElem.GetName(), tmpElem.GetError()));
            }
            m_ErrorCallback(errorList);
        }
    }
    private PackageResourceLoaderElement GetTask()
    {
        if(null == m_TaskQueue || m_TaskQueue.Count == 0)
        {
            return null;
        }
        return m_TaskQueue.Dequeue();
    }
    private string GetStreamingAssetPath()
    {
        switch(Application.platform)
        {
            case RuntimePlatform.Android:
                return "jar:file://" + Application.dataPath + "!/assets/";
            case RuntimePlatform.IPhonePlayer:
                return Application.dataPath + "/Raw/";
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return Application.dataPath + "/StreamingAssets/";

        }
        return  Application.dataPath + "/StreamingAssets/";
    }
}