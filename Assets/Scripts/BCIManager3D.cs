using Gtec.Chain.Common.Nodes.Utilities.LDA;
using Gtec.Chain.Common.SignalProcessingPipelines;
using Gtec.Chain.Common.Templates.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Gtec.Chain.Common.Templates.DataAcquisitionUnit.DataAcquisitionUnit;
using static Gtec.UnityInterface.ERPBCIManager;

namespace Gtec.UnityInterface
{
    public class BCIManager3D : MonoBehaviour
    {
        private ERPFlashController3D _flashController;
        private Canvas _cvTraining;
        private Canvas _cvConnectionDialog;
        private Canvas _cvTrainingCompletedDialog;
        private ConnectionDialog _connectionDialog;
        private TrainingDialog _trainingDialog;
        private TrainingCompletedDialog _trainingCompletedDialog;
        private States _currentState;
        private ERPPipeline.Mode _currentMode;
        private bool _connectionStateChanged;
        private bool _modeChanged;
        private bool _classifierCalculated;
        private bool _calculatingClassifier;
        private bool _classifierCalculationFailed;
        private bool _startFlashing;
        private System.Diagnostics.Stopwatch _sw;
        private int _flashingDelayMs = 1000;

        void Start()
        {
            _sw = new System.Diagnostics.Stopwatch();

            //get bci dialogs
            Canvas[] dialogs = gameObject.GetComponentsInChildren<Canvas>();
            foreach (Canvas dialog in dialogs)
            {
                if (dialog.name.Equals("dlgConnection"))
                    _cvConnectionDialog = dialog;
                else if (dialog.name.Equals("dlgTraining"))
                    _cvTraining = dialog;
                else if (dialog.name.Equals("dlgTrainingCompleted"))
                    _cvTrainingCompletedDialog = dialog;
            }

            //connection dialog
            _connectionDialog = gameObject.GetComponentInChildren<ConnectionDialog>();
            _connectionDialog.BtnConnect_Click += OnBtnConnectClick;
            _connectionStateChanged = false;
            _modeChanged = false;
            _startFlashing = false;

            //training dialog
            _trainingDialog = gameObject.GetComponentInChildren<TrainingDialog>();
            _trainingDialog.BtnStartStopFlashing_Click += OnBtnStartStopFlashing_Click;

            //training completed dialog
            _trainingCompletedDialog = gameObject.GetComponentInChildren<TrainingCompletedDialog>();
            _trainingCompletedDialog.BtnContinueFlashing_Click += OnBtnContinue_Click;
            _trainingCompletedDialog.BtnRetrain_Click += OnBtnRetrain_Click;

            //set dialog visibility
            _cvConnectionDialog.gameObject.SetActive(true);
            _cvTraining.gameObject.SetActive(false);
            _cvTrainingCompletedDialog.gameObject.SetActive(false);

            //flash controller
            _flashController = gameObject.GetComponent<ERPFlashController3D>();
            _flashController.FlashingStarted += OnFlashingStarted;
            _flashController.FlashingStopped += OnFlashingStopped;
            _flashController.Trigger += OnTrigger;

            BCIManager.Instance.Initialize(_flashController.NumberOfClasses);

            //bci manager
            _currentMode = ERPPipeline.Mode.Training;
            ERPBCIManager.Instance.RuntimeExceptionOccured += OnRuntimeExceptionOccured;
            ERPBCIManager.Instance.ModeChanged += OnModeChanged;
            ERPBCIManager.Instance.ClassifierCalculated += OnClassifierAvailable;
            ERPBCIManager.Instance.ClassifierCalculationFailed += OnClassifierCalculationFailed;
            ERPBCIManager.Instance.StateChanged += OnBCIStateChanged;

            _classifierCalculated = false;
            _calculatingClassifier = false;
        }

        private void OnTrigger(object sender, ERPTriggerEventArgs e)
        {
            if (ERPBCIManager.Instance.Initialized)
                ERPBCIManager.Instance.SetTrigger(e.IsTarget, e.Id, e.Trial, e.IsLastOfTrial);
        }

