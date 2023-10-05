using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    public int maxHealth = 3; // Vida máxima do jogador

    private bool isJumping;
    private bool doubleJump;
    private Rigidbody2D rig;
    private Animator anim;
    private int currentHealth;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            // Se a vida do jogador for menor ou igual a zero, encerre o jogo.
            GameController.instance.ShowGameOver();
            Destroy(gameObject);
        }

        Move();
        Jump();
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * Speed;

        if (Input.GetAxis("Horizontal") > 0f)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (Input.GetAxis("Horizontal") < 0f)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else
        {
            anim.SetBool("walk", false);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!isJumping)
            {
                rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                doubleJump = true;
                anim.SetBool("jump", true);
            }
            else
            {
                if (doubleJump)
                {
                    rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                    doubleJump = false;
                }
            }
        }
    }

    void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthUI();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isJumping = false;
            anim.SetBool("jump", false);
        }
        if (collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("Saw"))
        {
            TakeDamage(1); // 1 é o valor de dano ao jogador
        }
    }

    void UpdateHealthUI()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        string healthBar = "Health: ";
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < currentHealth)
            {
                healthBar += "#"; // Adiciona "#" para representar a vida atual
            }
            else
            {
                healthBar += "-"; // Adiciona "-" para representar a vida perdida
            }
        }
        Debug.Log(healthBar); // Imprime a barrinha de vida no console

        // Encontra o objeto de texto automaticamente se ele não estiver atribuído
        Text healthTextComponent = FindObjectOfType<Text>();
        if (healthTextComponent != null)
        {
            healthTextComponent.text = "Vidas: " + currentHealth.ToString();
        }
    }
}