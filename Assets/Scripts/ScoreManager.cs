using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChocolateFactory
{
    public class ScoreManager : MonoBehaviour
    {
        public int score;

        private void OnEnable()
        {
            WorldOutput.OnScoreIncreased += IncreaseScore;
        }

        private void OnDisable()
        {
            WorldOutput.OnScoreIncreased -= IncreaseScore;
        }

        private void IncreaseScore()
        {
            score++;
        }
    }
}
