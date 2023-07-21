using UnityEngine;

public class KeyColour {

#pragma warning disable IDE1006
    public static readonly KeyColour Red = new KeyColour("Red", 109, 0, 0);
    public static readonly KeyColour Orange = new KeyColour("Orange", 118, 70, 10);
    public static readonly KeyColour Yellow = new KeyColour("Yellow", 120, 104, 35);
    public static readonly KeyColour Green = new KeyColour("Green", 16, 90, 26);
    public static readonly KeyColour Blue = new KeyColour("Blue", 23, 93, 135);
    public static readonly KeyColour Teal = new KeyColour("Teal", 24, 68, 77);
    public static readonly KeyColour Purple = new KeyColour("Purple", 63, 29, 105);
    public static readonly KeyColour White = new KeyColour("White", 122, 122, 122, colourblindText: "");
    public static readonly KeyColour Black = new KeyColour("Black", 28, 28, 28, colourblindText: "");
#pragma warning restore IDE1006

    public string Name { get; private set; }
    public string ColourblindText { get; private set; }
    public Color Colour { get; private set; }

    private KeyColour(string name, int r, int g, int b) : this(name, r, g, b, name[0].ToString()) { }

    private KeyColour(string name, int r, int g, int b, string colourblindText) {
        Name = name;
        ColourblindText = colourblindText;
        Colour = new Color(r / 255f, g / 255f, b / 255f);
    }

}