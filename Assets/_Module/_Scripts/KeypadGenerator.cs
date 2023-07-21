using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rnd = UnityEngine.Random;

public static class KeypadGenerator {

    private static readonly int[][] s_digitArrangement = new int[12][] {
        new int[] { 1, 1, 0 },
        new int[] { 1, 2, 0 },
        new int[] { 0, 0, 1 },
        new int[] { 0, 1, 1 },
        new int[] { 0, 2, 1 },
        new int[] { 1, 2, 1 },
        new int[] { 1, 1, 1 },
        new int[] { 1, 0, 1 },
        new int[] { 0, 2, 0 },
        new int[] { 0, 1, 0 },
        new int[] { 0, 0, 0 },
        new int[] { 1, 0, 0 },
    };
    private static KeyColour[] s_allColours = new KeyColour[] {
        KeyColour.Red,
        KeyColour.Orange,
        KeyColour.Yellow,
        KeyColour.Green,
        KeyColour.Teal,
        KeyColour.Blue,
        KeyColour.Purple,
        KeyColour.Black,
        KeyColour.White
    };

    public static KeypadInfo GenerateKeypad() {
        int[] intersectionPositions;
        string digits = GetDigits(out intersectionPositions);
        return new KeypadInfo(digits, GetRandomKeyColours(), intersectionPositions);
    }

    private static KeyColour[] GetRandomKeyColours() => s_allColours.ToArray().Shuffle();

    private static string GetDigits(out int[] intersectionPositions) {
        var pairDigits = Enumerable.Range(0, 10).ToArray().Shuffle().Take(4).ToArray();
        var rowPairs = Enumerable.Range(0, 3).Select(_ => new List<int>()).ToArray();
        var colPairs = Enumerable.Range(0, 3).Select(_ => new List<int>()).ToArray();
        var positions = Enumerable.Range(0, 9).ToList().Shuffle();
        var interPoints = new List<int>();

        for (int ix = 0; ix < 9; ix++) {
            var pos = positions[ix];
            var rowPair = rowPairs[pos / 3];
            var colPair = colPairs[pos % 3];

            if (ix < pairDigits.Length) {
                var pairDigit = pairDigits[ix];

                if (rowPair.Count < 2 && colPair.Count < 2) {
                    rowPair.Add(pairDigit);
                    colPair.Add(pairDigit);
                }
                else if (rowPair.Count < 2)
                    rowPair.Add(colPair.PickRandom());
                else
                    colPair.Add(rowPair.PickRandom());
                interPoints.Add(pos);
            }
            else {
                var tryDigit = Rnd.Range(0, 10 - rowPair.Count);
                while (rowPair.Contains(tryDigit))
                    tryDigit = (tryDigit + 1) % 10;
                rowPair.Add(tryDigit);

                tryDigit = Rnd.Range(0, 10 - colPair.Count);
                while (colPair.Contains(tryDigit))
                    tryDigit = (tryDigit + 1) % 10;
                colPair.Add(tryDigit);

                if (rowPair.Any(d => colPair.Contains(d)))
                    interPoints.Add(pos);
            }
        }
        var pairLists = new List<int>[][] { rowPairs, colPairs };
        string digitText = string.Empty;

        foreach (int[] coord in s_digitArrangement)
            digitText += pairLists[coord[0]][coord[1]][coord[2]];

        intersectionPositions = interPoints.ToArray();
        return digitText;
    }
}