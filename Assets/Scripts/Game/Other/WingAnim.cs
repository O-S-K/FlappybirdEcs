using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyECS
{
    public class WingAnim : MonoBehaviour
    {
        void Update() => transform.localEulerAngles = new Vector3(0,  35 * Mathf.Sin(Time.time * 10),0);
    }
}