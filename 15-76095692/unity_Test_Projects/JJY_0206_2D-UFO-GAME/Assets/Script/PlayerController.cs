﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed;
    private int count;
    private Rigidbody2D rb2d;

    public Text countText;
    public Text winText;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        count = 0;
        SetCountText();
        winText.text = "";
    }
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb2d.AddForce(movement*speed);

    }
   void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag ("Pickup"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();

        }
    }

    void SetCountText()
    {
        countText.text = "Count = " + count.ToString();
        if (count >= 12)
        {
            winText.text = "YOU WIN";
        }
    }
}