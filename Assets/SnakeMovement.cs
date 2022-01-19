using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SnakeBehaviour;
using MulticastSend;
using System;


namespace SnakeMovementController
{
    //Worked on by Ethan Sadowski
    public class SnakeMovement : MonoBehaviour
    {
        Dictionary<string, GameObject> snakes;
        Dictionary<string, List<GameObject>> snakeBodies;
        Guid nativeSnakeId;

        // Start is called before the first frame update
        void Start()
        {
            snakeBodies = new Dictionary<string, List<GameObject>>();
            getSnakes();
        }

        // Update is called once per frame
        void Update()
        {
            moveNativeSnake();
        }

        public void addSnake(GameObject snake)
        {
            Debug.Log(snake);
            string id = snake.GetComponent<Snake>().getId().ToString();
            this.snakes.Add(id, snake);
            List<GameObject> newBody = new List<GameObject>();
            this.snakeBodies.Add(id, newBody);
        }

        public void removeSnakeParts(Guid snakeId, int count)
        {
            Debug.Log(count);
            
            for (int i = count; i > 0; i--)
            {
                List<GameObject> bodyToRemoveFrom = snakeBodies[snakeId.ToString()];
                int bodyLength = bodyToRemoveFrom.Count;
                GameObject removedPart = bodyToRemoveFrom[bodyLength - 1];
                Debug.Log(removedPart);
                bodyToRemoveFrom.RemoveAt(bodyLength - 1);
                Destroy(removedPart);

            }
        }

        public void addSnakePart(Guid snakeId, GameObject bodyPart)
        {
            List<GameObject> bodyList = this.snakeBodies[snakeId.ToString()];
            bodyList.Add(bodyPart);
        }

        public void setNativeSnakeId(Guid id)
        {
            this.nativeSnakeId = id;
        }

        void getSnakes()
        {
            snakes = new Dictionary<string, GameObject>();
            Scene scene = SceneManager.GetSceneByName("SampleScene");
            GameObject[] sceneObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in sceneObjects)
            {
                if (obj.GetComponent("SnakeBehaviour.Snake") != null)
                {
                    string id = obj.GetComponent<Snake>().getId().ToString();
                    snakes.Add(id, obj);
                }
            }
        }

        public bool checkIfSnakeExists(Guid id)
        {
            return this.snakes.ContainsKey(id.ToString());
        }

        public int getBodySizeById(Guid id)
        {
            return this.snakeBodies[id.ToString()].Count;
        }

        public GameObject getSnakeById(Guid id)
        {
            Scene scene = SceneManager.GetSceneByName("SampleScene");
            GameObject[] sceneObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in sceneObjects)
            {
                if (obj.GetComponent("SnakeBehaviour.Snake") != null)
                {
                    if (obj.GetComponent<Snake>().getId() == id)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }



        public void updateSnakeLocation(Guid id, List<Vector2> snakeLocations)
        {
            GameObject snakeObj = getSnakeById(id);
            Transform snakeTransform = snakeObj.GetComponent<Transform>();
            snakeTransform.position = snakeLocations[0];
            List<GameObject> body = snakeBodies[id.ToString()];
            for (int i = 0; i < snakeLocations.Count; i++)
            {
                body[i].GetComponent<Transform>().position = snakeLocations[i];
            }
        }

        void moveNativeSnake()
        {
            GameObject snakeObj = getSnakeById(nativeSnakeId);
            Snake snake = snakeObj.GetComponent<Snake>();
            string direction = snake.getDirection();
            Transform snakeTransform = snakeObj.GetComponent<Transform>();
            float xPosition = snakeTransform.position.x;
            float yPosition = snakeTransform.position.y;
            snake.updatePreviousHeadLocation(new Vector2(xPosition, yPosition));
            if (direction == "up")
            {
                snakeTransform.position = calculateNextLocation(new Vector2(xPosition, yPosition + 1));
            }

            if (direction == "down")
            {
                snakeTransform.position = calculateNextLocation(new Vector2(xPosition, yPosition - 1));
            }

            if (direction == "left")
            {
                snakeTransform.position = calculateNextLocation(new Vector2(xPosition - 1, yPosition));
            }

            if (direction == "right")
            {
                snakeTransform.position = calculateNextLocation(new Vector2(xPosition + 1, yPosition));
            }
            snake.moveBody();
            snake.enableTurning();
        }

        Vector2 calculateNextLocation(Vector2 vectorInput)
        {
            if (vectorInput.x > 44)
            {
                return new Vector2(-44, vectorInput.y);
            }

            if (vectorInput.x < -44)
            {
                return new Vector2(44, vectorInput.y);
            }

            if (vectorInput.y < -24.5)
            {
                return new Vector2(vectorInput.x, (float)24.5);
            }

            if (vectorInput.y > 24.5)
            {
                return new Vector2(vectorInput.x, (float)-24.5);
            }

            return vectorInput;

        }
    }
}

