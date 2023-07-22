using System.Linq;

public class KeypadInfo
{
    private KeyColour[] _colours;
    private int[] _intersectionPositons;
    private bool _greenLed;
    private bool _yellowLed;
    private bool _redLed;

    public string Digits { get; private set; }
    public KeyColour[] Colours => _colours.ToArray();
    public int[] IntersectionPositions => _intersectionPositons.ToArray();
    public bool[] LedStates => new bool[] { _greenLed, _yellowLed, _redLed };

    public KeypadInfo(string digits, KeyColour[] colours, int[] intersectionPositions, bool red, bool yellow, bool green) {
        Digits = digits;
        _colours = colours;
        _intersectionPositons = intersectionPositions;
        _greenLed = green;
        _yellowLed = yellow;
        _redLed = red;
    }
}