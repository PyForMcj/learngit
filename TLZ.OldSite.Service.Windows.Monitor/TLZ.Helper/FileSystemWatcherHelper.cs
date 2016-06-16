#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  FileSystemWatcherHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  王云鹏 
     * 创建时间：2014/8/20 17:03:28 
     * 描述    : 监控文件帮助类型
     * =====================================================================
     * 修改时间：2014/8/20 17:03:28 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TLZ.Helper
{
    public class FileSystemWatcherHelper : IDisposable
    {
        private readonly int _timeout_millis = 2000;
        private readonly string _applicationTag = AppDomain.CurrentDomain.FriendlyName;

        System.Threading.Timer _timer = null;
        List<String> _filePathList = new List<string>();
        FileSystemEventHandler _fileSystemEventHandler = null;

        public FileSystemWatcherHelper(FileSystemEventHandler watchHandler)
        {
            this._timer = new System.Threading.Timer(new TimerCallback(this.OnTimer), null, Timeout.Infinite, Timeout.Infinite);
            this._fileSystemEventHandler = watchHandler;
        }


        public FileSystemWatcherHelper(FileSystemEventHandler watchHandler, int timerInterval)
            : this(watchHandler)
        {
            this._timeout_millis = timerInterval;
        }

        public void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            using (Mutex mutex = new Mutex(false, this._applicationTag))
            {
                mutex.WaitOne();
                if (!this._filePathList.Contains(e.FullPath))
                {
                    this._filePathList.Add(e.FullPath);
                }
                mutex.ReleaseMutex();
            }
            this._timer.Change(this._timeout_millis, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            List<String> filePathList = new List<string>();
            using (Mutex mutex = new Mutex(false, this._applicationTag))
            {
                mutex.WaitOne();
                filePathList.AddRange(this._filePathList);
                this._filePathList.Clear();
                mutex.ReleaseMutex();
            }
            foreach (string filePath in filePathList)
            {
                this._fileSystemEventHandler(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, IOHelper.GetDirectoryName(filePath), IOHelper.GetFileNameWithExtension(filePath)));
            }
            filePathList.Clear();
            filePathList = null;
        }

        public void Dispose()
        {
            this._timer.Dispose();
            this._filePathList.Clear();
        }
    }
}
