using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 玩家的移动速度
    public float jumpForce = 10f; // 跳跃的力度
    [HideInInspector]
    public bool isJumping = false; // 检查玩家是否正在跳跃

    private Rigidbody2D rb; // Rigidbody2D组件的引用
    private TMP_Text myText;

    public AudioSource coinSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D组件
        myText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (myText.text == "")
        {
            rb.gravityScale = 0;
            return;
        }
        else
        {
            rb.gravityScale = 300;
        }
        // Rotate();
        // 水平移动
        float move = Input.GetAxis("Horizontal"); // 获取水平轴输入
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y); // 设置水平速度

        // 跳跃
        if (Input.GetButtonDown("Jump") && !isJumping) // 检查是否按下跳跃键且不在跳跃状态
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse); // 以冲击方式添加垂直力
            isJumping = true; // 设置跳跃状态为true
        }
    }

    // 检测与地面的碰撞
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("Ground") || collision.collider.tag.Equals("Wall")) // 如果碰撞的对象标签为"Ground"
        {
            isJumping = false; // 设置跳跃状态为false
        }

        if (collision.collider.tag.Equals("Coin"))
        {
            print("get coin");
            coinSound.Play();
            Destroy(collision.collider);
            StartCoroutine(WaitForAudioThenLoadScene());
        }

        if (collision.collider.tag.Equals("Finish"))
            GameManager.Instance.RestartGame();
    }

    private IEnumerator WaitForAudioThenLoadScene()
    {
        // Wait until the audio source is finished playing
        yield return new WaitForSeconds(1);

        // Load the desired scene
        GameManager.Instance.ProceedToNextLevel();
    }

    // public float moveSpeed = 5.0f; // Player's movement speed
    //
    // private Rigidbody2D rb; // Reference to the Rigidbody2D component
    // private Vector2 moveInput; // Stores input from the arrow keys
    // private Vector2 moveVelocity; // The velocity we will set for the player
    //
    // private TMP_Text myText;
    //
    // void Start()
    // {
    //     rb = GetComponent<Rigidbody2D>(); // Getting the Rigidbody2D component
    //     myText = GetComponent<TMP_Text>();
    // }
    //
    // void Update()
    // {
    //     if (myText.text == "") return;
    //
    //     Move();
    //     // Rotate();
    // }
    //
    // void FixedUpdate()
    // {
    //     // Setting the player's velocity
    //     rb.velocity = moveVelocity;
    // }
    //
    // private void Move()
    // {
    //     // Getting input from arrow keys
    //     moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //     // Normalizing the input vector and multiplying it by the speed
    //     moveVelocity = moveInput.normalized * moveSpeed;
    // }

    // private void Rotate()
    // {
    //     if (Input.GetKeyDown(KeyCode.RightShift))
    //     {
    //         transform.Rotate(Vector3.forward, 90);
    //     }
    // }
}
