using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class AngelHernandezScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;

    public KMSelectable[] ButtonSels;
    public GameObject[] ButtonObjs;
    public GameObject[] LedsOn;
    public GameObject[] LedsOff;

    public GameObject[] DotParents;
    public GameObject[] DotObjsA;
    public GameObject[] DotObjsB;
    public GameObject[] DotObjsC;
    public GameObject[] DotObjsD;
    public GameObject[] DotObjsE;
    public GameObject[] DotObjsF;
    private GameObject[][] DotObjs = new GameObject[6][];

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    private static readonly bool[][] _brailleConfigs = new string[26] { "100000", "110000", "100100", "100110", "100010", "110100", "110110", "110010", "010100", "010110", "101000", "111000", "101100", "101110", "101010", "111100", "111110", "111010", "011100", "011110", "101001", "111001", "010111", "101101", "101111", "101011" }.Select(i => i.Select(j => j == '1').ToArray()).ToArray();
    private static readonly int[] _buttonLookIxs = new int[26] { 3, 2, 1, 0, 5, 4, 4, 5, 3, 1, 0, 2, 4, 0, 5, 2, 1, 3, 0, 3, 2, 1, 5, 4, 1, 2 };
    private static readonly int[][] _letterLists = new int[26][] {
        new int[26] { 2,11,20,3,24,9,17,19,23,10,6,22,12,7,16,5,1,0,13,15,18,25,4,21,8,14 },
        new int[26] { 5,17,15,14,12,23,24,19,18,9,2,13,10,4,1,8,20,16,0,7,11,25,21,6,3,22 },
        new int[26] { 24,14,25,5,4,21,3,13,10,18,1,7,12,9,16,19,0,20,2,6,8,17,23,22,15,11 },
        new int[26] { 22,10,16,8,0,14,17,23,5,20,15,1,7,11,2,12,18,24,4,25,6,9,19,13,3,21 },
        new int[26] { 21,24,22,13,3,23,0,11,6,25,12,14,15,10,4,2,19,8,18,20,5,16,17,1,9,7 },
        new int[26] { 21,1,19,11,9,23,8,22,16,0,5,24,3,7,6,18,10,13,20,17,2,4,14,12,15,25 },
        new int[26] { 7,18,25,12,9,15,4,3,16,1,13,23,2,19,22,20,8,11,5,10,14,0,21,17,24,6 },
        new int[26] { 20,14,8,23,17,25,6,2,4,9,16,13,11,19,18,24,15,5,1,10,0,7,3,12,22,21 },
        new int[26] { 6,4,1,8,20,2,11,3,15,13,9,18,19,22,12,14,10,23,0,25,7,5,17,16,24,21 },
        new int[26] { 1,11,9,7,19,22,15,2,6,23,25,0,12,24,3,4,17,21,5,16,13,8,20,14,18,10 },
        new int[26] { 18,16,1,22,0,8,15,4,11,3,21,19,2,13,17,10,23,12,6,14,9,20,25,5,7,24 },
        new int[26] { 25,19,21,18,11,1,24,22,20,13,16,7,4,6,15,9,14,8,17,10,0,23,5,12,2,3 },
        new int[26] { 18,14,9,17,10,13,2,6,11,20,22,1,21,16,8,15,3,12,0,19,7,23,4,5,24,25 },
        new int[26] { 5,8,24,23,0,6,13,9,19,22,16,1,2,25,17,4,3,12,21,7,18,15,14,20,11,10 },
        new int[26] { 2,18,7,9,22,4,25,6,1,0,10,23,20,21,19,3,12,17,5,11,16,15,13,14,24,8 },
        new int[26] { 25,6,1,12,17,8,20,23,5,15,24,21,10,18,19,0,13,3,11,7,4,2,14,22,9,16 },
        new int[26] { 22,21,14,9,25,3,20,5,19,10,6,17,12,15,4,8,1,23,16,11,24,0,7,18,13,2 },
        new int[26] { 18,6,13,3,20,10,12,15,0,19,21,7,2,8,17,23,11,1,14,25,4,22,5,9,24,16 },
        new int[26] { 18,8,25,6,2,22,0,24,13,10,3,23,11,16,5,19,7,4,20,12,15,14,21,1,9,17 },
        new int[26] { 3,2,11,6,1,25,18,15,17,19,7,12,9,21,0,4,24,8,23,14,13,22,10,5,20,16 },
        new int[26] { 24,18,8,13,7,5,10,20,15,16,23,21,2,4,9,14,22,0,17,3,6,1,25,11,19,12 },
        new int[26] { 4,10,0,7,17,15,12,3,23,21,18,14,16,25,20,9,24,19,1,2,8,6,5,22,11,13 },
        new int[26] { 1,12,25,2,22,4,23,18,13,17,10,16,3,6,8,0,14,20,24,21,5,11,7,19,15,9 },
        new int[26] { 0,11,10,15,7,24,19,22,17,2,13,1,20,3,8,25,14,5,18,9,23,6,4,12,16,21 },
        new int[26] { 19,6,25,8,11,12,18,10,7,9,2,20,15,5,0,3,21,13,22,16,1,17,24,23,4,14 },
        new int[26] { 9,13,12,24,17,16,25,7,5,1,10,8,14,20,18,2,23,6,22,21,0,19,15,3,11,4 }
    };
    private static readonly string[] _positionNames = new string[6] { "top-left", "middle-left", "bottom-left", "top-right", "middle-right", "bottom-right" };

    private int _currentStage;
    private bool _canPress;
    private int _mainLetter;
    private bool[] _mainConfig = new bool[6];
    private int[] _buttonLetters = new int[6];
    private readonly bool[][] _buttonConfigs = new bool[6][];
    private int _expectedPress;

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        DotObjs = new[] { DotObjsA, DotObjsB, DotObjsC, DotObjsD, DotObjsE, DotObjsF };
        for (int i = 0; i < 6; i++)
        {
            ButtonSels[i].OnInteract += ButtonPress(i);
            ButtonSels[i].OnHighlight += ButtonHighlight(i);
            ButtonSels[i].OnHighlightEnded += ButtonHighlightEnd(i);
            ButtonObjs[i].transform.localPosition = new Vector3(0f, -0.01f, 0f);
            for (int j = 0; j < 6; j++)
                DotObjs[i][j].SetActive(false);
        }
        Module.OnActivate += Activate;
    }

    private KMSelectable.OnInteractHandler ButtonPress(int btn)
    {
        return delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            ButtonSels[btn].AddInteractionPunch(0.5f);
            if (!_canPress || _moduleSolved)
                return false;
            if (btn == _expectedPress)
            {
                Debug.LogFormat("[Ángel Hernández #{0}] Correctly pressed {1}, the {2} button.", _moduleId, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_buttonLetters[btn]], _positionNames[btn]);
                LedsOff[_currentStage].SetActive(false);
                LedsOn[_currentStage].SetActive(true);
                if (_currentStage == 2)
                {
                    Debug.LogFormat("[Ángel Hernández #{0}] Module solved!", _moduleId);
                    Module.HandlePass();
                    _moduleSolved = true;
                    return false;
                }
                _currentStage++;
            }
            else
            {
                Module.HandleStrike();
                Debug.LogFormat("[Ángel Hernández #{0}] Pressed {1}, the {2} button, when {3}, the {4} button was expected. Strike.", _moduleId,
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_buttonLetters[btn]], _positionNames[btn], "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_buttonLetters[_expectedPress]], _positionNames[_expectedPress]);
            }
            GenerateStage();
            StartCoroutine(AnimateButtons());
            return false;
        };
    }

    private Action ButtonHighlight(int btn)
    {
        return delegate ()
        {
            for (int i = 0; i < 6; i++)
                if (_buttonConfigs[btn][i])
                    DotObjs[btn][i].SetActive(true);
        };
    }

    private Action ButtonHighlightEnd(int btn)
    {
        return delegate ()
        {
            for (int i = 0; i < 6; i++)
                DotObjs[btn][i].SetActive(false);
        };
    }

    private void GenerateStage()
    {
        _mainLetter = Rnd.Range(0, 26);
        _mainConfig = _brailleConfigs[_mainLetter];
        _buttonLetters = Enumerable.Range(0, 26).ToArray().Shuffle().Take(6).ToArray();
        for (int i = 0; i < 6; i++)
            _buttonConfigs[i] = _brailleConfigs[_buttonLetters[i]];
        Debug.LogFormat("[Ángel Hernández #{0}] The elevated buttons show the letter {1}.", _moduleId, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_mainLetter]);
        var buttonToLook = _buttonLookIxs[_mainLetter];
        Debug.LogFormat("[Ángel Hernández #{0}] Look at {1}, the {2} button.", _moduleId, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_buttonLetters[buttonToLook]], _positionNames[buttonToLook]);
        for (int i = 0; i < _letterLists[buttonToLook].Length; i++)
        {
            if (_buttonLetters.Contains(_letterLists[_buttonLetters[buttonToLook]][i]))
            {
                _expectedPress = Array.IndexOf(_buttonLetters, _letterLists[_buttonLetters[buttonToLook]][i]);
                break;
            }
        }
        Debug.LogFormat("[Ángel Hernández #{0}] The button to press is {1}, the {2} button.", _moduleId, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_buttonLetters[_expectedPress]], _positionNames[_expectedPress]);
    }

    private void Activate()
    {
        GenerateStage();
        StartCoroutine(AnimateButtons(true));
    }

    private IEnumerator AnimateButtons(bool startup = false)
    {
        _canPress = false;
        if (!startup)
        {
            for (int i = 0; i < 6; i++)
            {
                StartCoroutine(SinkButton(ButtonObjs[i]));
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1f);
        }
        for (int i = 0; i < 6; i++)
        {
            StartCoroutine(PopUpButton(ButtonObjs[i], _mainConfig[i]));
            yield return new WaitForSeconds(0.1f);
        }
        _canPress = true;
    }

    private IEnumerator SinkButton(GameObject obj)
    {
        var duration = 0.3f;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            obj.transform.localPosition = new Vector3(0f, Easing.InOutQuad(elapsed, 0f, -0.01f, duration), 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        obj.transform.localPosition = new Vector3(0f, -0.01f, 0f);
    }

    private IEnumerator PopUpButton(GameObject obj, bool raisedUp)
    {
        var duration = 0.3f;
        var elapsed = 0f;
        var targetPos = raisedUp ? 0f : -0.005f;
        while (elapsed < duration)
        {
            obj.transform.localPosition = new Vector3(0f, Easing.InOutQuad(elapsed, -0.01f, targetPos, duration), 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        elapsed = 0f;
        duration = 0.1f;
        while (elapsed < duration)
        {
            obj.transform.localPosition = new Vector3(0f, Easing.InQuad(elapsed, targetPos, targetPos - 0.0025f, duration), 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        elapsed = 0f;
        while (elapsed < duration)
        {
            obj.transform.localPosition = new Vector3(0f, Easing.InQuad(elapsed, targetPos - 0.0025f, targetPos, duration), 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        obj.transform.localPosition = new Vector3(0f, targetPos, 0f);
    }

#pragma warning disable 0414
    private readonly string TwitchHelpMessage = "!{0} press TL [Press the TL button.] | !{0} highlight MR [Highlight the MR button.] | !{0} cycle [Highlight all the buttons.]";
#pragma warning restore 0414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        if (!_canPress)
        {
            yield return "sendtochaterror You can't interact with the module right now!";
            yield break;
        }
        command = command.Trim().ToLowerInvariant();
        if (Regex.Match(command, @"^\s*cycle\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Success)
        {
            yield return null;
            for (int i = 0; i < 6; i++)
            {
                ButtonSels[i].OnHighlight();
                yield return new WaitForSeconds(1.4f);
                ButtonSels[i].OnHighlightEnded();
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }
        var parameters = command.Split(' ');
        if (parameters.Length != 2)
            yield break;
        if (Regex.Match(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Success)
        {
            var pos = new string[] { "tl", "ml", "bl", "tr", "mr", "br" };
            var ix = Array.IndexOf(pos, parameters[1]);
            if (ix == -1)
                yield break;
            yield return null;
            yield return new[] { ButtonSels[ix] };
        }
        if (Regex.Match(parameters[0], @"^\s*highlight\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Success)
        {
            var pos = new string[] { "tl", "ml", "bl", "tr", "mr", "br" };
            var ix = Array.IndexOf(pos, parameters[1]);
            if (ix == -1)
                yield break;
            yield return null;
            ButtonSels[ix].OnHighlight();
            yield return new WaitForSeconds(1.4f);
            ButtonSels[ix].OnHighlightEnded();
            yield break;
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        while (!_moduleSolved)
        {
            while (!_canPress)
                yield return null;
            yield return new[] { ButtonSels[_expectedPress].OnInteract() };
        }
    }
}
