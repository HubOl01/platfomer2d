using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public UnitStats unitStats;
    public AudioSource audioSource;
    public AudioClip runClip, meditationClip, painClip;
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
        // currentHealth = maxHealth;
        currentHealth = unitStats.maxHealth;
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
        timerText.text = $"{Mathf.Max(0, Mathf.CeilToInt(timer))}";

        if (timer <= 0)
        {
            Debug.Log("Time is up!");
            EndGame(2);
        }
    }

    void GameRun()
    {
        if (state == State.Meditation) return;

        float moveInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = new Vector2(moveInput * (unitStats.movementSpeed/*moveSpeed*/ + 10), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(moveInput * unitStats.movementSpeed/*moveSpeed*/, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, unitStats.jumpForce/*jumpForce*/);
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
            // Проверяем только по X-координате
            if (transform.position.x <= finalPositionObject.transform.position.x)
            {
                state = State.Idle;
                Debug.Log("You reached the final position!");
                EndGame(0); // Переход к следующей сцене
            }
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

        state = State.Idle;
        if (currentHealth <= 0)
        {
            EndGame(1);
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
    private bool isGameOver = false;

    void EndGame(int index)
    {
        if (isGameOver) return;

        isGameOver = true; // Устанавливаем флаг, чтобы избежать повторного вызова
        gameOver = true; // Останавливаем игру
        Debug.Log("Game Over!");

        // Сбрасываем состояние
        state = State.Idle;
        StopSound();

        // Добавляем задержку перед загрузкой сцены для предотвращения конфликтов
        StartCoroutine(LoadMainMenuAfterDelay(1f, index));
    }

    IEnumerator LoadMainMenuAfterDelay(float delay, int index)
    {
        yield return new WaitForSeconds(delay); // Ждём завершения текущих процессов
        Resources.UnloadUnusedAssets();
        if (index == 1)
        {
            SceneManager.LoadScene("GameOverHealth");
        }
        else if (index == 2)
        {
            SceneManager.LoadScene("GameOverTimer");
        }
        else
        {
            SceneManager.LoadScene("Winner");
        }
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

}
