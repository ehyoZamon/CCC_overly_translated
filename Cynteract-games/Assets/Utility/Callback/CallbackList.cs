using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class CallbackList
{
    List<Callback> callbacks = new List<Callback>();
    Mutex mutex = new Mutex();
    public void Add(Callback callback)
    {
        mutex.WaitOne();
        callbacks.Add(callback);
        mutex.ReleaseMutex();
    }
    public bool Remove(Callback callback)
    {
        bool success=false;
        mutex.WaitOne();
        success = callbacks.Remove(callback);
        mutex.ReleaseMutex();
        return success;
    }
    public void CallCallbacks()
    {
        mutex.WaitOne();
        for (int i = 0; i < callbacks.Count; i++)
        {
            callbacks[i].Invoke();
        }
        callbacks=callbacks.Where(x => x.callbackType != CallbackType.Once).ToList();
        mutex.ReleaseMutex();
    }
}
public class CallbackList<T>
{
    List<Callback<T>> callbacks = new List<Callback<T>>();
    Mutex mutex = new Mutex();
    public void Add(Callback <T>callback)
    {
        mutex.WaitOne();
        callbacks.Add(callback);
        mutex.ReleaseMutex();
    }
    public bool Remove(Callback<T> callback)
    {
        bool success = false;
        mutex.WaitOne();
        success = callbacks.Remove(callback);
        mutex.ReleaseMutex();
        return success;
    }
    public void CallCallbacks(T argument)
    {
        mutex.WaitOne();
        for (int i = 0; i < callbacks.Count; i++)
        {
            callbacks[i].Invoke(argument);
        }
        callbacks = callbacks.Where(x => x.callbackType != CallbackType.Once).ToList();

        mutex.ReleaseMutex();
    }
}