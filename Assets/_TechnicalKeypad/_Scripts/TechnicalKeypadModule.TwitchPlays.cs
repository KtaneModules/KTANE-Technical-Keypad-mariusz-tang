using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

partial class TechnicalKeypadModule
{
#pragma warning disable IDE1006
    private readonly string TwitchHelpMessage = @"Use '!{0} 1 2 3 456789' to press all of the buttons in reading order | "
                                                + "'!{0} R O Y GBTPKW' to do press the Red, Orange, Yellow, Green, Blue, Teal, Purple, blacK, and White buttons; "
                                                + "do not mix colours and positions | "
                                                + "'!{0} hold R 1' to do hold the Red button for one beep; "
                                                + "you can do the same with positions | "
                                                + "'{0} mash' to mash bottom button | "
                                                + "'!{0} <cb/colourblind>' to toggle colourblind mode.";
    private bool TwitchShouldCancelCommand;
    private bool TwitchPlaysActive;
#pragma warning restore IDE1006

    private readonly WaitForSeconds _tpPressInterval = new WaitForSeconds(.1f);

    private string _buttonColours;
    // * TP Documentation: https://github.com/samfundev/KtaneTwitchPlays/wiki/External-Mod-Module-Support

    private IEnumerator ProcessTwitchCommand(string command) {
        command = command.Trim().ToUpperInvariant();

        RegexOptions options = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
        Match match = Regex.Match(command, @"^[1-9\s]+$", options);
        if (match.Success) {
            yield return null;
            int presses = 0;
            foreach (char digit in command.Replace(" ", "")) {
                yield return PressButton(digit - '1');
                presses++;
                yield return $"trycancel Command cancelled after {presses} press(es).";
            }
            yield break;
        }

        match = Regex.Match(command, @"^hold ([1-9]) (\d+)$", options);
        if (match.Success) {
            var holdTime = int.Parse(match.Groups[2].Value);
            if (holdTime == 0)
                yield return "sendtochaterror You cannot hold for 0 seconds!";
            if (holdTime > Mathf.Max(15, _currentAction.HoldTime))
                yield return "sendtochaterror You cannot hold a button for this long!";
            yield return null;
            yield return PressButton(int.Parse(match.Groups[1].Value) - 1, .5f * holdTime + .1f);
            yield break;
        }

        match = Regex.Match(command, @"^[roygbtpkw\s]+$", options);
        if (match.Success) {
            yield return null;
            int presses = 0;
            foreach (char colour in command.Replace(" ", "")) {
                yield return PressButton(_buttonColours.IndexOf(colour));
                presses++;
                yield return $"trycancel Command cancelled after {presses} press(es).";
            }
            yield break;
        }

        match = Regex.Match(command, @"^hold ([roygbtpkw]) (\d+)$", options);
        if (match.Success) {
            var holdTime = int.Parse(match.Groups[2].Value);
            if (holdTime == 0)
                yield return "sendtochaterror You cannot hold for 0 seconds!";
            if (holdTime > Mathf.Max(15, _currentAction.HoldTime))
                yield return "sendtochaterror you cannot hold a button for this long!";
            yield return null;
            yield return PressButton(_buttonColours.IndexOf(match.Groups[1].Value), .5f * holdTime + .1f);
            yield break;
        }

        match = Regex.Match(command, @"^colou?rblind|cb$", options);
        if (match.Success) {
            yield return null;
            _statusLightSelectable.OnInteract();
            yield break;
        }

        if (command == "MASH") {
            if (!_sirenStateActive)
                yield return "sendtochaterror that button is not currently accessible!";
            yield return null;
            while (_sirenStateActive) {
                _submitHatch.Selectable.OnInteract();
                yield return new WaitForSeconds(.05f);
                _submitHatch.Selectable.OnInteractEnded();
                yield return new WaitForSeconds(.05f);
            }
            yield break;
        }

        yield return "sendtochaterror Invalid command!";
    }

    private IEnumerator TwitchHandleForcedSolve() {
        while (!_isSolved) {
            if (_currentActionIndex >= _correctActions.Length)
                yield return ProcessTwitchCommand("MASH");
            else if (_currentAction.IsHoldAction)
                yield return ProcessTwitchCommand($"HOLD {_currentAction.ValidButtons[0] + 1} {_currentAction.HoldTime}");
            else
                yield return ProcessTwitchCommand(_currentAction.ValidButtons.Except(_currentPresses).Select(ix => ix + 1).Join());
        }
    }

    private IEnumerator PressButton(int button, float holdTime = .1f) {
        _buttons[button].Selectable.OnInteract();
        yield return new WaitForSeconds(holdTime);
        _buttons[button].Selectable.OnInteractEnded();
    }

    private void GetButtonColours() {
        _buttonColours = string.Empty;
        foreach (KeyColour colour in _keypadInfo.Colours) {
            if (colour.Name == "Black")
                _buttonColours += "K";
            else if (colour.Name == "White")
                _buttonColours += "W";
            else
                _buttonColours += colour.Name[0];
        }
    }
}
