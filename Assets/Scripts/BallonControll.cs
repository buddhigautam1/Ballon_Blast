using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BallonControll : MonoBehaviour
{
    public float upSpeed;
    int score = 0;

   
    //Audio Source
   AudioSource audioSource;

    public TextMeshProUGUI scoreText;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        // check ballon beyond the screen then reset the game
        if (transform.position.y >=7f)
        {
            SceneManager.LoadScene("GameOver");

        }
            
        
    }

    private void FixedUpdate()
    {
        transform.Translate(0, upSpeed, 0);
    }

    private void OnMouseDown()
    {
        score++;
        
        scoreText.text = score.ToString();
        audioSource.Play();
       
        ResetPosition();



    }
 

    private void ResetPosition()
    {
        float randoX = Random.Range(-2.3f, 2.3f);

        transform.position = new Vector2(randoX, -7f);
    }
  

}
