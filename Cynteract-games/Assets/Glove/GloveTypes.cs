using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.CGlove
{
    public enum Player
    {
        Player0,
        Player1,
        Player2
    }
    public enum Side
    {
        NotSet,Left, Right
    }
    public delegate void GloveCallback(Glove data);
    public delegate void GloveConnectedCallback(GloveInformation information);
    public delegate void GloveDisconnectedCallback(GloveInformation information);
    public delegate void RawGloveDataCallback(DataReceive data);
  
    public enum Finger
    {
        Thumb=0,
        Index=1,
        Middle=2,
        Ring=3,
        Pinky=4
    }
    public enum Fingerpart
    {
        thumbBase=0, thumbCenter= 1, thumbTop= 2,
        indexBase=3, indexCenter = 4, indexTop = 5,
        middleBase=6, middleCenter = 7, middleTop = 8,
        ringBase=9, ringCenter = 10, ringTop = 11,
        pinkyBase=12, pinkyCenter = 13, pinkyTop = 14,
        palmCenter =15, palmTop=16, palmBase=17

    }
    public enum ImuState
    {
        Booting = 0,
        NotConnected = 1,
        Error = 2,
        RunBooting = 3,
        Running2 = 4,
        Running1 = 5,
        Running = 6,
    }
    public enum VibrationPattern
    {
        None,
        ShortWeakClick,
        ShortMediumClick,
        ShortStrongClick,

        MediumWeakClick,
        MediumMediumClick,
        MediumStrongClick,

        LongWeakClick,
        LongMediumClick,
        LongStrongClick,

        LongBump,
        MediumBump,
        ShortBump,

        LongRampUp,
        MediumRampUp,
        ShortRampUp,

        LongRampDown,
        MediumRampDown,
        ShortRampDown
    }
    public struct GloveInputData
    {
        public float angle;
        public ImuState state;

        public GloveInputData(float angle, ImuState state)
        {
            this.angle = angle;
            this.state = state;
        }
    }
}
