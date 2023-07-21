using System.Linq;

public class KeypadInfo
{
    private KeyColour[] _colours;
    private int[] _intersectionPositons;

    public string Digits { get; private set; }
    public KeyColour[] Colours => _colours.ToArray();
    public int[] IntersectionPositions => _intersectionPositons.ToArray();

    public KeypadInfo(string digits, KeyColour[] colours, int[] intersectionPositions) {
        Digits = digits;
        _colours = colours;
        _intersectionPositons = intersectionPositions;
    }
}