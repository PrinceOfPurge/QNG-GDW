using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    public class CustomizationUi : MonoBehaviour
    {
        [SerializeField] private CharecterCustomizer customizer;
        [SerializeField]
        private Color[] skinPalette =
        {
            new Color(1.01f, 0.89f, 0.74f),
            new Color(0.95f, 0.77f, 0.59f),
            new Color(0.79f, 0.58f, 0.39f),
        };
        private CustomizationInvoker _invoker;
        private void Awake()
        {
            if (customizer == null) customizer = FindAnyObjectByType<CharecterCustomizer>();
            _invoker = new CustomizationInvoker(maxHistory: 100, mergeWindow: 0.5f);
        }
    }
}

