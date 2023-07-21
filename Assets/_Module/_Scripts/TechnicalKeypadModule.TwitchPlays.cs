using System.Collections;
using UnityEngine;
// using System.Text.RegularExpressions; // Useful for implementing TP.

#pragma warning disable 414, IDE0051, IDE1006

partial class TechnicalKeypadModule {

    private readonly string TwitchHelpMessage = @"Use '!{0} breh' to do things | '!{0} breh2' to do other things; extra information here.";
    // * TP Documentation: https://github.com/samfundev/KtaneTwitchPlays/wiki/External-Mod-Module-Support
    // ! Remove if not used. For more niche things like TwitchManualCode and ZenModeActive, look at tp docs ^^
    private bool TwitchPlaysActive;
    private bool TwitchShouldCancelCommand;

    private IEnumerator ProcessTwitchCommand(string command) {
        yield return "sendtochaterror TP has not yet been implemented.";

        // * Setup for implementing TP using regular expressions for command validation.
        // ! Requires importing the System.Text.RegularExpressions namespace.
        //  command = command.Trim().ToUpperInvariant();

        // Match match = Regex.Match(command, @"EXPRESSION", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        // if (match.Success) {
        //     yield return null;
        //     //! Do stuff.
        //     yield break;
        // }

        // yield return "sendtochaterror Invalid command!";
    }

    private IEnumerator TwitchHandleForcedSolve() {
        Debug.Log("TP autosolver has not yet been implemented. Calling KMBombModule.HandlePass.");
        _module.HandlePass();
        yield return null;
    }
}