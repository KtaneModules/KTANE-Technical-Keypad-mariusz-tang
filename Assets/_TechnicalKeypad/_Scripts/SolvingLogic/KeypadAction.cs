using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeypadAction
{
    private int[] _validButtons;

    public int[] ValidButtons => _validButtons.ToArray();
    public bool IsHoldAction { get; private set; }
    public int HoldTime { get; private set; }

    private KeypadAction(params int[] validButtons) {
        _validButtons = validButtons.ToArray();
    }

    private KeypadAction(IEnumerable<int> validButtons) {
        _validButtons = validButtons.ToArray();
    }

    public static KeypadAction CreatePressAction(params int[] validButtons) => new KeypadAction(validButtons);
    public static KeypadAction CreatePressAction(IEnumerable<int> validButtons) => new KeypadAction(validButtons);

    public static KeypadAction CreateHoldAction(int validButton, int holdTime) => new KeypadAction(validButton) { IsHoldAction = true, HoldTime = holdTime };
}
