using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MyMonoBehaviour
    {
        public float TimeRemaining { get; set; }
        public long EarnedPoints { get; set; }
        public bool CanMove { get; set; }
        public LetterManager LetterManager { get; set; }
        public string Name { get; set; }
        public TheLetterManager TheLetterManager { get; set; }
        public Credit Credit;

        public void Start()
        {
            TimeRemaining = 30;
            EarnedPoints = 0;
            Instantiate(Credit);
        }

        public void Update()
        {
            if (!CanMove) return;
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0)
            {
                CanMove = false;
            }
        }
    }
}