        private void OnClassifierAvailable(object sender, EventArgs e)
        {
            _calculatingClassifier = false;
            _classifierCalculated = true;
            Dictionary<int, Accuracy> accuracy = ERPBCIManager.Instance.Accuracy();

            string classifierAccuracy = string.Empty;
            foreach (KeyValuePair<int, Accuracy> kvp in accuracy)
                classifierAccuracy += string.Format("Averages: {0}, Accuracy {1}\n", kvp.Key, kvp.Value.Mean);
            UnityEngine.Debug.Log(string.Format("Classifier calculated.\n{0}", classifierAccuracy));

            double maxAccuracy = 0;
            int numberOfAverages = 1;
            for (int i = 0; i < accuracy.Count; i++)
            {
                if (accuracy[i + 1].Mean > maxAccuracy)
                {
                    maxAccuracy = accuracy[i + 1].Mean;
                    numberOfAverages = i + 1;

                    if (maxAccuracy == 100)
                        break;
                }
            }

            //autoselect number of averages
            if (accuracy.Count <= 4 && maxAccuracy < 100)
                numberOfAverages = numberOfAverages * 2;
            else
                numberOfAverages += 1;

            ERPBCIManager.Instance.NumberOfAverages = numberOfAverages;
            UnityEngine.Debug.Log(string.Format("{0} averages selected.", numberOfAverages));
        }

        private void OnClassifierCalculationFailed(object sender, EventArgs e)
        {
            _calculatingClassifier = false;
            _classifierCalculated = false;
            _classifierCalculationFailed = true;

            Debug.Log("Could not calculate classifier.");
        }

        private void OnFlashingStarted(object sender, EventArgs e)
        {
            Debug.Log("Flashing started");
        }

        private void OnFlashingStopped(object sender, EventArgs e)
        {
            Debug.Log("Flashing stopped");
            if (_currentMode == ERPPipeline.Mode.Training)
            {
                _calculatingClassifier = true;
                _classifierCalculated = false;
                ERPBCIManager.Instance.Train();
            }
        }

        private void OnModeChanged(object sender, ModeChangedEventArgs e)
        {
            _currentMode = e.Mode;
            _modeChanged = true;
            Debug.Log(String.Format("Mode Changed to '{0}'", _currentMode.ToString()));
        }

        private void OnBCIStateChanged(object sender, StateChangedEventArgs e)
        {
            _currentState = e.State;
            _connectionStateChanged = true;
            Debug.Log(String.Format("Device state changed to '{0}'", e.State));
        }

        private void OnRuntimeExceptionOccured(object sender, RuntimeExceptionEventArgs e)
        {
            Debug.Log(String.Format("A runtime exception occured.\n{0}\n{1}", e.Exception.Message, e.Exception.StackTrace));
        }

