using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement Variables")]
    [SerializeField] private float playerSpeed = 6.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    [SerializeField] private float sprintDeadzone = 0.8f;

    [Header("Jump Variables")]
    [SerializeField] private float jumpHeight = 3.0f;
    [SerializeField] private float jumpTimeAllowance = 0.2f;
    [SerializeField] private float gravityValue = -9.81f;

    [Header("View Variables")]
    [SerializeField] private float xSensitivity = 2.0f;
    [SerializeField] private float ySensitivity = 2.0f;
    [SerializeField] private Vector2 yViewClamp;
    [SerializeField] private float viewDeadzone = 0.1f;

    [Header("Wall run")]
    [SerializeField] private float wallRunGravity = 4.9f;
    [SerializeField] private Raycaster[] wallRunChecks;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private AnimationCurve wallJumpCurve;

    private CharacterController controller;
    private PlayerController player;

    private Vector2 moveInput;
    private Vector2 viewInput;
    private float timeSinceJump;
    private float timeSinceGrounded;
    private float timeSinceOnWall;
    private float currentWallJumpTime;
    private bool sprint;
    private Vector3 playerVelocity;
    private Vector3 playerRotation;
    private Vector3 cameraRotation;
    private Vector3 wallJumpVelocity;

    private void Awake()
    {
        this.controller = GetComponent<CharacterController>();
        this.player = GetComponent<PlayerController>();

        this.playerRotation = this.transform.rotation.eulerAngles;
        this.cameraRotation = this.cameraTransform.rotation.eulerAngles;

        this.ResetJumpTime();
    }

    private void Start()
    {
        this.SetInputEvents();
    }

    private void SetInputEvents()
    {
        var input = player.Input;

        input.Movement.Move.performed += this.SetMovementInput;
        input.Movement.Move.canceled += this.SetMovementInput;
        input.Movement.View.performed += this.SetViewInput;

        input.Movement.Jump.started += this.SetJumpInput;
        input.Movement.Sprint.started += e => this.SetSprintInput();

        input.Movement.Pause.started += e => Pause.TogglePause();

        input.Enable();
    }

    private void SetMovementInput(CallbackContext e) => this.moveInput = e.ReadValue<Vector2>();
    private void SetJumpInput(CallbackContext e) => this.timeSinceJump = 0f;
    private void SetSprintInput() => this.sprint = !this.sprint;
    private void SetViewInput(CallbackContext e) => this.viewInput = e.ReadValue<Vector2>();

    private void Update()
    {
        this.UpdateTimeVariables();

        this.SetMovement();
        this.SetView();
    }

    private void UpdateTimeVariables()
    {
        TimedVariables.UpdateTimeVariable(ref timeSinceGrounded, jumpTimeAllowance);
        TimedVariables.UpdateTimeVariable(ref timeSinceJump, jumpTimeAllowance);
        TimedVariables.UpdateTimeVariable(ref timeSinceOnWall, jumpTimeAllowance);
        TimedVariables.UpdateTimeVariable(ref currentWallJumpTime, wallJumpTime);
    }

    private void SetMovement()
    {
        // Check if grounded
        var grounded = this.controller.isGrounded;
        if (grounded)
            this.timeSinceGrounded = 0f;

        // Zero vertical velocity if grounded
        if (grounded && this.playerVelocity.y < 0)
            this.playerVelocity.y = 0f;

        // Check if wall running
        var wallRunning = this.wallRunChecks.Any(c => c.HasHit);
        var wallNormal = Vector3.zero;
        if(wallRunning)
        {
            wallNormal = this.GetWallNormal();
            this.timeSinceOnWall = 0f;
        }

        if (this.moveInput.magnitude < this.sprintDeadzone)
            this.sprint = false;

        // Get basic speed from input
        var speed = this.sprint ? this.sprintSpeed : this.playerSpeed;
        var move = new Vector3(this.moveInput.x * speed, this.playerVelocity.y, this.moveInput.y * speed);
        var up = 0f;

        if (this.timeSinceJump <= this.jumpTimeAllowance)
        {
            // Add jump force if jumping
            if (this.timeSinceGrounded <= this.jumpTimeAllowance)
            {
                up = Mathf.Sqrt(Mathf.Abs(this.jumpHeight * this.gravityValue));
                this.ResetJumpTime();
            }
            // Set wall jump force if jumping
            else if (this.timeSinceOnWall <= this.jumpTimeAllowance)
            {
                this.wallJumpVelocity = wallNormal * this.wallJumpForce.x;
                up = Mathf.Sqrt(Mathf.Abs(this.wallJumpForce.y * this.gravityValue));

                wallRunning = false;
                this.currentWallJumpTime = 0f;
                this.ResetJumpTime();
            }
        }

        this.playerVelocity = this.transform.TransformVector(move);

        // Add wall jump force if jumping from wall
        if (this.currentWallJumpTime <= this.wallJumpTime)
            AddWallJumpVelocity();

        // Set vertical if jumping
        if (up > 0f)
            this.playerVelocity.y = up;

        // Add gravity
        var gravity = wallRunning ? this.wallRunGravity : this.gravityValue;
        this.playerVelocity.y += gravity * Time.deltaTime;

        // Move
        this.controller.Move(this.playerVelocity * Time.deltaTime);
    }

    private Vector3 GetWallNormal()
    {
        var result = this.wallRunChecks.AggregateNormals();
        Debug.DrawLine(this.transform.position, this.transform.position + result * 5, Color.red);
        return result;
    }

    private void AddWallJumpVelocity()
    {
        var planarVelocity = Vector3.ProjectOnPlane(this.playerVelocity, this.wallJumpVelocity);

        var normalVelocity = this.wallJumpVelocity * this.wallJumpCurve.Evaluate(this.currentWallJumpTime / this.wallJumpTime);
        var playerVelocity = Vector3.Project(this.playerVelocity, this.wallJumpVelocity);

        if (Vector3.Dot(this.wallJumpVelocity, playerVelocity) > 0 && playerVelocity.magnitude > normalVelocity.magnitude)
            normalVelocity = playerVelocity;

        this.playerVelocity = planarVelocity + normalVelocity;
    }

    private void ResetJumpTime()
    {
        this.timeSinceJump = this.jumpTimeAllowance + 1f;
        this.timeSinceOnWall = this.jumpTimeAllowance + 1f;
    }

    private void SetView()
    {
        if (this.viewInput.magnitude < this.viewDeadzone)
            this.viewInput = Vector2.zero;

        this.cameraRotation.z = this.cameraTransform.rotation.eulerAngles.z;

        this.cameraRotation.x += -this.ySensitivity * viewInput.y * Time.deltaTime;
        this.cameraRotation.x = Mathf.Clamp(this.cameraRotation.x, this.yViewClamp.x, this.yViewClamp.y);

        this.cameraTransform.localRotation = Quaternion.Euler(this.cameraRotation);

        this.playerRotation.y += this.xSensitivity * viewInput.x * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(this.playerRotation);
    }
}
