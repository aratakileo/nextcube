using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float rotateSpeed = 0.8f;
    public float gravity = 20.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private CharacterController _controller;
    private Camera _camera;

    public GameWorld gameWorld;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Player movement
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed, 0);
        _camera.transform.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed, 0, 0);
        
        if (_camera.transform.localRotation.eulerAngles.y != 0)
            _camera.transform.Rotate(Input.GetAxis("Mouse Y") * rotateSpeed, 0, 0);

        _moveDirection = new Vector3(
            Input.GetAxis("Horizontal") * speed, _moveDirection.y,
            Input.GetAxis("Vertical") * speed
            );
        _moveDirection = transform.TransformDirection(_moveDirection);

        if (_controller.isGrounded)
        {
            if (Input.GetButton("Jump"))
                _moveDirection.y = jumpSpeed;
            else _moveDirection.y = 0;
        }

        _moveDirection.y -= gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
        
        // Block placing and destroing
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return;
        
        var ray = _camera.ViewportPointToRay(new Vector3(.5f, .5f));

        if (!Physics.Raycast(ray, out var hitInfo, 8 * ChunkRenderer.BlockScale)) return;
        
        var blockCenter = hitInfo.point + hitInfo.normal * ((Input.GetMouseButtonDown(0) ? -1 : 1) * ChunkRenderer.BlockScale) / 2;
        var blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale);
        var chunk2DPos = gameWorld.GetChunkPositionContainingBlock(blockWorldPos);
        
        if (!gameWorld.chunksData.TryGetValue(chunk2DPos, out var chunkData)) return;

        var chunkPos = blockWorldPos - new Vector3Int(chunk2DPos.x, 0, chunk2DPos.y) * ChunkRenderer.ChunkWidth;

        if (Input.GetMouseButtonDown(0))
            chunkData.renderer.DestroyBlock(chunkPos);
        else
            chunkData.renderer.SetBlock(chunkPos, BlockType.Grass);
    }
}
