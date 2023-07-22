using System.Collections;
using System.Collections.Generic;
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
    private KeypadAction[] _correctActions;

    public KeypadSolver(KeypadInfo keypadInfo) {
        _keypadInfo = keypadInfo;
    }


}
