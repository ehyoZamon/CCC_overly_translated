using Cynteract.CGlove;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandDemo : MonoBehaviour
{
    public GloveData.RotationSet.RotationType rotationType;
    public Side side;
    public HandDemoPart  wrist,
        thumbBase, thumbCenter, thumbTop,
        indexBase, indexCenter, indexTop,
        middleBase, middleCenter, middleTop,
        ringBase, ringCenter, ringTop,
        pinkyBase, pinkyCenter, pinkyTop;
    public Button resetButton;
    private void Awake()
    {
        resetButton.onClick.AddListener(Reset);
    }

    private void Reset()
    {
        Glove.RightData.ResetAll();
        Glove.LeftData.ResetAll();
    }

    private void Start()
    {
    }
    private void Update()
    {
        DrawLines();
        GloveData data=null;
        switch (side)
        {
            case Side.NotSet:
                data = Glove.AnyData;
                break;
            case Side.Left:
                data = Glove.LeftData;
                break;
            case Side.Right:
                data = Glove.RightData;
                break;
        }
        RotateCubes(data);
    }

    private void RotateCubes(GloveData anyData)
    {
        if (anyData != null)
        {
            wrist.transform.rotation = anyData.wristRotation.reset;

            thumbBase.transform.localRotation = anyData[Fingerpart.thumbBase].GetRotation(rotationType);
            thumbCenter.transform.localRotation = anyData[Fingerpart.thumbCenter].GetRotation(rotationType);
            thumbTop.transform.localRotation = anyData[Fingerpart.thumbTop].GetRotation(rotationType);

            indexBase.transform.localRotation = anyData[Fingerpart.indexBase].GetRotation(rotationType);
            indexCenter.transform.localRotation = anyData[Fingerpart.indexCenter].GetRotation(rotationType);
            indexTop.transform.localRotation = anyData[Fingerpart.indexTop].GetRotation(rotationType);

            middleBase.transform.localRotation = anyData[Fingerpart.middleBase].GetRotation(rotationType);
            middleCenter.transform.localRotation = anyData[Fingerpart.middleCenter].GetRotation(rotationType);
            middleTop.transform.localRotation = anyData[Fingerpart.middleTop].GetRotation(rotationType);

            ringBase.transform.localRotation = anyData[Fingerpart.ringBase].GetRotation(rotationType);
            ringCenter.transform.localRotation = anyData[Fingerpart.ringCenter].GetRotation(rotationType);
            ringTop.transform.localRotation = anyData[Fingerpart.ringTop].GetRotation(rotationType);

            pinkyBase.transform.localRotation = anyData[Fingerpart.pinkyBase].GetRotation(rotationType);
            pinkyCenter.transform.localRotation = anyData[Fingerpart.pinkyCenter].GetRotation(rotationType);
            pinkyTop.transform.localRotation = anyData[Fingerpart.pinkyTop].GetRotation(rotationType);

        }
    }
    private void DrawLines()
    {
        thumbBase.lineRenderer.SetPositions(new Vector3[] { thumbBase.transform.position, wrist.transform.position });
        thumbCenter.lineRenderer.SetPositions(new Vector3[] { thumbCenter.transform.position, thumbBase.transform.position });
        thumbTop.lineRenderer.SetPositions(new Vector3[] { thumbTop.transform.position, thumbCenter.transform.position });

        indexBase.lineRenderer.SetPositions(new Vector3[] { indexBase.transform.position, wrist.transform.position });
        indexCenter.lineRenderer.SetPositions(new Vector3[] { indexCenter.transform.position, indexBase.transform.position });
        indexTop.lineRenderer.SetPositions(new Vector3[] { indexTop.transform.position, indexCenter.transform.position });

        middleBase.lineRenderer.SetPositions(new Vector3[] { middleBase.transform.position, wrist.transform.position });
        middleCenter.lineRenderer.SetPositions(new Vector3[] { middleCenter.transform.position, middleBase.transform.position });
        middleTop.lineRenderer.SetPositions(new Vector3[] { middleTop.transform.position, middleCenter.transform.position });

        ringBase.lineRenderer.SetPositions(new Vector3[] { ringBase.transform.position, wrist.transform.position });
        ringCenter.lineRenderer.SetPositions(new Vector3[] { ringCenter.transform.position, ringBase.transform.position });
        ringTop.lineRenderer.SetPositions(new Vector3[] { ringTop.transform.position, ringCenter.transform.position });

        pinkyBase.lineRenderer.SetPositions(new Vector3[] { pinkyBase.transform.position, wrist.transform.position });
        pinkyCenter.lineRenderer.SetPositions(new Vector3[] { pinkyCenter.transform.position, pinkyBase.transform.position });
        pinkyTop.lineRenderer.SetPositions(new Vector3[] { pinkyTop.transform.position, pinkyCenter.transform.position });
    }
}
