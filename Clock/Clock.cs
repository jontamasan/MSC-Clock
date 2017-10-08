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
        private bool _isUpdate;
        private bool _isNeedReset;
        bool _isSlept;

        //Called when mod is loading
        public override void OnLoad()
        {
            _style = new GUIStyle();
            _style.fontSize = 12 * Screen.width / 1000;
            _style.alignment = TextAnchor.MiddleCenter;
            _style.normal.textColor = Color.white;
            _position = new Rect(left: Screen.width - 80, top: 20, width: 80, height: 20);
            _raw_minute = FsmVariables.GlobalVariables.FindFsmFloat("ClockMinutes");
            _hour_count = 0;
            _isNeedReset = true;
            _isSlept = false;
        }

        // Update is called once per frame
        public override void Update()
        {
            if (Application.loadedLevel == 3)
            {
                // only once when startup
                if (_isNeedReset)
                {
                    Reset();
                    _isNeedReset = false;
                }

                if (Math.Floor(_raw_minute.Value) != 0 && _isUpdate == false)
                {
                    _isUpdate = true;
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
                if (Math.Floor(_raw_minute.Value) == 0 && _isUpdate == true)
                {
                    _hour_count++;
                    if (_hour_count == 1)
                    {
                        if (_hour > 12)
                        {
                            _hour = 1;
                        }
                    }
                    else if (_hour_count >= 2)
                    {
                        _hour++;
                        _hour_count = 0;
                    }
                    _isUpdate = false;
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


        private void Reset()
        {
            var globalTime = (int)Math.Abs(FsmVariables.GlobalVariables.FindFsmFloat(name: "GlobalTime").Value);
            switch (globalTime / 60)
            {
                case 0: _hour = 2; break;
                case 1: _hour = 4; break;
                case 2: _hour = 6; break;
                case 3: _hour = 8; break;
                case 4: _hour = 10; break;
                case 5: _hour = 12; break;
            }
            _hour_count = 0;
            _raw_minute.Value = 0;
            _isUpdate = false;
        }

        public override void OnGUI()
        {


            //GUI.Label(new Rect(left: 0, top: 20, width: 200, height: 200), "hour_count: " + hour_count);
            //GUI.Label(new Rect(left: 0, top: 40, width: 200, height: 200), "PlayerSleeps: " + FsmVariables.GlobalVariables.FindFsmBool("PlayerSleeps"));
            //GUI.Label(new Rect(left: 0, top: 60, width: 200, height: 200), "loadedLevel: " + Application.loadedLevel);
            if (Application.loadedLevel == 3)
            {
                GUI.Label(_position, _hour + ":" + Math.Floor(_actual_minutes).ToString("00"), _style);
                //position.y += line_height;
                //GUI.Label(position, "RawMinutes: " + raw_minute);
                //position.y += line_height;
                //GUI.Label(position, "ActualMinutes: " + actual_minutes);
                //position.y += line_height;
                //GUI.Label(position, "GlobalTime: " + FsmVariables.GlobalVariables.FindFsmFloat(name: "GlobalTime"));
                //position.y += line_height;
                //GUI.Label(position, "ToolWrenchSize: " + FsmVariables.GlobalVariables.FindFsmFloat(name: "ToolWrenchSize"));
            }
        }
    }
}
