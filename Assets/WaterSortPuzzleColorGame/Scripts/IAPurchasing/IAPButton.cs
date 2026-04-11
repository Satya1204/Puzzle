using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WaterSortPuzzleGame.DataClass;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public class IAPButton : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] TMP_Text priceText;


        private ProductKeyType key;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        public void Init(ProductKeyType key)
        {
            this.key = key;

            UpdateState();
        }

        public void UpdateState()
        {
            UpdateState(IAPManager.GetProductData(key));
        }

        public void UpdateState(ProductData product)
        {
            if (priceText == null) return;

            priceText.gameObject.SetActive(true);
           
            if (product != null)
            {
                //Debug.Log("Product:" + product);
                priceText.text = product.GetLocalPrice();
            }
        }
        public void PurchaseState()
        {
            if (priceText == null) return;

            priceText.text = "Purchased";
            priceText.transform.parent.gameObject.GetComponent<Button>().interactable = false;
        }

        private void OnButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            if (!GameManager.IsInternetConnection())
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.InternetError);

                UIController.ShowPage<SuccessErrorPanel>();
            }
            else
            {
                IAPManager.BuyProduct(key);
            }
        }
    }
}