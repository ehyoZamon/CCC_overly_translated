using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cynteract.UI
{
    public class InstantiatedMenu : MonoBehaviour
    {
        public Button backButton;
        public void Close()
        {
            backButton.onClick.Invoke();
        }
    }
}
