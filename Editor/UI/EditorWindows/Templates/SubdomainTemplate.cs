using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class SubdomainTemplate : VisualElement
    {
        private const string XML_PATH = "SubdomainTemplate";

        public new class UxmlFactory : UxmlFactory<SubdomainTemplate, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }

        public SubdomainTemplate()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);

        }
    }
}
