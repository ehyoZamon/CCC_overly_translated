using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sequence
{
    public class WaitForGloveSequence :SequenceElement

    {
        public WaitForConnectionPanel waitForGlovePanel, waitForGlovePanelInstance;
        public enum Mode
        {
            Left, Right, Both, Any
        }
        public Mode mode;
        private Action onElementFinish;


        IEnumerator WaitForGlove()
        {
            string dot = ".";
            int numberOfDots = 0;
            while (true)
            {
                bool right = CGlove.Glove.Right.information.RecievedInformation;
                bool left = CGlove.Glove.Left.information.RecievedInformation;
                string text = "Warte auf Handschuhverbindung\n";
                for (int i = 0; i < numberOfDots; i++)
                {
                    text += dot;
                }
                numberOfDots = (numberOfDots + 1) % 10+1;
                waitForGlovePanelInstance.textMesh.text = text;
                switch (mode)
                {
                    case Mode.Left:
                        if (left)
                        {
                            goto Finish;
                        }
                        break;
                    case Mode.Right:
                        if (right)
                        {
                            goto Finish;
                        }
                        break;
                    case Mode.Both:
                        if (right&&left)
                        {
                            goto Finish;
                        }
                        break;
                    case Mode.Any:
                        if (right || left)
                        {
                            goto Finish;
                        }
                        break;
                    default:
                        break;
                }
                yield return new WaitForSecondsRealtime(.5f);
            }
        Finish:
            GameObject.Destroy(waitForGlovePanelInstance.gameObject);
            onElementFinish();
        }
        public IEnumerator StartSequence(Action onElementFinish)
        {
            this.onElementFinish = onElementFinish;
            waitForGlovePanelInstance = GameObject.Instantiate(waitForGlovePanel, MainCanvas.instance.transform);
            return WaitForGlove();
        }

    }
}