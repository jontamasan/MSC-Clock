using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using System;

namespace Clock
{
    public class Clock : Mod
    {
        public override string ID { get { return "Clock"; } }
        public override string Name { get { return "Clock"; } }
        public override string Author { get { return "PigeonBB"; } }
        public override string Version { get { return "1.0"; } }

        GUIStyle _style;
        private Rect _position;
        private uint _hour;
        private uint _hour_count;
        private float _actual_minutes;
        private FsmFloat _raw_minute;
        private bool _doUpdateHour;
        private bool _isNeedReset;
        private bool _isSlept;

        //Called when mod is loading
        public override void OnLoad()
        {
            // GUI Style
            _style = new GUIStyle();
            _style.fontSize = 12 * Screen.width / 1000;
            _style.alignment = TextAnchor.MiddleCenter;
            _style.normal.textColor = Color.white;
            // GUI Position
            _position = new Rect(left: Screen.width - (Screen.width / 20), top: Screen.height - (Screen.height / 30), width: Screen.width / 20, height: 20);
            // in-game clock
            _raw_minute = FsmVariables.GlobalVariables.FindFsmFloat("ClockMinutes");
            _hour_count = 0;
            // global flags
            _isNeedReset = true;
        }

        // Update is called once per frame
        public override void Update()
        {
            if (Application.loadedLevel == 3)
            {
                // once when startup or bak to main menu
                if (_isNeedReset)
                {
                    foreach (var resource in Resources.FindObjectsOfTypeAll<PlayMakerFSM>())
                    {
                        if (resource.name == "SUN")
                        {
                            Reset(resource.FsmVariables.FindFsmBool("Night").Value);
                            break;
                        }
                    }
                    _isNeedReset = false;
                }

                if (Math.Floor(_raw_minute.Value) != 0 && _doUpdateHour == false)
                {
                    _doUpdateHour = true;
                }

                var sleeping = FsmVariables.GlobalVariables.FindFsmBool("PlayerSleeps");
                if (sleeping.Value == true)
                {
                    _isSlept = true;
                }

                if (_isSlept && sleeping.Value == false)
                {
                    _isNeedReset = true;
                    _isSlept = false;
                }

                // update hours
                if (Math.Floor(_raw_minute.Value) == 0 && _doUpdateHour == true)
                {
                    _hour_count++;
                    if (_hour_count >= 2)
                    {
                        _hour++;
                        if (_hour > 23)
                        {
                            _hour = 0;
                        }
                        _hour_count = 0;
                    }
                    _doUpdateHour = false;
                }

                // update minutes
                if (_hour_count == 0)
                {
                    _actual_minutes = _raw_minute.Value / 2;
                }
                else
                {
                    _actual_minutes = _raw_minute.Value / 2 + 30;
                }

            }
            else
            {
                _isNeedReset = true;
            }
        }


        private void Reset(bool isNight)
        {
            var globalTime = (int)Math.Abs(FsmVariables.GlobalVariables.FindFsmFloat(name: "GlobalTime").Value);
            switch (globalTime / 60)
            {
                case 0:
                    if (isNight)
                        _hour = 2;
                    else
                        _hour = 14;
                    break;
                case 1:
                    if (isNight)
                        _hour = 4;
                    else
                        _hour = 16;
                    break;
                case 2:
                    if (isNight)
                        _hour = 18;
                    else
                        _hour = 6;
                    break;
                case 3:
                    if (isNight)
                        _hour = 20;
                    else
                        _hour = 8;
                    break;
                case 4:
                    if (isNight)
                        _hour = 22;
                    else
                        _hour = 10;
                    break;
                case 5:
                    if (isNight)
                        _hour = 0;
                    else
                        _hour = 12;
                    break;
            }
            _hour_count = 0;
            _raw_minute.Value = 0;
            _doUpdateHour = false;
        }

        public override void OnGUI()
        {
            if (Application.loadedLevel == 3)
            {
                GUI.Label(_position, _hour.ToString("00") + ":" + Math.Floor(_actual_minutes).ToString("00"), _style);
            }
        }
    }
}
