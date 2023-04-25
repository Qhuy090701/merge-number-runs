using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMovement : MonoBehaviour
{
    [SerializeField] private float speedTouch = 5f;
    [SerializeField] private float swipeThreshold = 20f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    void Start()
    {
        
    }
    private void Update()
    {
        TouchMove(); 
    }


    private void TouchMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            float distance = Vector3.Distance(startPosition, endPosition);

            if (distance > swipeThreshold)
            {
                Vector3 direction = endPosition - startPosition;

                if (direction.x > 0)
                {
                    isMovingRight = true;
                    isMovingLeft = false;
                }
                else
                {
                    isMovingLeft = true;
                    isMovingRight = false;
                }
            }
            else
            {
                isMovingLeft = false;
                isMovingRight = false;
            }
        }

        if (isMovingLeft)
        {
            transform.position += Vector3.left * speedTouch * Time.deltaTime;
            Debug.Log("isMovingLeft");
        }
        else if (isMovingRight)
        {
            transform.position += Vector3.right * speedTouch * Time.deltaTime;
            Debug.Log("isMovingRight");
        }
    }
}
