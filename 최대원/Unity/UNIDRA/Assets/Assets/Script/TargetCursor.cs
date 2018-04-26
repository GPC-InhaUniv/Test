﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCursor : MonoBehaviour
{
    public float radius = 1f;
    public float angularVelocity = 480f;
    public Vector3 destination = new Vector3(0f, 0.5f, 0f);
    Vector3 position = new Vector3(0f, 0.5f, 0f);
    float angle = 0f;

    public void SetPosiotion(Vector3 iPosition)
    {
        destination = iPosition;
        destination.y = 0.5f;
    }

    private void Start()
    {
        SetPosiotion(transform.position);
        position = destination;
    }

    private void Update()
    {
        position += (destination - position) / 10f;
        angle += angularVelocity * Time.deltaTime;
        Vector3 offset = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, radius);
        transform.localPosition = position + offset;
    }

}
