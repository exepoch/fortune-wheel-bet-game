using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class WheelController : MonoBehaviour
    {
        #region Events

        public static Action OnSpinStart;
        public static OnSpin OnSpinResult;
        public delegate void OnSpin(string result);

        #endregion

        #region Modifiers

        [SerializeField] private float apiWaitTime = 2f; //Wait time for api response simulation
        [SerializeField] private float rotateTime;

        #endregion

        #region SerializedInstances

        [SerializeField] private GameObject firstSpin; //First spin gameObject
        [SerializeField] private PointerBehavior pointerAnimator; //spinPointer rotate animator
        [SerializeField] private AnimationCurve animationCurve; //Speed curve for rotation animation of wheel

        [SerializeField] private Animator spinnerAnim; //Spin button interactable visual animate
        [SerializeField] private AudioClip nextSectionSound, goodResult,badResult; //sopunds

        #endregion

        #region Data

        [SerializeField] private List<Image> shadows; //Spin visual effect sprites

        [SerializeField] private List<Transform> wheels;
        [SerializeField] private List<Wheel> wheelsData; //Init at editor or From JSON File
        private List<float2> _angles = new List<float2>(); //Current angles for spin target

        #endregion

        #region PrivateProp

        [SerializeField]private Transform _currentWheel; //Active wheel to rotate
        private Api _api; //Rest api simulator
        private IEnumerator _spinAction; //action holder to control spin action
        private AudioSource _audioSource;
        private Dictionary<string, Angle> _dictionary; //Rotate angle dictionary

        private int _currentWheelIndex = -1;
        private string _spinSectionKey; //Multiplier Key (x0.5f , x1 , x2 , next ..)

        private bool _isSpinning;
        private bool _wheelChanged; //loop cutter for updating angle list

        #endregion

        #region UnityEvents

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            _api = new Api();

            
            //Arrange all wheel section angles wtihin the dictionary for easy access
            foreach (var wheel in wheelsData)
            {
                wheel.Initialize();
            }
            
            ChangeWheel(0); //Set initial wheel index
        }

        #endregion

        #region PublicMethods

        //Main spin command method to spin the current wheel
        public void Spin()
        {
            if(_isSpinning || _spinAction != null)
                return;

            firstSpin.SetActive(false);
            _spinAction = SpinWheel();
            StartCoroutine(_spinAction);
        }

        #endregion

        #region PrivateMethods

        private void UpdateAngleList()
        {
            _angles = _dictionary[_spinSectionKey].GetAngles();
            _wheelChanged = false;
        }

        //Updates the current wheel index and arrange angle lists and dictionary
        private void ChangeWheel(int newIndex)
        {
            print("Input"+newIndex);
            print("Currnt"+_currentWheelIndex);
            if (newIndex == _currentWheelIndex)
            {
                _wheelChanged = false;
                return;
            }

            _currentWheelIndex = newIndex;
            _currentWheel = wheels[newIndex];
            _dictionary = wheelsData[_currentWheelIndex].GetDic();
            _wheelChanged = true;
        }

        #endregion

        #region CoRoutines

        //Spin sequance
        private IEnumerator SpinWheel()
        {
            yield return RequestAngleKeyFromApi(); //Gets a random key from api
            
            var randomIndex = _angles[Random.Range(0, _angles.Count)]; //Get random minmax value from angles
            var randomAngle = Random.Range(randomIndex.x, randomIndex.y); //Get randım angel between min and max angle
            
            Debug.Log("Generated MultiplierKey : " + _spinSectionKey);

            //Make a shadow on inactive wheels
            for (int i = 0; i < 3; i++)
            {
                if(_currentWheelIndex == i)
                    continue;
                
                shadows[i].DOFade(.5f, .5f);
            }
            
            OnSpinStart?.Invoke();
            spinnerAnim.enabled = false;
            _isSpinning = true;
            pointerAnimator.Animate(true);

            var spinAngle = Vector3.zero;
            spinAngle.z += 360f * 10f + randomAngle;

            //Rotates the wheel according to given angle
            _currentWheel.DOLocalRotate(spinAngle, rotateTime, RotateMode.FastBeyond360).SetEase(animationCurve).OnComplete(
                () =>
                {
                    spinnerAnim.enabled = true;
                    pointerAnimator.Animate(false);
                    OnSpinResult?.Invoke(_spinSectionKey);
                });

            yield return new WaitForSeconds(rotateTime);
            
            //Remove all shadows to see all wheels
            for (var i = 0; i < 3; i++)
            {
                shadows[i].DOFade(0f, .5f);
            }

            yield return SpinEndAnimation();

            switch (_spinSectionKey)
            {
                case "next":
                    StartCoroutine(SpinNextSection()); //If next key generated, spin the next wheel auto
                    break;
                case "0":
                case "0.5":
                    _audioSource.PlayOneShot(badResult);
                    _isSpinning = false;
                    _spinAction = null;
                    ChangeWheel(0);
                    break;
                default:
                    _audioSource.PlayOneShot(goodResult);
                    _isSpinning = false;
                    _spinAction = null;
                    ChangeWheel(0);
                    break;
            }
        }

        
        //return a random key from active wheel angles
        private IEnumerator RequestAngleKeyFromApi()
        {
            if (_api == null) yield break; //No Connection?

            _api.GetKey(_currentWheelIndex, out var newKey);

            if (newKey != _spinSectionKey || _wheelChanged)
            {
                _spinSectionKey = newKey;
                UpdateAngleList();
            }

            //Api wait time simulation
            yield return new WaitForSeconds(apiWaitTime);
        }

        //Spins next wheel
        private IEnumerator SpinNextSection()
        {
            ChangeWheel(_currentWheelIndex +1);
            _audioSource.PlayOneShot(nextSectionSound);
            yield return new WaitForSeconds(nextSectionSound.length-1f);
            
            _isSpinning = false;
            _spinAction = null;
            Spin();

            yield return null;
        }

        private IEnumerator SpinEndAnimation()
        {
            var scaleVector = Vector3.one * .1f;
            //SpinAnimation
            wheels[2].DOPunchScale(scaleVector, .5f, 2).OnComplete(() =>
            {
                wheels[1].DOPunchScale(scaleVector, .5f,2).OnComplete(() =>
                {
                    wheels[0].DOPunchScale(scaleVector, .5f,2);
                });
            });
            
            yield return new WaitForSeconds(1.5f);
        }

        #endregion

        #region CustomDataClasses

        private class Api
        {
            //Contains the keys and returns a randım key angle to spin to
            private readonly List<string[]> _keyList = new List<string[]> 
            { 
                new []{"0","0.5", "1", "1.5", "next"} ,
                new []{"0","1","2", "1.5","5", "next"} ,
                new []{"0","2","3","5","10","35"}
            };

            public void GetKey(int wheelNumber, out string key)
            {
                var wheel = _keyList[wheelNumber];
                key = wheel[Random.Range(0, wheel.Length)];
            }
        }


        [Serializable]
        public class Wheel
        {
            /// <summary>
            /// Seperate key value pair Dictionary simulation for Non-Serializable collection
            /// "prize" holds keys
            /// "minMaxAngle" hold values for editing in editor mod.
            /// </summary>
            [SerializeField] private List<string> prize; //Must be equal to section count, multiplier prize of section, result multiplier value
            [SerializeField] private List<Angle> angleList; //Values of the prize's
            private Dictionary<string, Angle> _dictionary;
            private bool _isInitialized;

            public void Initialize()
            {
                if(_isInitialized)
                    return;
            
                _dictionary = new Dictionary<string, Angle>();
            
                for (var i = 0; i < prize.Count; i++)
                {
                    _dictionary.Add(prize[i], angleList[i]);
                }

                _isInitialized = true;
            }

            public Dictionary<string, Angle> GetDic()
            {
                return _dictionary;
            }
        }
    
        [Serializable]
        public struct Angle
        {
            //X is the min angle, Y is the max angle
            //1 Prize can have multiple min max values
            [SerializeField] private List<float2> angles;

            public List<float2> GetAngles()
            {
                return angles;
            }
        }

        #endregion
    }
}
