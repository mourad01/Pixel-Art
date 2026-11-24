using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoxDLL
{
    public class ColorSettings
    {
        List<Color> colors = new List<Color>();
        IDictionary<Color, int> dicColors = new Dictionary<Color, int>();

        public ColorSettings(Color[] pallete)
        {
            colors = pallete.ToList<Color>().Distinct<Color>().ToList<Color>();
            //this.colors.Remove(Color.white);
            //this.colors.Remove(new Color32(75, 75, 75, 255));
            colors.Remove(new Color32(0, 0, 0, 0));
            int index = 0;
            foreach (Color color in colors)
            {
                dicColors.Add(color, index);
                index++;
            }
        }

        internal Color GetColorByIndex(int value)
        {
            if (value == -1)
            {
                return Color.white;
            }
            var keyValuePair = dicColors.FirstOrDefault<KeyValuePair<Color, int>>((KeyValuePair<Color, int> x) => x.Value == value);
            return keyValuePair.Key;
        }

        internal IDictionary<Color, int> GetColorPallete()
        {
            return dicColors;
        }

        internal List<Color> GetColorPalleteList()
        {
            return colors;
        }

        internal int GetNumberByColor(Color color)
        {
            if (color == Color.white)
            {
                return -1;
            }
            var keyValuePair = dicColors.FirstOrDefault<KeyValuePair<Color, int>>((KeyValuePair<Color, int> x) => x.Key == color);
            return keyValuePair.Value;
        }
    }
}
