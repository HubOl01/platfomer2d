using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public Rigidbody2D rb;

    private Animator animator;
    public int score = 0;
    public TextMeshProUGUI scoreNumText;
    #region FiniteStateMachine
    private enum State { Idle, Run, Meditation }
    private State state = State.Idle;
    #endregion
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.deltaTime != 0)
        {
            GameRun();
            animator.SetInteger("state", (int)state);
        }
    }

    void GameRun()
    {
        if (state == State.Meditation)
            return;

        float moveInput = Input.GetAxis("Horizontal");

        scoreNumText.text = $"{moveInput}";
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = new Vector2(moveInput * (moveSpeed + 10), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.Idle;
        }
        else if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            state = State.Run;
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            state = State.Run;

        }
        else
        {
            state = State.Idle;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(meditation());
        }
    }

    IEnumerator meditation()
    {
        state = State.Meditation;
        animator.SetInteger("state", (int)state);
        yield return new WaitForSeconds(3);
        state = State.Idle;
        animator.SetInteger("state", (int)state);
    }
}