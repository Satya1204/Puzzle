using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WaterSortPuzzleGame.Managers
{
    public class ConfettiManager : MonoBehaviour
    {

        [SerializeField] private GameObject _confetti;
        private GameObject particleFX;

        private void OnEnable()
        {
            EventManager.WinEffect += StartConfettiEffects;
            EventManager.LoadNextLevel += StopConfettiEffect;
        }

        private void OnDisable()
        {
            EventManager.WinEffect -= StartConfettiEffects;
            EventManager.LoadNextLevel -= StopConfettiEffect;
        }

        private void StartConfettiEffects()
        {
            StartCoroutine(PlayConfetti());
        }

        private void StopConfettiEffect()
        {

            Destroy(particleFX);
        }

        private IEnumerator PlayConfetti()
        {
            var point = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f));
            point.z = 0;
            particleFX = Instantiate(_confetti, point, Quaternion.identity);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.confetti);
            yield return null;

        }
    }

}

