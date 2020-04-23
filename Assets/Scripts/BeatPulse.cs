﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatPulse : MonoBehaviour
{

    public float BPM = 100f;
    public GameObject ray;

    // Update is called once per frame

    void Update()
    {
        var baseValue = Mathf.Cos(Time.time * Mathf.PI * (BPM / 60f) % Mathf.PI);
        ray.transform.Rotate(0, 0, 30 * Time.deltaTime);
        this.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1), new Vector3(1.04f,1.04f,1), baseValue);
        ray.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1), new Vector3(1.08f, 1.08f, 1), baseValue);
    }

}