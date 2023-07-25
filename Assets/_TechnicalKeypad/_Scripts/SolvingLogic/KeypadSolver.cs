using System;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;

public static class KeypadSolver
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

    private static KeypadInfo s_keypadInfo;
    private static KMBombInfo s_bombInfo;

    private static int[] s_sortedButtonOrder;

    private static Action<string> s_logger;

    public static KeypadAction[] GenerateSolution(KeypadInfo keypadInfo, KMBombInfo bombInfo, Action<string> logger) {
        s_keypadInfo = keypadInfo;
        s_bombInfo = bombInfo;
        s_logger = logger;

        int[] buttons = GetOrderedButtonList();
        KeyColour[] colours = s_keypadInfo.Colours;

        s_logger($"The buttons which apply are, in order, {buttons.Join(", ")}.");
        return buttons.Select(b => GetActionFromButton(b, colours[b])).ToArray();
    }

    private static int[] GetOrderedButtonList() {
        string colourOrder = GetColourOrder();
        s_logger($"The colour order is {colourOrder}.");

        int[] interPositions = s_bombInfo.IsPortPresent(Port.Parallel) ? s_keypadInfo.IntersectionPositions.Select(p => 2 - p % 3 + 3 * (p / 3)).ToArray() : s_keypadInfo.IntersectionPositions;
        int[] unsortedButtons = s_keypadInfo.RedIsLit ? Enumerable.Range(0, 9).Except(interPositions).ToArray() : interPositions;

        s_sortedButtonOrder = Enumerable
            .Range(0, 9)
            .OrderBy(b => colourOrder.IndexOf(
                s_keypadInfo.Colours[b] == KeyColour.White ? "W" :
                s_keypadInfo.Colours[b] == KeyColour.Black ? "K" :
                s_keypadInfo.Colours[b].ColourblindText
            )).ToArray();

        return s_sortedButtonOrder.Where(b => unsortedButtons.Contains(b)).ToArray();
    }

    private static string GetColourOrder() {
        string order;
        int digitToUse = Mathf.Max(s_bombInfo.GetSerialNumberNumbers().Last() - 1, 0);

        if (s_keypadInfo.GreenIsLit) // Row
            order = s_colourTable[digitToUse];
        else // Column
            order = s_colourTable.Select(seq => seq[digitToUse]).Join("");

        if (s_keypadInfo.YellowIsLit)
            order = order.Reverse().Join("");

        return order;
    }

    private static int GetPosition(int x, int y) => x + y * 3;

    private static KeypadAction GetActionFromButton(int button, KeyColour colour) {
        int row = button / 3;
        int col = button % 3;

        if (colour == KeyColour.White) {
            int holdTime = s_bombInfo.GetBatteryCount() + 1;
            s_logger($"Button {button} is {colour.Name.ToLower()}. {colour.RuleLogging.Replace("{0}", holdTime.ToString())}");
            return KeypadAction.CreateHoldAction(button, holdTime);
        }
        if (colour == KeyColour.Black) {
            int holdTime = s_bombInfo.GetIndicators().Count() + 1;
            s_logger($"Button {button} is {colour.Name.ToLower()}. {colour.RuleLogging.Replace("{0}", holdTime.ToString())}");
            return KeypadAction.CreateHoldAction(button, holdTime);
        }

        s_logger($"Button {button} is {colour.Name.ToLower()}. {colour.RuleLogging}");

        if (colour == KeyColour.Blue) // All except this one.
            return KeypadAction.CreatePressAction(Enumerable.Range(0, 8).Select(p => p >= button ? p + 1 : p));
        if (colour == KeyColour.Orange)
            return KeypadAction.CreatePressAction(s_sortedButtonOrder.TakeWhile(b => b != button).Concat(new int[] { button }));
        if (colour == KeyColour.Purple) // This one and all after it.
            return KeypadAction.CreatePressAction(s_sortedButtonOrder.Except(s_sortedButtonOrder.TakeWhile(b => b != button)));
        if (colour == KeyColour.Teal)
            return KeypadAction.CreatePressAction(button, GetPosition(col, (row + 1) % 3), GetPosition(col, (row + 2) % 3));
        if (colour == KeyColour.Yellow)
            return KeypadAction.CreatePressAction(button, GetPosition((col + 1) % 3, row), GetPosition((col + 2) % 3, row));

        var validButtons = new List<int> { button };
        if (colour == KeyColour.Red) {
            if (row + 1 < 3)
                validButtons.Add(GetPosition(col, row + 1));
            if (row - 1 >= 0)
                validButtons.Add(GetPosition(col, row - 1));
            if (col + 1 < 3)
                validButtons.Add(GetPosition(col + 1, row));
            if (col - 1 >= 0)
                validButtons.Add(GetPosition(col - 1, row));
            return KeypadAction.CreatePressAction(validButtons);
        }
        if (colour == KeyColour.Green) {
            if (col + 1 < 3) {
                if (row + 1 < 3)
                    validButtons.Add(GetPosition(col + 1, row + 1));
                if (row - 1 >= 0)
                    validButtons.Add(GetPosition(col + 1, row - 1));
            }
            if (col - 1 >= 0) {
                if (row + 1 < 3)
                    validButtons.Add(GetPosition(col - 1, row + 1));
                if (row - 1 >= 0)
                    validButtons.Add(GetPosition(col - 1, row - 1));
            }
            return KeypadAction.CreatePressAction(validButtons);
        }
        throw new ArgumentException($"Received a non-predefined colour {colour.Name} with colourblind text {colour.ColourblindText}.");
    }
}
