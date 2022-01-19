using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SnakeBehaviour
{
    //Worked on by Ethan Sadowski, Jimun Jang, and Bryan Xing
    public class Snake : MonoBehaviour
    {

        public GameObject snakeHeadPrefab;
        public GameObject snakeBodyPrefab;
        private Guid id;
        private GameObject snakeHead;
        private List<GameObject> snakeBody;
        private string direction = "up";
        private Vector2 previousHeadLocation;
        private bool canTurn;
        private bool ate;

        void Start()
        {
            canTurn = true;
        }

        public string getDirection()
        {
            return this.direction;
        }

        public void setId(Guid id)
        {
            this.id = id;
        }

        public List<Vector2> getBodyCoordinateList()
        {
            List<Vector2> bodyList = new List<Vector2>();
            foreach (GameObject bodyPart in this.snakeBody)
            {
                bodyList.Add(bodyPart.GetComponent<Transform>().position);
            }
            return bodyList;
        }

        public Guid getId()
        {
            return this.id;
        }

        public void createBody(Vector2 startingCoordinate)
        {
            this.snakeBody = new List<GameObject>();
            GameObject newBodyPiece;
            for (int i = 0; i < 8; i++)
            {
                newBodyPiece = Instantiate(snakeBodyPrefab) as GameObject; 

                // add physics to body
                newBodyPiece.AddComponent<BoxCollider2D>();
                newBodyPiece.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
                newBodyPiece.SetActive(true);
                newBodyPiece.GetComponent<Transform>().position = new Vector2(startingCoordinate.x, startingCoordinate.y - ((1 * i) + 1));
                this.snakeBody.Add(newBodyPiece);
            }
        }

        public void updatePreviousHeadLocation(Vector2 previous)
        {
            this.previousHeadLocation = previous;
        }
        
        public void moveBody()
        {
            Vector2 bodyPlaceHolder;
            
            bodyPlaceHolder = this.snakeBody[0].GetComponent<Transform>().position;
            this.snakeBody[0].GetComponent<Transform>().position = this.previousHeadLocation;
            for (int i = 1; i < this.snakeBody.Count; i++)
            {
                if (this.snakeBody[i] != null)
                {
                    Vector2 tempLocation = this.snakeBody[i].GetComponent<Transform>().position;
                    this.snakeBody[i].GetComponent<Transform>().position = bodyPlaceHolder;
                    bodyPlaceHolder = tempLocation;
                }
            }
        }

        public void enableTurning()
        {
            this.canTurn = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) && this.direction != "down" && canTurn)
            {
                this.direction = "up";
                canTurn = false;
            }
            if (Input.GetKeyDown(KeyCode.S) && this.direction != "up" && canTurn)
            {
                this.direction = "down";
                canTurn = false;
            }
            if (Input.GetKeyDown(KeyCode.A) && this.direction != "right" && canTurn)
            {
                this.direction = "left";
                canTurn = false;
            }
            if (Input.GetKeyDown(KeyCode.D) && this.direction != "left" && canTurn)
            {
                this.direction = "right";
                canTurn = false;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Food")
            {
                // Eats the food
                Destroy(collision.gameObject);

                // Adds a new section to the snake body
                GameObject newBodyPiece;

                newBodyPiece = Instantiate(this.snakeBodyPrefab) as GameObject;
                newBodyPiece.AddComponent<BoxCollider2D>();
                newBodyPiece.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
                newBodyPiece.SetActive(true);
                this.snakeBody.Add(newBodyPiece);
            }
            else
            {
                Debug.Log("impact");
                // Snake body set to 1
                for (int i = 1; i < this.snakeBody.Count; i++)
                {
                    Destroy (snakeBody[i]);
                }
                this.snakeBody.RemoveRange(1, this.snakeBody.Count - 1);
                enableTurning();
            }
        }
    }
}