        private void OnBtnConnectClick(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    _currentState = States.Connecting;
                    _connectionStateChanged = true;
                    ERPBCIManager.Instance.Initialize(_connectionDialog.Serial);
                }
                catch (Exception ex)
                {
                    try
                    {
                        ERPBCIManager.Instance.Uninitialize();
                    }
                    catch
                    {
                    //DO NOTHING 
                    }

                    _currentState = States.Disconnected;
                    _connectionStateChanged = true;
                    Debug.Log(String.Format("Device initialization failed.\n{0}\n{1}", ex.Message, ex.StackTrace));
                }
            }).Start();
        }

        private void OnBtnStartStopFlashing_Click(object sender, EventArgs e)
        {
            if (_trainingDialog.IsFlashing)
            {
                _startFlashing = true;
            }
            else
            {
                _flashController.StopFlashing();
            }
        }

        private void OnBtnRetrain_Click(object sender, EventArgs e)
        {
            ERPBCIManager.Instance.Configure(ERPPipeline.Mode.Training);
            _trainingDialog.Reset();
        }

        private void OnBtnContinue_Click(object sender, EventArgs e)
        {
            ERPBCIManager.Instance.Configure(ERPPipeline.Mode.Application);
            _startFlashing = true;
        }

        void Update()
        {
            //show/hide connection dialog
            if (_connectionStateChanged || _modeChanged)
            {
                if (_currentState == States.Disconnected)
                {
                    _cvConnectionDialog.gameObject.SetActive(true);
                    _cvTraining.gameObject.SetActive(false);
                    _cvTrainingCompletedDialog.gameObject.SetActive(false);
                }
                else if (_currentState == States.Connecting)
                {
                    _cvConnectionDialog.gameObject.SetActive(false);
                    _cvTraining.gameObject.SetActive(false);
                    _cvTrainingCompletedDialog.gameObject.SetActive(false);
                }
                else
                {
                    if (_currentMode == ERPPipeline.Mode.Training)
                    {
                        _cvConnectionDialog.gameObject.SetActive(false);
                        _cvTraining.gameObject.SetActive(true);
                        _cvTrainingCompletedDialog.gameObject.SetActive(false);
                    }
                    else
                    {
                        _cvConnectionDialog.gameObject.SetActive(false);
                        _cvTraining.gameObject.SetActive(false);
                        _cvTrainingCompletedDialog.gameObject.SetActive(false);
                    }
                }
                _connectionStateChanged = false;
                _modeChanged = false;
            }

            if (_startFlashing && _currentState == States.Acquiring)
            {
                if (!_sw.IsRunning)
                {
                    _sw.Reset();
                    _sw.Start();
                }
                if (_sw.IsRunning && _sw.ElapsedMilliseconds > _flashingDelayMs)
                {
                    if (_currentMode == ERPPipeline.Mode.Training)
                        _flashController.StartFlashing(ERPFlashController3D.Mode.Training);
                    else
                        _flashController.StartFlashing(ERPFlashController3D.Mode.Application);
                    _startFlashing = false;
                    _sw.Stop();
                }
            }

            if (_calculatingClassifier)
            {
                _cvConnectionDialog.gameObject.SetActive(false);
                _cvTraining.gameObject.SetActive(false);
                _cvTrainingCompletedDialog.gameObject.SetActive(false);
                _calculatingClassifier = false;
            }

            if (_classifierCalculated)
            {
                _cvConnectionDialog.gameObject.SetActive(false);
                _cvTraining.gameObject.SetActive(true);
                _cvTrainingCompletedDialog.gameObject.SetActive(true);
                _classifierCalculated = false;
            }

            if (_classifierCalculationFailed)
            {
                ERPBCIManager.Instance.Configure(ERPPipeline.Mode.Training);
                _trainingDialog.Reset();
                _classifierCalculationFailed = false;
            }
        }

        private void OnApplicationQuit()
        {
            _connectionDialog.BtnConnect_Click -= OnBtnConnectClick;

            _trainingDialog.BtnStartStopFlashing_Click -= OnBtnStartStopFlashing_Click;

            _trainingCompletedDialog.BtnContinueFlashing_Click -= OnBtnContinue_Click;
            _trainingCompletedDialog.BtnRetrain_Click -= OnBtnRetrain_Click;

            _flashController.FlashingStarted -= OnFlashingStarted;
            _flashController.FlashingStopped -= OnFlashingStopped;
            _flashController.Trigger -= OnTrigger;

            BCIManager.Instance.Uninitialize();

            ERPBCIManager.Instance.RuntimeExceptionOccured -= OnRuntimeExceptionOccured;
            ERPBCIManager.Instance.ModeChanged -= OnModeChanged;
            ERPBCIManager.Instance.ClassifierCalculated -= OnClassifierAvailable;
            ERPBCIManager.Instance.ClassifierCalculationFailed -= OnClassifierCalculationFailed;
            ERPBCIManager.Instance.StateChanged -= OnBCIStateChanged;

            ERPBCIManager.Instance.Uninitialize();
        }
    }
}