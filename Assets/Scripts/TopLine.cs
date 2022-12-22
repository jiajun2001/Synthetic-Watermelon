using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TopLine : MonoBehaviour
{
    public bool isMove = false;
    public float speed = 0.2f;
    public float limit_y = -5.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            if (this.transform.position.y > limit_y) 
            {
                this.transform.Translate(Vector3.down * 30 * speed * Time.deltaTime);
                Manager.gameManagerInstance.synthesizeAudio.Play();
            }
            else 
            {
                isMove = false;
                Manager.gameManagerInstance.finishGameAudio.Play();
                // Reload the game
                Invoke("reloadGame", 3f);
            }
        }
    }

    void reloadGame()
    {
        float highestScore = PlayerPrefs.GetFloat("HighestScore");
        if (highestScore < Manager.gameManagerInstance.totalScore)
        {
            PlayerPrefs.SetFloat("HighestScore", Manager.gameManagerInstance.totalScore);
        }      
        SceneManager.LoadScene("SyntheticWatermelon");
    }

    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.tag.Contains("Fruit"))
        {
            Debug.Log("tag");
            if ((int)Manager.gameManagerInstance.gameState < (int)GameState.GameOver)
            {
                Debug.Log("gameState");
                if (collider.gameObject.GetComponent<Fruit>().fruitState == FruitState.Collision) 
                {
                    Debug.Log("GameOver");
                    // Game Over
                    Manager.gameManagerInstance.gameState = GameState.GameOver;
                    Invoke("OpenMoveAndCalculateScore", 0.5f);
                }
            }

            // Destroy the rest fruits
            if ((int)collider.gameObject.GetComponent<Fruit>().fruitState == (int)FruitState.Collision)
            {
                float currentScore = collider.GetComponent<Fruit>().fruitScore;
                Manager.gameManagerInstance.totalScore += currentScore;
                Manager.gameManagerInstance.totalScoreText.text = "Score: " + Manager.gameManagerInstance.totalScore.ToString();
                Destroy(collider.gameObject);
            }
        }
    }

    void OpenMoveAndCalculateScore()
    {
        isMove = true;
        Manager.gameManagerInstance.gameState = GameState.CalculateScore;
    }

    void CalculateScore()
    {

    }
}
