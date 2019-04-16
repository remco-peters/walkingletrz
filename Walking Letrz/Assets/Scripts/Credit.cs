using UnityEngine;

public class Credit : MonoBehaviour
{
        public int Count { get; set; }

        private void Awake()
        {
            Count = PlayerPrefs.GetInt("numberOfCredits", 0);
        }

        public void AddCredits(int amount)
        {
            
            Count = PlayerPrefs.GetInt("numberOfCredits", 0);
            Debug.Log($"Credits before add: {Count}");
            Count += amount;
            Debug.Log($"Credits after add: {Count}");
            PlayerPrefs.SetInt("numberOfCredits", Count);
            PlayerPrefs.Save();
        }
}
