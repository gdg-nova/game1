/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Input
{
    using Apex.Input;
    using UnityEngine;

    /// <summary>
    /// This is a trivial implementation of an input receiver. All input keys and buttons are hard coded.
    /// It is highly recommended that you implement your own input receiver to abstract input buttons and make use of the Unity Input Manager.
    /// </summary>
    [AddComponentMenu("Apex/Input/Very basic input receiver")]
    [InputReceiver]
    public class InputReceiverBasic : MonoBehaviour
    {
        private InputController _inputController;
        private SelectionRectangleComponent _selectRectangle;
        private Vector3 _lastSelectDownPos;
        private bool _isMac;

        private void Awake()
        {
            _inputController = new InputController();
            _selectRectangle = this.GetComponentInChildren<SelectionRectangleComponent>();

            if (_selectRectangle == null)
            {
                Debug.LogWarning("Missing SelectionRectangleComponent, this is required by the input receiver to handle unit selection.");
            }

            _isMac = (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXWebPlayer);
            if (Application.platform != RuntimePlatform.WindowsPlayer &&
                Application.platform != RuntimePlatform.WindowsEditor &&
                Application.platform != RuntimePlatform.WindowsWebPlayer &&
                !_isMac)
            {
                Debug.LogWarning("The default basic input receiver only works on Windows and Mac.");
            }
        }

        private void Update()
        {
            Movement();

            Selection();
        }

        private void Movement()
        {
            bool moveInput = false;
            if (_isMac)
            {
                moveInput = Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl);
            }
            else
            {
                moveInput = Input.GetMouseButtonUp(1);
            }

            if (moveInput)
            {
                var setWaypoint = Input.GetKey(KeyCode.LeftShift);

                _inputController.SetDestination(Input.mousePosition, setWaypoint);
            }
        }

        private void Selection()
        {
            if (_selectRectangle == null)
            {
                return;
            }

            var selectAppend = Input.GetKey(KeyCode.LeftShift);

            if (Input.GetMouseButtonDown(0))
            {
                _lastSelectDownPos = Input.mousePosition;
                _selectRectangle.StartSelect();
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (_selectRectangle.HasSelection(_lastSelectDownPos, Input.mousePosition))
                {
                    _inputController.SelectUnitRangeTentative(_lastSelectDownPos, Input.mousePosition, selectAppend);
                }

                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_selectRectangle.HasSelection(_lastSelectDownPos, Input.mousePosition))
                {
                    _inputController.SelectUnitRange(_lastSelectDownPos, Input.mousePosition, selectAppend);
                }
                else
                {
                    _inputController.SelectUnit(_lastSelectDownPos, selectAppend);
                }

                _selectRectangle.EndSelect();
                return;
            }

            var selectGroup = Input.GetKey(KeyCode.LeftShift);
            var assignGroup = Input.GetKey(KeyCode.LeftAlt);

            for (int index = 0; index < 5; index++)
            {
                var code = KeyCode.Alpha1 + index;
                if (Input.GetKeyUp(code))
                {
                    if (selectGroup)
                    {
                        _inputController.SelectGroup(index);
                    }
                    else if (assignGroup)
                    {
                        _inputController.AssignGroup(index);
                    }
                    else
                    {
                        _inputController.SelectUnit(index, false);
                    }
                }
            }
        }
    }
}
