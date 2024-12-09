// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// public class PlayerController : MonoBehaviour
// {
//     public AudioSource audioSource;   // Источник звука
//     public AudioClip runClip;         // Звук бега
//     public AudioClip meditationClip;         // Звук бега
//     public AudioClip painClip;         // Звук боли
//     public AudioClip gameOverClip;         // Звук боли
//     public Vector3 startPosition;
//     public float moveSpeed = 5f;
//     public float jumpForce = 8f;
//     public Rigidbody2D rb;
//     public TextMeshProUGUI healthText; // Текст для отображения здоровья
//     public int maxHealth = 3;

//     private Animator animator;
//     public int score = 0;
//     private int currentHealth;
//     public TextMeshProUGUI scoreNumText;
//     #region FiniteStateMachine
//     private enum State { Idle, Run, Meditation }
//     private State state = State.Idle;
//     #endregion
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         startPosition = transform.position;
//         currentHealth = maxHealth;          // Устанавливаем полное здоровье
//         UpdateHealthUI();
//     }

//     void Update()
//     {
//         if (Time.deltaTime != 0)
//         {
//             if (transform.position.y <= -15)
//             {
//                 Respawn();
//             }
//             else
//             {
//                 GameRun();
//                 animator.SetInteger("state", (int)state);
//             }
//         }
//     }

//     void GameRun()
//     {
//         if (state == State.Meditation)
//             return;

//         float moveInput = Input.GetAxis("Horizontal");
//         scoreNumText.text = $"{moveInput}";

//         if (Input.GetKey(KeyCode.LeftShift))
//         {
//             rb.velocity = new Vector2(moveInput * (moveSpeed + 10), rb.velocity.y);
//         }
//         else
//         {
//             rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
//         }

//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             rb.velocity = new Vector2(rb.velocity.x, jumpForce);
//             state = State.Idle;
//             StopSound();
//         }
//         else if (moveInput > 0)
//         {
//             transform.localScale = new Vector3(1, 1, 1);
//             state = State.Run;
//             PlayRunSound();
//         }
//         else if (moveInput < 0)
//         {
//             transform.localScale = new Vector3(-1, 1, 1);
//             state = State.Run;
//             PlayRunSound();
//         }
//         else
//         {
//             state = State.Idle;
//             StopSound();
//         }

//         if (Input.GetKeyDown(KeyCode.M))
//         {
//             Debug.Log("Meditation started");
//             StopSound();
//             StartCoroutine(meditation());
//         }
//     }


//     IEnumerator meditation()
//     {
//         Debug.Log("Entering meditation");
//         state = State.Meditation;
//         PlayMeditationSound();
//         animator.SetInteger("state", (int)state);

//         yield return new WaitForSeconds(3);

//         Debug.Log("Meditation complete");
//         if (currentHealth < maxHealth)
//         {
//             currentHealth += 1; // Увеличиваем здоровье после завершения медитации
//             UpdateHealthUI();
//             Debug.Log($"Health increased to: {currentHealth}");
//         }
//         else
//         {
//             Debug.Log($"Health is already at maximum {currentHealth} == ${maxHealth}");
//         }

//         StopSound();
//         state = State.Idle;
//         animator.SetInteger("state", (int)state);
//     }

//     void PlayRunSound()
//     {
//         if (!audioSource.isPlaying) // Проверка, чтобы звук не накладывался
//         {
//             audioSource.clip = runClip;
//             audioSource.loop = true; // Для непрерывного звука бега
//             audioSource.Play();
//         }
//     }
//     void PlayGameOverSound()
//     {
//         if (!audioSource.isPlaying) // Проверка, чтобы звук не накладывался
//         {
//             audioSource.clip = gameOverClip;
//             audioSource.loop = true; // Для непрерывного звука бега
//             audioSource.Play();
//         }
//     }
//     void PlayMeditationSound()
//     {
//         if (!audioSource.isPlaying) // Проверка, чтобы звук не накладывался
//         {
//             audioSource.clip = meditationClip;
//             audioSource.loop = true; // Для непрерывного звука бега
//             audioSource.Play();
//         }
//     }
//     void PlayPainSound()
//     {
//         if (!audioSource.isPlaying) // Проверка, чтобы звук не накладывался
//         {
//             audioSource.clip = painClip;
//             audioSource.loop = true; // Для непрерывного звука бега
//             audioSource.Play();
//         }
//     }

//     void StopSound()
//     {
//         if (audioSource.isPlaying)
//         {
//             audioSource.Stop();
//         }
//     }
//     void Respawn()
//     {
//         // Возвращаем персонажа на стартовую позицию
//         transform.position = startPosition;
//         rb.velocity = Vector2.zero; // Сбрасываем скорость
//         state = State.Idle;
//         animator.SetInteger("state", (int)state);
//         TakeDamage(1); // Наказание за падение
//     }
//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         Debug.Log($"Collided with: {collision.gameObject.name}");

//         // Проверяем столкновение с ловушкой
//         if (collision.CompareTag("traps"))
//         {
//             StopSound();
//             Debug.Log("Hit a trap!");
//             TakeDamage(1);
//             PlayPainSound();
//             StartCoroutine(DelayedAction()); // Запускаем сопрограмму
//         }
//     }

