public class BoardMovement
{
    // [SerializeField] private float speed, horizontalLimit;
    // [Tooltip("Position Z = Min(position.z - _verticalLimit), Max(position.z + 2)")]
    // [SerializeField] private float _verticalLimit;
    // private Vector3 _clickPos;
    // private float _horizontal, _vertical, _newX, _newZ, positionZ;
    // private void Awake()
    // {
    //     if (GameManager.gameModeOption == GameManager.GameMode.Defence)
    //         this.enabled = false;
    //     positionZ = transform.position.z;
    // }
    // private void Update()
    // {
    //     Movement();
    // }
    // private void Movement()
    // {
    //     if (!GameManager.instance.isOnStarted) return;
    //     if (!(GameManager.gameModeOption == GameManager.GameMode.Runner)) return;
    //     if (GameManager.instance.gameOver) return;
    //     HorizontalMovement();
    //     VerticalMovement();
    // }
    // private void HorizontalMovement()
    // {
    //     _horizontal = Input.GetMouseButton(0) ? Input.GetAxisRaw("Mouse X") : 0;

    //     _newX = transform.position.x + _horizontal * speed * Time.deltaTime;
    //     _newX = Mathf.Clamp(_newX, -horizontalLimit, horizontalLimit);
    //     transform.position = new Vector3(_newX, transform.position.y, transform.position.z);
    // }
    // private void VerticalMovement()
    // {
    //     _vertical = Input.GetMouseButton(0) ? Input.GetAxisRaw("Mouse Y") : 0;

    //     _newZ = transform.position.z + _vertical * speed * Time.deltaTime;
    //     _newZ = Mathf.Clamp(_newZ, (positionZ - _verticalLimit), (positionZ + 2));
    //     transform.position = new Vector3(transform.position.x, transform.position.y, _newZ);
    // }
}