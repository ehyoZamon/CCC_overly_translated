using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CynteractInput
{
    public enum Type
    {
        Axis,
        Action
    }
    public interface ICalibration
    {
        void StopCalibration();
        void StartCalibration(int index, Type type);
    }
}