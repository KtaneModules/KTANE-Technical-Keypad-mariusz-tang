using System.Linq;

public class KeypadInfo
{
    private KeyColour[] _colours;
    private int[] _intersectionPositons;

    public string Digits { get; private set; }
    public KeyColour[] Colours => _colours.ToArray();
    public int[] IntersectionPositions => _intersectionPositons.ToArray();
    public bool GreenIsLit { get; set; }
    public bool YellowIsLit { get; set; }
    public bool RedIsLit { get; set; }
    public bool[] LedStates => new bool[] { GreenIsLit, YellowIsLit, RedIsLit };

    public KeypadInfo(string digits, KeyColour[] colours, int[] intersectionPositions, bool red, bool yellow, bool green) {
        Digits = digits;
        _colours = colours;
        _intersectionPositons = intersectionPositions;
        GreenIsLit = green;
        YellowIsLit = yellow;
        RedIsLit = red;
    }
}