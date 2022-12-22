using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FruitType 
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
}

// Fruit State Change:

// Initialization: Ready
// StartGame -> CreateFruit: StandBy
// Update -> GetMouseButtonUp: Dropping
// OnCollisionEnter2D: Collision
// Update -> GetMouseButtonUp -> InvokeCreateFruit -> CreateFruit: StandBy
// (Iteration)

public enum FruitState 
{
    Ready = 0,
    StandBy = 1,
    Dropping = 2,
    Collision = 3,
}


public class Fruit : MonoBehaviour
{
    public FruitType fruitType = FruitType.One;
    public FruitState fruitState = FruitState.Ready;
    private bool isMove = false;
    public float limit_x_left = 2f;
    public float limit_x_right = 2f;
    public Vector3 originalScale = new Vector3(0, 0, 0);
    public float fruitScore = 1.0f;

    void Awake() {
        originalScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Condition for moving the created fruit
        if (Manager.gameManagerInstance.gameState == GameState.StandBy && fruitState == FruitState.StandBy) 
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                isMove = true;
            }

            if (isMove) 
            {
                // Convert Computer screen position of the mouse to Unity screen position 
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
                this.gameObject.GetComponent<Transform>().position = new Vector3(mousePos.x, this.gameObject.GetComponent<Transform>().position.y, this.gameObject.GetComponent<Transform>().position.z);
            }

            if (Input.GetMouseButtonUp(0) && isMove) 
            {
                isMove = false;
                // Change the gravity of the newly created fruit
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;

                // Change the fruit state
                fruitState = FruitState.Dropping;

                // Change game state
                Manager.gameManagerInstance.gameState = GameState.InProgress;

                // Create a new fruit
                Manager.gameManagerInstance.InvokeCreateFruit(0.9f);
            }
        }

        // Restrict the dropping position
        if (this.transform.position.x < limit_x_left + 0.8) 
        {
            this.transform.position = new Vector3(limit_x_left + (float)1.30, this.transform.position.y, this.transform.position.z);
        }
        if (this.transform.position.x > limit_x_right + 1.5) 
        {
            this.transform.position = new Vector3(limit_x_right + (float)1.50, this.transform.position.y, this.transform.position.z);
        }

        // Restore the size of synthesized fruit
        if (this.transform.localScale.x < originalScale.x) 
        {
            this.transform.localScale += new Vector3(1, 1, 1) * 0.05f;
        }

    }

    // Detect Collision
    // Be careful: This function will execute all the time if there are collisions.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (fruitState == FruitState.Dropping) 
        {
            if (collision.gameObject.tag.Contains("Floor") || collision.gameObject.tag.Contains("Fruit")) 
            {
                Manager.gameManagerInstance.gameState = GameState.StandBy;
                fruitState = FruitState.Collision;
                if (collision.gameObject.tag.Contains("Floor")) 
                {
                    Manager.gameManagerInstance.hitFloorAudio.Play();
                } 
                if (collision.gameObject.tag.Contains("Fruit")) 
                {
                    Manager.gameManagerInstance.hitFruitAudio.Play();
                }
            }
        }

        // If the fruit is Dropping or Collision
        if ((int)fruitState >= (int)FruitState.Dropping)  
        {
            if (collision.gameObject.tag.Contains("Fruit"))
            {
                if (fruitType == collision.gameObject.GetComponent<Fruit>().fruitType && (int)fruitType < 10) 
                {
                    // Only execute one synthesize() method
                    float position1 = this.transform.position.x + this.transform.position.y;
                    float position2 = collision.transform.position.x + collision.transform.position.y;
                    if (position1 > position2) 
                    {
                        Destroy(this.gameObject);
                        Destroy(collision.gameObject);
                        Manager.gameManagerInstance.synthesize(fruitType, this.transform.position, collision.transform.position);
                        Manager.gameManagerInstance.totalScore += fruitScore;
                        Manager.gameManagerInstance.totalScoreText.text = "Score: " + Manager.gameManagerInstance.totalScore.ToString();
                    }
                }
            }
        }
    }
}
