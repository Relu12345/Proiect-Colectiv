using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gtec.UnityInterface
{
    public class TrainingCompletedDialog : MonoBehaviour
    {
        #region Event Handlers...

        public event EventHandler BtnRetrain_Click;
        public event EventHandler BtnContinueFlashing_Click;

        #endregion

        #region Private Members...

        private Button _btnRetrain;
        private Button _btnContinue;

        #endregion

        void Start()
        {
            Button[] buttons = gameObject.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.gameObject.name.Equals("btnRetrain"))
                    _btnRetrain = button;
                if (button.gameObject.name.Equals("btnContinue"))
                    _btnContinue = button;
            }

            _btnRetrain.onClick.AddListener(OnRetrainClick);
            _btnContinue.onClick.AddListener(OnContinueClick);
        }

        private void OnContinueClick()
        {
            BtnContinueFlashing_Click?.Invoke(this, null);
        }

        private void OnRetrainClick()
        {
            BtnRetrain_Click?.Invoke(this, null);
        }
    }
}