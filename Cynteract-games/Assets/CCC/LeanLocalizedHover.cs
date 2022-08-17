using Lean.Localization;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Cynteract.CCC
{
    [ExecuteInEditMode]

    [DisallowMultipleComponent]
    [RequireComponent(typeof(HoverTooltip))]
    [AddComponentMenu(LeanLocalization.ComponentPathPrefix + "Localized Hover")]

    public class LeanLocalizedHover : LeanLocalizedBehaviour
    {
        public string FallbackText;
        public override void UpdateTranslation(LeanTranslation translation)
        {
            // Get the TextMeshProUGUI component attached to this GameObject
            var text = GetComponent<HoverTooltip>();

            // Use translation?
            if (translation != null && translation.Data is string)
            {
                text.text = LeanTranslation.FormatText((string)translation.Data, text.text, this);
            }
            // Use fallback?
            else
            {
                text.text = LeanTranslation.FormatText(FallbackText, text.text, this);
            }
        }
        protected virtual void Awake()
        {
            // Should we set FallbackFont?
            if (FallbackText == null)
            {
                // Get the TextMesh component attached to this GameObject
                var text = GetComponent<HoverTooltip>();

                // Copy current text to fallback
                FallbackText = text.text;
            }
        }
    }

}