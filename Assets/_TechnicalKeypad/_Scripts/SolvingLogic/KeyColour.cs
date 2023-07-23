using UnityEngine;

public class KeyColour
{
#pragma warning disable IDE1006
    public static readonly KeyColour Red = new KeyColour("Red", 109, 0, 0, "Press this button and all orthogonally adjacent buttons.");
    public static readonly KeyColour Orange = new KeyColour("Orange", 118, 70, 10, "Press this button and all buttons before this one in the colour order.");
    public static readonly KeyColour Yellow = new KeyColour("Yellow", 120, 104, 35, "Press all buttons in the same row as this one.");
    public static readonly KeyColour Green = new KeyColour("Green", 16, 90, 26, "Press this button and all diagonally adjacent buttons.");
    public static readonly KeyColour Blue = new KeyColour("Blue", 23, 93, 135, "Press every button except this one.");
    public static readonly KeyColour Teal = new KeyColour("Teal", 24, 68, 77, "Press every button in the same column as this one.");
    public static readonly KeyColour Purple = new KeyColour("Purple", 63, 29, 105, "Press this button and all buttons after this one in the colour order.");
    public static readonly KeyColour White = new KeyColour("White", 122, 122, 122, colourblindText: "",ruleLogging: "Hold this button for {0} beep(s).");
    public static readonly KeyColour Black = new KeyColour("Black", 28, 28, 28, colourblindText: "", ruleLogging: "Hold this button for {0} beep(s).");
#pragma warning restore IDE1006

    public string Name { get; private set; }
    public string ColourblindText { get; private set; }
    public Color Colour { get; private set; }
    public string RuleLogging { get; private set; }

    private KeyColour(string name, int r, int g, int b, string ruleLogging)
        : this(name, r, g, b, name[0].ToString(), ruleLogging) { }

    private KeyColour(string name, int r, int g, int b, string colourblindText, string ruleLogging) {
        Name = name;
        ColourblindText = colourblindText;
        Colour = new Color(r / 255f, g / 255f, b / 255f);
        RuleLogging = ruleLogging;
    }

    public override string ToString() => Name;
}