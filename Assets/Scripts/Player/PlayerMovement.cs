using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float moveSpeed;
    
    [Header("Player Boundaries")]
    [SerializeField] private float horizontalScreenMargin = 0.03f;
    [SerializeField] private float allowedVerticalFraction = 0.33f;

    private Vector2 _minBounds, _maxBounds;
    private Camera _cam;
    private InputSystem_Actions _inputActions;
    private Vector2 _movementInput;
    private Rigidbody2D _playerRigidbody;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
        }
    }

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _movementInput = Vector2.zero;
        CacheBounds();
    }

    private void Update()
    {
        _movementInput = _inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }
    
    private void FixedUpdate()
    {
        if (_movementInput != Vector2.zero)
        {
            Vector2 newPosition = _playerRigidbody.position + _movementInput * (moveSpeed * Time.fixedDeltaTime);

            // Clamp the new position within the defined bounds
            newPosition.x = Mathf.Clamp(newPosition.x, _minBounds.x, _maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, _minBounds.y, _maxBounds.y);

            _playerRigidbody.MovePosition(newPosition);
        }
    }

    private void CacheBounds()
    {
        // Calculate the screen bounds based on the camera's viewport
        var camToPlayerZ = transform.position.z - _cam.transform.position.z;
        var bottomLeft = _cam.ViewportToWorldPoint(new Vector3(0, 0, camToPlayerZ));
        var topRight = _cam.ViewportToWorldPoint(new Vector3(1, 1, camToPlayerZ));
        var fullWidth = topRight.x - bottomLeft.x;
        var fullHeight = topRight.y - bottomLeft.y;
        
        // Consider the horizontal and vertical margins
        var horizontalWidthWithMargin = fullWidth * horizontalScreenMargin;
        _minBounds.x = bottomLeft.x + horizontalWidthWithMargin;
        _maxBounds.x = topRight.x - horizontalWidthWithMargin;
        _minBounds.y = bottomLeft.y;
        _maxBounds.y = bottomLeft.y + fullHeight * allowedVerticalFraction;
    }
}
