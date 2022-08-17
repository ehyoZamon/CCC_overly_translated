using System;
using System.Collections.Generic;


public enum CallbackType
{
    Once,
    Forever

}
public class Callback<T>
{
    Action<T> action;
    public CallbackType callbackType;
    public void Invoke(T argument)
    {
        action(argument);
    }
    public Callback(Action<T> action, CallbackType callbackType)
    {
        this.action = action;
        this.callbackType = callbackType;
    }
}
public class Callback
{
    Action action;
    public CallbackType callbackType;
    public void Invoke()
    {
        try
        {

            action();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }
    public Callback(Action action, CallbackType callbackType)
    {
        this.action = action;
        this.callbackType = callbackType;
    }


}

