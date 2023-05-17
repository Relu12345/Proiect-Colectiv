using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gtec.UnityInterface
{
    public class TrainingDialog : MonoBehaviour
    {
        #region Event Handlers...

        public event EventHandler BtnStartStopFlashing_Click;

        #endregion

        #region Properties...

        public bool IsFlashing
        {
            get { return _isFlashing; }
        }

        #endregion

        private Button _btnStartStop;
        private string _txtStart = "Start Flashing";
        private string _txtStop = "Stop Flashing";
        private bool _isFlashing = false;

        void Start()
        {
            _btnStartStop = gameObject.GetComponentInChildren<Button>();
            _btnStartStop.GetComponentInChildren<Text>().text = _txtStart;
            _btnStartStop.onClick.AddListener(OnClick);
            _isFlashing = false;
        }

        public void Reset()
        {
            _btnStartStop.GetComponentInChildren<Text>().text = _txtStart;
            _isFlashing = false;
        }

        private void OnClick()
        {
            if (!_isFlashing)
            {
                _btnStartStop.GetComponentInChildren<Text>().text = _txtStop;
                _isFlashing = true;
            }
            else
            {
                _btnStartStop.GetComponentInChildren<Text>().text = _txtStart;
                _isFlashing = false;
            }
            BtnStartStopFlashing_Click?.Invoke(this, null);
        }
    }
}