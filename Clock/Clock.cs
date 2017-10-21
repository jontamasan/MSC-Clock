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

        private GameObject _clockObject;
        private GameObject _clockData;
        private uint _hour;
        private uint _hour_count;
        private float _actual_minutes;
        private FsmFloat _raw_minute;
        private bool _doUpdateHour;
        private bool _isNeedReset;
        private bool _isSlept;
        private bool _isInitialize;

        //Called when mod is loading
        public override void OnLoad()
        {
            // in-game clock variable
            _raw_minute = FsmVariables.GlobalVariables.FindFsmFloat("ClockMinutes");
            
            // global flags
            _hour_count = 0;
            _isNeedReset = true;
            _isInitialize = true;
        }

        // Update is called once per frame
        public override void Update()
        {
            if (Application.loadedLevelName == "GAME")
            {
                if (_isInitialize)
                {
                    _clockObject = GameObject.Instantiate<GameObject>(GameObject.Find("GUI/HUD/Day"));
                    _clockData = _clockObject.transform.Find("Data").gameObject;
                    GameObject.Destroy(_clockData.GetComponent("PlayMakerFSM"));
                    _clockObject.transform.SetParent(GameObject.Find("GUI/HUD").transform, false);
                    _clockObject.transform.localPosition -= new Vector3(1.5f, 0f, 0);
                    _isInitialize = false;
                }

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
                        _hour = 6;
                    else
                        _hour = 18;
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
                if (_clockObject)
                    _clockData.GetComponent<TextMesh>().text = _hour.ToString("00") + ":" + Math.Floor(_actual_minutes).ToString("00");
            }
        }
    }
}
