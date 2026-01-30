using System.Collections.Generic;
using UnityEngine;

namespace DropStack.Visual
{
    [CreateAssetMenu(menuName = "DropStack/Palette")]
    public class Palette : ScriptableObject
    {
        public string paletteId = "default";
        public List<Color> colors = new List<Color>();
        public Color backgroundTop = new Color(0.1f, 0.1f, 0.15f);
        public Color backgroundBottom = new Color(0.05f, 0.05f, 0.08f);
    }
}
