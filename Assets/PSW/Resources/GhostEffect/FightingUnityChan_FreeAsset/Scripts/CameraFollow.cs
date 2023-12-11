using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class CameraFollow : MonoBehaviour
    {

        [Header("Focus Target")]
        public Transform role;

        [Header("Camera Offset")]
        public Vector3 offset = new Vector3(0f, 10.1f, -13.1f);

        [Header("Camera Rotation")]
        public Vector3 rotation = new Vector3(35f, 0f, 0f);

        void LateUpdate()
        {
            if (this.enabled)
            {
                UpdateCamera();
            }
        }


        public void UpdateCamera()
        {
            if (role == null)
            {
                return;
            }

            transform.position = role.position + offset;
            transform.rotation = Quaternion.Euler(rotation);
        }

       
    }
}

