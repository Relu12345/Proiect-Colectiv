using Gtec.Chain.Common.Nodes.Utilities.MatrixLib;
using System;
using static Gtec.Chain.Common.Nodes.InputNodes.ToWorkspace;

namespace Gtec.UnityInterface
{
    public sealed class BCIManager
    {
        public event EventHandler ClassSelectionAvailable;

        public class ClassSelectionAvailableEventArgs : EventArgs
        {
            public uint Class { get; set; }
        };

        private static double _probabilityThreshold = 0.99;
        private static BCIManager _instance = null;
        private ERPSequenceManager _sequenceManager;
        private bool _initialized;

        public static BCIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BCIManager();
                }
                return _instance;
            }
        }

        private BCIManager()
        {
            _initialized = false;
        }

        public void Initialize(uint numberOfClasses)
        {
            if (!_initialized)
            {
                _sequenceManager = new ERPSequenceManager(numberOfClasses);
                ERPBCIManager.Instance.ScoreValueAvailable += OnScoreValueAvailable;
                _initialized = true;
            }
        }

        public void Uninitialize()
        {
            if (_initialized)
            {
                ERPBCIManager.Instance.ScoreValueAvailable -= OnScoreValueAvailable;
                _initialized = false;
            }
        }

        private void OnScoreValueAvailable(object sender, ToWorkspaceEventArgs e)
        {
            Matrix scoreMatrix = new Matrix(e.Data);
            double[] scores = scoreMatrix.GetColumn(1);
            double[] probabilities = scoreMatrix.GetColumn(2);

            //find maxvalue
            bool[] sequence = new bool[scores.Length];
            int maxValPos = 0;
            double maxVal = scores[0];
            double prob = probabilities[0];
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] > maxVal)
                {
                    maxVal = scores[i];
                    maxValPos = i;
                    prob = probabilities[i];
                }
            }

            //convert to boolean array
            for (int i = 0; i < scores.Length; i++)
            {
                if (i == maxValPos)
                    sequence[i] = true;
                else
                    sequence[i] = false;
            }

            //get class
            int selectedClass = _sequenceManager.GetSequenceID(sequence);
            if (prob < _probabilityThreshold || maxVal < 0)
            {
                selectedClass = 0;
            }

            ClassSelectionAvailableEventArgs c = new ClassSelectionAvailableEventArgs();
            c.Class = (uint)selectedClass;
            ClassSelectionAvailable?.Invoke(this, c);
        }
    }
}