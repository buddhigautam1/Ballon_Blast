using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Text pointsText;

    private void OnEnable()
    {
        Setup(GameSessionStats.LastScore);
    }

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        if (pointsText != null)
        {
            pointsText.text = score + " POINTS";
        }
    }
}
