using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SandMovement : MonoBehaviour
{
    public float speed;
    public float turningSpeed;
    public float particleLength = 6;
    public float particleDensity = 15;

    public ParticleSystem mainTrail;
    public ParticleSystem flakeTrail;

    Vector2 direction;

    Vector2 inputDir;

    PlayerControls controls;
    Rigidbody2D rb;
    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.MousePos.performed += ctx => CheckInput(ctx.ReadValue<Vector2>());
        controls.Player.MousePos.started += ctx => CheckInput(ctx.ReadValue<Vector2>());
        controls.Player.MousePos.canceled += ctx => CheckInput(ctx.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.MousePos.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Player.MousePos.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = Vector2.right;

        mainTrail.startLifetime = particleLength / speed;
        flakeTrail.startLifetime = particleLength / speed;

        mainTrail.emissionRate = speed * particleDensity;
        flakeTrail.emissionRate = (int)(speed * particleDensity * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        inputDir = Camera.main.ScreenToWorldPoint(mosPos) - transform.position;
    }

    Vector2 mosPos;
    void CheckInput(Vector2 pos)
    {
        mosPos = pos;
        inputDir = Camera.main.ScreenToWorldPoint(mosPos)-transform.position;
    }

    private void FixedUpdate()
    {
        if (inputDir.sqrMagnitude > 0.1f)
        {
            float angleDiff = Vector2.SignedAngle(direction, inputDir);
            float turning = turningSpeed * Time.fixedDeltaTime;
            if (Mathf.Abs(turning) > Mathf.Abs(angleDiff)) turning = angleDiff;
            else if (angleDiff < 0) turning *= -1;

            direction = Quaternion.Euler(0, 0, turning) * direction;
        }

        rb.velocity = direction.normalized * speed;
        //Vector2 vel = direction.normalized * speed * Time.fixedDeltaTime;
        //transform.position += new Vector3(vel.x, vel.y, transform.position.z);
    }
}
