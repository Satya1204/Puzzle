using UnityEngine;

namespace WaterSortPuzzleGame.BottleCodes
{
    public class BottleColorController : MonoBehaviour
    {
        private SpriteRenderer _bottleMaskSR;
        private void Awake()
        {
            _bottleMaskSR = GetComponent<BottleController>().BottleMaskSR;
        }

        public void SetFillAmount(float fillAmount)
        {
            _bottleMaskSR.material.SetFloat(ShaderValueNames.FillAmount, fillAmount);
        }

        public void SetSARM(float value)
        {
            _bottleMaskSR.material.SetFloat(ShaderValueNames.SARM, value);
        }

        public void FillUp(float fillAmountToAdd)
        {
            _bottleMaskSR.material.SetFloat(ShaderValueNames.FillAmount,
                _bottleMaskSR.material.GetFloat(ShaderValueNames.FillAmount) + fillAmountToAdd);
        }

        public void ClampFillAmount(float min, float max)
        {
            _bottleMaskSR.material.SetFloat(ShaderValueNames.FillAmount,
                Mathf.Clamp(_bottleMaskSR.material.GetFloat(ShaderValueNames.FillAmount), min, max));
        }

        public void UpdateColorsOnShader(int[] bottleColorsIndex)
        {
            _bottleMaskSR.material.SetColor(ShaderValueNames.Color1, GameManager.GetColorFromIndex(bottleColorsIndex[0]));
            _bottleMaskSR.material.SetColor(ShaderValueNames.Color2, GameManager.GetColorFromIndex(bottleColorsIndex[1]));
            _bottleMaskSR.material.SetColor(ShaderValueNames.Color3, GameManager.GetColorFromIndex(bottleColorsIndex[2]));
            _bottleMaskSR.material.SetColor(ShaderValueNames.Color4, GameManager.GetColorFromIndex(bottleColorsIndex[3]));
        }

        public void UpdateTopColorValues(BottleData bottleData)
        {
            var bottleColorsIndex = bottleData.BottleColorsIndex;
            var searchColorLength = bottleColorsIndex.Length - (4 - bottleData.NumberOfColorsInBottle);
            FindNumberOfTopLayers(searchColorLength, bottleData);
            FindTopColor(searchColorLength, bottleData);
            UpdateColorsOnShader(bottleData.BottleColorsIndex);
            
        }
        
        private void FindNumberOfTopLayers(int searchColorLength, BottleData bottleData)
        {
            int numberOfTopColorLayers;
            var bottleColorsIndex = bottleData.BottleColorsIndex;

            if (searchColorLength <= 0)
            {
                numberOfTopColorLayers = 0;
                bottleData.NumberOfTopColorLayers = numberOfTopColorLayers;
                return;
            }
            numberOfTopColorLayers = 1;

            for (var i = searchColorLength - 1; i >= 1; i--)
            {
                if (bottleColorsIndex[i] == bottleColorsIndex[i - 1])
                {
                    numberOfTopColorLayers++;
                }
                else
                {
                    break;
                }
            }

            bottleData.NumberOfTopColorLayers = numberOfTopColorLayers;
        }

        public void CheckIsBottleSorted(BottleData bottleData)
        {
            var isBottleSorted = bottleData.NumberOfTopColorLayers == 4;

            if (isBottleSorted)
            {
                bottleData.BottleSorted = true;
                EventManager.CheckIsLevelCompleted?.Invoke();
            }
            else
            {
                bottleData.BottleSorted = false;
            }
        }

        private void FindTopColor(int searchColorLength, BottleData bottleData)
        {
            var assignValue = Mathf.Clamp(searchColorLength - 1, 0, int.MaxValue);
            bottleData.TopColorIndex = bottleData.BottleColorsIndex[assignValue];
        }

        public void PlayParticleFX()
        {
            var particlePosition = transform.position + new Vector3(0, 1.25f, -1);
            var particleRotation = GameManager.Instance.ConfettiParticle.transform.rotation;
            var particleFX = Instantiate(GameManager.Instance.ConfettiParticle, particlePosition, particleRotation);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.confetti);
            Destroy(particleFX, 3);
        }
    }
}