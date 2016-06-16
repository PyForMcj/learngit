#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  TaskAsyncHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  王云鹏 
     * 创建时间：2014/10/8 15:02:59 
     * 描述    : 异步执行任务帮助类
     * =====================================================================
     * 修改时间：2014/10/8 15:02:59 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TLZ.Helper
{
    public static class TaskAsyncHelper
    {
        private static readonly Task _emptyTask = TaskAsyncHelper.MakeEmpty();
        public static Task Empty
        {
            get
            {
                return TaskAsyncHelper._emptyTask;
            }
        }
        private static Task MakeEmpty()
        {
            return TaskAsyncHelper.FromResult<object>(null);
        }
        public static Task Catch(this Task task)
        {
            return task.ContinueWith<Task>(delegate(Task t)
            {
                if (t != null && t.IsFaulted)
                {
                    AggregateException exception = t.Exception;
                    Trace.TraceError("Catch exception thrown by Task: {0}", new object[]
					{
						exception
					});
                }
                return t;
            }).Unwrap();
        }
        public static Task<T> Catch<T>(this Task<T> task)
        {
            return task.ContinueWith<Task<T>>(delegate(Task<T> t)
            {
                if (t != null && t.IsFaulted)
                {
                    AggregateException exception = t.Exception;
                    Trace.TraceError("Catch<T> exception thrown by Task: {0}", new object[]
					{
						exception
					});
                }
                return t;
            }).Unwrap<T>();
        }
        public static Task Success(this Task task, Action<Task> successor)
        {
            return task.ContinueWith<Task>(delegate(Task _)
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    return task;
                }
                return Task.Factory.StartNew(delegate
                {
                    successor(task);
                });
            }).Unwrap();
        }
        public static Task Success<TResult>(this Task<TResult> task, Action<Task<TResult>> successor)
        {
            return task.ContinueWith<Task>(delegate(Task<TResult> _)
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    return task;
                }
                return Task.Factory.StartNew(delegate
                {
                    successor(task);
                });
            }).Unwrap();
        }
        public static Task<TResult> Success<TResult>(this Task task, Func<Task, TResult> successor)
        {
            return task.ContinueWith<Task<TResult>>(delegate(Task _)
            {
                if (task.IsFaulted)
                {
                    return TaskAsyncHelper.FromError<TResult>(task.Exception);
                }
                if (task.IsCanceled)
                {
                    return TaskAsyncHelper.Cancelled<TResult>();
                }
                return Task.Factory.StartNew<TResult>(() => successor(task));
            }).Unwrap<TResult>();
        }
        public static Task<TResult> Success<T, TResult>(this Task<T> task, Func<Task<T>, TResult> successor)
        {
            return task.ContinueWith<Task<TResult>>(delegate(Task<T> _)
            {
                if (task.IsFaulted)
                {
                    return TaskAsyncHelper.FromError<TResult>(task.Exception);
                }
                if (task.IsCanceled)
                {
                    return TaskAsyncHelper.Cancelled<TResult>();
                }
                return Task.Factory.StartNew<TResult>(() => successor(task));
            }).Unwrap<TResult>();
        }
        public static Task AllSucceeded(this Task[] tasks, Action continuation)
        {
            return tasks.AllSucceeded(delegate(Task[] _)
            {
                continuation();
            });
        }
        public static Task AllSucceeded(this Task[] tasks, Action<Task[]> continuation)
        {
            return Task.Factory.ContinueWhenAll<Task>(tasks, delegate(Task[] _)
            {
                Task task2 = tasks.FirstOrDefault((Task task) => task.IsCanceled);
                if (task2 != null)
                {
                    throw new TaskCanceledException();
                }
                List<Exception> list = (
                    from task in tasks
                    where task.IsFaulted
                    select task).SelectMany((Task task) => task.Exception.InnerExceptions).ToList<Exception>();
                if (list.Count > 0)
                {
                    throw new AggregateException(list);
                }
                return Task.Factory.StartNew(delegate
                {
                    continuation(tasks);
                });
            }).Unwrap();
        }
        public static Task<T> AllSucceeded<T>(this Task[] tasks, Func<T> continuation)
        {
            return Task.Factory.ContinueWhenAll<Task<T>>(tasks, delegate(Task[] _)
            {
                Task task2 = tasks.FirstOrDefault((Task task) => task.IsCanceled);
                if (task2 != null)
                {
                    throw new TaskCanceledException();
                }
                List<Exception> list = (
                    from task in tasks
                    where task.IsFaulted
                    select task).SelectMany((Task task) => task.Exception.InnerExceptions).ToList<Exception>();
                if (list.Count > 0)
                {
                    throw new AggregateException(list);
                }
                return Task.Factory.StartNew<T>(continuation);
            }).Unwrap<T>();
        }
        /// <summary>
        /// 启动一个多线程任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task<T> FromResult<T>(T value)
        {
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetResult(value);
            return taskCompletionSource.Task;
        }
        private static Task<T> FromError<T>(Exception e)
        {
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetException(e);
            return taskCompletionSource.Task;
        }
        private static Task<T> Cancelled<T>()
        {
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetCanceled();
            return taskCompletionSource.Task;
        }
    }

    /// <summary>
    /// 实现java中的CountDownLatch.java
    /// http://www.educity.cn/develop/127256.html
    /// </summary>
    public class CountDownLatch
    {
        private int _count;

        public CountDownLatch(int count)
        {
            this._count = count;
        }

        public void Await()
        {
            lock (this)
            {
                while (this._count > 0)
                {
                    Monitor.Wait(this);
                }
            }
        }

        public void CountDown()
        {
            lock (this)
            {
                Interlocked.Decrement(ref _count);
                Monitor.PulseAll(this);
            }
        }
    }
}
