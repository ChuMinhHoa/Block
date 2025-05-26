using System;
using UnityEngine;

namespace _BaseGame.Script.Unit
{
    public class TestController : MonoBehaviour
    {
        public CharacterController controller;
        public float playerSpeed = 2.0f;
        private Vector3 playerVelocity;
        private Camera mainCamera;
        private Vector3 lastPoint;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                var vectorMouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var dir = vectorMouse - lastPoint;
                var vector3 = new Vector3(dir.x, 0, dir.z);
                Move(vector3);
                lastPoint = vectorMouse;
            }
          
        }

        public void Move(Vector3 vectorMove)
        {
            // Combine horizontal and vertical movement
            Vector3 finalMove = (vectorMove * playerSpeed);
            controller.SimpleMove(finalMove);
           // controller.SimpleMove(finalMove * Time.deltaTime);
            transform.eulerAngles = Vector3.zero;
        }
    }
}
