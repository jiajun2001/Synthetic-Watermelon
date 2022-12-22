using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState 
{
    Ready = 0,
    StandBy = 1, // StandBy -> InProgress
    InProgress = 2, // InProgress -> StandBy
    GameOver = 3,
    CalculateScore = 4
}

public class Manager : MonoBehaviour
{
    public GameObject[] fruitList;
    public GameObject fruitCreatePosition;
    public GameObject startBtn;
    public Vector3 synthesisScale = new Vector3(0, 0, 0);
    public float totalScore = 0f;
    public Text totalScoreText;
    public Text highestScoreText;

    public AudioSource synthesizeAudio;
    public AudioSource hitFloorAudio;
    public AudioSource hitFruitAudio;
    public AudioSource finishGameAudio;

    // By creating a static variable, we can use it in other classes.
    public static Manager gameManagerInstance; 
    public GameState gameState = GameState.Ready;

    

    // Awake is called before Start()
    void Awake() 
    {
        gameManagerInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() 
    {
        Debug.Log("Start");
        // Read the highest score
        float highestScore = PlayerPrefs.GetFloat("HighestScore");
        totalScoreText.text = "Score: " + totalScore.ToString();
        highestScoreText.text = "Highest Score: " + highestScore.ToString();
        CreateFruit();
        startBtn.SetActive(false);
        gameManagerInstance.gameState = GameState.StandBy;
    }

    public void InvokeCreateFruit(float invokeTime)
    {
        Invoke("CreateFruit", invokeTime);
    }

    public void CreateFruit() 
    {
        int index = Random.Range(0, 4); // 0, 1, 2, 3
        if (fruitList.Length >= index && fruitList[index] != null) {
            GameObject fruitObject = fruitList[index];
            var currentFruit = Instantiate(fruitObject, fruitCreatePosition.transform.position, fruitObject.transform.rotation);
            currentFruit.GetComponent<Fruit>().fruitState = FruitState.StandBy;
        }
    }

    public void synthesize(FruitType myType, Vector3 myPosition, Vector3 collisionPosition)  
    {
        Vector3 centerPosition = (myPosition + collisionPosition) / 2;
        int index = (int)myType + 1;
        GameObject syntheticFruit = fruitList[index];
        var currentFruit = Instantiate(syntheticFruit, centerPosition, syntheticFruit.transform.rotation);
        currentFruit.GetComponent<Fruit>().fruitState = FruitState.Collision;
        currentFruit.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        currentFruit.transform.localScale = synthesisScale;

        // Audio
        synthesizeAudio.Play();
    }
}
