using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;

public class KeypadSolver
{
    private static string[] s_colourTable = new string[] {
        "GYTBWOPRK",
        "ROYGTKBPW",
        "YPBTORWKG",
        "WROYKTGBP",
        "TKGPYBOWR",
        "OGPKRWTYB",
        "KBWOPGRTY",
        "BWKRGPYOT",
        "PTRWBYKGO",
    };

    private KeypadInfo _keypadInfo;
    private KMBombInfo _bombInfo;

    private string _colourOrder;
    private int[] _sortedButtonOrder;

    private Action<string> _logger;

    private KeypadAction[] _correctActions;

    public KeypadSolver(KeypadInfo keypadInfo, KMBombInfo bombInfo, Action<string> logger) {
        _keypadInfo = keypadInfo;
        _bombInfo = bombInfo;
        _logger = logger;
        GenerateSolution();
    }

    private void GenerateSolution() {
        int[] buttons = GetOrderedButtonList();
        KeyColour[] colours = _keypadInfo.Colours;

        _logger($"The colour order is {_colourOrder}.");
        _logger($"The buttons which apply are {buttons.Join(", ")} (positions in reading order).");

        _correctActions = buttons.Select(b => GetActionFromButton(b, colours[b])).ToArray();

        _logger("The correct actions are as follows:");
        foreach (KeypadAction action in _correctActions)
            _logger($"Hold: {action.IsHoldAction}. Valid buttons: {action.ValidButtons.Join(", ")}");
    }

    private int[] GetOrderedButtonList() {
        _colourOrder = GetColourOrder();
        int[] interPositions = _bombInfo.IsPortPresent(Port.Parallel) ? _keypadInfo.IntersectionPositions.Select(p => 2 - p % 3 + 3 * (p / 3)).ToArray() : _keypadInfo.IntersectionPositions;
        int[] unsortedButtons = _keypadInfo.RedIsLit ? interPositions : Enumerable.Range(0, 9).Except(interPositions).ToArray();

        _sortedButtonOrder = Enumerable
            .Range(0, 9)
            .OrderBy(b => _colourOrder.IndexOf(
                _keypadInfo.Colours[b] == KeyColour.White ? "W" :
                _keypadInfo.Colours[b] == KeyColour.Black ? "K" :
                _keypadInfo.Colours[b].ColourblindText
            )).ToArray();

        return _sortedButtonOrder.Where(b => unsortedButtons.Contains(b)).ToArray();
    }

    private string GetColourOrder() {
        string order;
        int digitToUse = _bombInfo.GetSerialNumberNumbers().Last() - 1;
        if (digitToUse < 0)
            digitToUse = 0;

        if (_keypadInfo.GreenIsLit)
            order = s_colourTable[digitToUse];
        else
            order = s_colourTable.Select(seq => seq[digitToUse]).Join("");

        if (_keypadInfo.YellowIsLit)
            order = order.Reverse().Join("");

        return order;
    }

    private KeypadAction GetActionFromButton(int button, KeyColour colour) {
        int row = button / 3;
        int col = button % 3;

        Func<int, int, int> getPos = (x, y) => x + y * 3;

        // TODO: Turn this into a dictionary.
        if (colour == KeyColour.White) {
            int holdTime = _bombInfo.GetBatteryCount() + 1;
            _logger($"Button {button} is {colour.Name.ToLower()}. {colour.RuleLogging.Replace("{0}", holdTime.ToString())}");
            return KeypadAction.CreateHoldAction(button, holdTime);
        }
        if (colour == KeyColour.Black) {
            int holdTime = _bombInfo.GetIndicators().Count() + 1;
            _logger($"Button {button} is {colour.Name.ToLower()}. {colour.RuleLogging.Replace("{0}", holdTime.ToString())}");
            return KeypadAction.CreateHoldAction(button, holdTime);
        }

        _logger($"Button {button} is {colour.Name.ToLower()}. {colour.RuleLogging}");

        if (colour == KeyColour.Blue) // All except this one.
            return KeypadAction.CreatePressAction(Enumerable.Range(0, 9).Select(p => p >= button ? p + 1 : p));
        if (colour == KeyColour.Orange)
            return KeypadAction.CreatePressAction(_sortedButtonOrder.TakeWhile(b => b != button).Concat(new int[] { button }));
        if (colour == KeyColour.Purple) // This one and all after it.
            return KeypadAction.CreatePressAction(_sortedButtonOrder.Except(_sortedButtonOrder.TakeWhile(b => b != button)));
        if (colour == KeyColour.Teal)
            return KeypadAction.CreatePressAction(button, getPos(col, (row + 1) % 3), getPos(col, (row + 2) % 3));
        if (colour == KeyColour.Yellow)
            return KeypadAction.CreatePressAction(button, getPos((col + 1) % 3, row), getPos((col + 2) % 3, row));

        var validButtons = new List<int> { button };
        if (colour == KeyColour.Red) {
            if (row + 1 < 3)
                validButtons.Add(getPos(col, row + 1));
            if (row - 1 >= 0)
                validButtons.Add(getPos(col, row - 1));
            if (col + 1 < 3)
                validButtons.Add(getPos(col + 1, row));
            if (col - 1 >= 0)
                validButtons.Add(getPos(col - 1, row));
            return KeypadAction.CreatePressAction(validButtons);
        }
        if (colour == KeyColour.Green) {
            if (col + 1 < 3) {
                if (row + 1 < 3)
                    validButtons.Add(getPos(col + 1, row + 1));
                if (row - 1 >= 0)
                    validButtons.Add(getPos(col + 1, row - 1));
            }
            if (col - 1 >= 0) {
                if (row + 1 < 3)
                    validButtons.Add(getPos(col - 1, row + 1));
                if (row - 1 >= 0)
                    validButtons.Add(getPos(col - 1, row - 1));
            }
            return KeypadAction.CreatePressAction(validButtons);
        }

        throw new ArgumentException($"Received a non-predefined colour {colour.Name} with colourblind text {colour.ColourblindText}.");
    }
}
