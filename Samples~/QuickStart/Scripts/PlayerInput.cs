using System;
using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    public class PlayerInput : MonoBehaviour
    {
        public Action OnJumpPress;
        public float AxisHorizontal { get; private set; }
        public float AxisVertical { get; private set; }
        public float MouseAxisX { get; private set; }
        public float MouseAxisY { get; private set; }
        
        [SerializeField] private float mouseSensitivityX = 1;
        [SerializeField] private float mouseSensitivityY = 2;
        [SerializeField] private bool inputEnabled = true;
        
        private const string HORIZONTAL_AXIS = "Horizontal";
        private const string VERTICAL_AXIS = "Vertical";
        private const string MOUSE_AXIS_X = "Mouse X";
        private const string MOUSE_AXIS_Y = "Mouse Y";
        private const string JUMP_BUTTON = "Jump";

        
        void Update()
        {
            if (!inputEnabled) return;
            
            AxisHorizontal = Input.GetAxis(HORIZONTAL_AXIS);
            AxisVertical = Input.GetAxis(VERTICAL_AXIS);
            MouseAxisX = Input.GetAxis(MOUSE_AXIS_X) * mouseSensitivityX;
            MouseAxisY = Input.GetAxis(MOUSE_AXIS_Y) * mouseSensitivityY;
            if (Input.GetButtonDown(JUMP_BUTTON))
            {
                OnJumpPress?.Invoke();
            }
        }

        public void SetInputEnabled(bool enable)
        {
            inputEnabled = enable;
        }
    }
}