//     IEnumerator DelayedAction()
//     {
//         yield return new WaitForSeconds(3); // Задержка в 1 секунду
//         StopSound();
//     }


//     void TakeDamage(int damage)
//     {
//         if (state == State.Idle)
//             return; // Игнорируем повторный урон во время анимации "ранен"

//         currentHealth -= damage;
//         UpdateHealthUI();

//         if (currentHealth <= 0)
//         {
//             GameOver();
//         }
//         else
//         {
//             StartCoroutine(HurtRoutine());
//         }
//     }

//     IEnumerator HurtRoutine()
//     {
//         // state = State.Hurt;
//         animator.SetInteger("state", (int)state);
//         yield return new WaitForSeconds(0.5f); // Пауза для "анимации получения урона"
//         state = State.Idle;
//         animator.SetInteger("state", (int)state);
//     }

//     void UpdateHealthUI()
//     {
//         Debug.Log($"Updating health UI: {currentHealth}");
//         healthText.text = $"{currentHealth}";
//     }

//     void GameOver()
//     {
//         // Логика для завершения игры
//         PlayGameOverSound();
//         Debug.Log("Game Over!");
//         Respawn();
//         currentHealth = maxHealth; // Сброс здоровья
//         UpdateHealthUI();
//         DelayedAction();
//     }

// }

using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip runClip, meditationClip, painClip, gameOverClip;
    public Vector3 startPosition;
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public Rigidbody2D rb;
    public TextMeshProUGUI healthText, scoreText, timerText;
    public int maxHealth = 3;
    public GameObject finalPositionObject; // Объект для финала

    private Animator animator;
    private int currentHealth;
    private int jumpCount = 0;
    private const int maxJumps = 2; // Лимит прыжков
    private int score = 0;
    private float timer = 60f; // Таймер на 60 секунд
    private bool gameOver = false;

    private enum State { Idle, Run, Meditation }
    private State state = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateScoreUI();
    }

    void Update()
    {
        if (gameOver) return;

        UpdateTimer();

        if (transform.position.y <= -15)
        {
            Respawn();
        }
        else
        {
            GameRun();
            animator.SetInteger("state", (int)state);
        }
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;
        timerText.text = $"{Mathf.Max(0, Mathf.CeilToInt(timer))} сек.";

        if (timer <= 0)
        {
            Debug.Log("Time is up!");
            EndGame();
        }
    }

    void GameRun()
    {
        if (state == State.Meditation) return;

        float moveInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = new Vector2(moveInput * (moveSpeed + 10), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            state = State.Run;
            PlayRunSound();
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            state = State.Run;
            PlayRunSound();
        }
        else
        {
            state = State.Idle;
            StopSound();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Meditation started");
            StopSound();
            StartCoroutine(meditation());
        }

        if (rb.velocity.y == 0) jumpCount = 0; // Сброс прыжков при приземлении
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("traps"))
        {
            PlayPainSound();
            TakeDamage(1);
            StartCoroutine(DelayedAction());
        }
        else if (collision.CompareTag("cryst"))
        {
            score += 10;
            UpdateScoreUI();
            Destroy(collision.gameObject); // Уничтожить кристалл
        }
        else if (collision.gameObject == finalPositionObject)
        {
            Debug.Log("You reached the final position!");
            EndGame();
        }
    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(3); // Задержка в 1 секунду
        StopSound();
    }
    IEnumerator meditation()
    {
        Debug.Log("Entering meditation");
        state = State.Meditation;
        PlayMeditationSound();
        animator.SetInteger("state", (int)state);

        yield return new WaitForSeconds(3);

        Debug.Log("Meditation complete");
        if (currentHealth < maxHealth)
        {
            currentHealth += 1; // Увеличиваем здоровье после завершения медитации
            UpdateHealthUI();
            Debug.Log($"Health increased to: {currentHealth}");
        }
        else
        {
            Debug.Log($"Health is already at maximum {currentHealth} == ${maxHealth}");
        }

        StopSound();
        state = State.Idle;
        animator.SetInteger("state", (int)state);
    }
    void Respawn()
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        state = State.Idle;
        StopSound();
        TakeDamage(1);
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            EndGame();
        }
    }

    void UpdateHealthUI()
    {
        healthText.text = $"{currentHealth}";
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"{score}";
    }

    void EndGame()
    {
        gameOver = true;
        Debug.Log("Game Over!");
        state = State.Idle;
        StopSound();
        PlayGameOverSound();
        Respawn();
    }

    void PlayRunSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = runClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void PlayMeditationSound()
    {
        if (!audioSource.isPlaying) // Проверка, чтобы звук не накладывался
        {
            audioSource.clip = meditationClip;
            audioSource.loop = true; // Для непрерывного звука бега
            audioSource.Play();
        }
    }
    void PlayPainSound()
    {
        if (!audioSource.isPlaying) // Проверка, чтобы звук не накладывался
        {
            audioSource.clip = painClip;
            audioSource.loop = true; // Для непрерывного звука бега
            audioSource.Play();
        }
    }
    void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void PlayGameOverSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = gameOverClip;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
}
