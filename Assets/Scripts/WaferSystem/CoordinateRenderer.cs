using UnityEngine;

public class CoordinateRenderer : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D col;
    
    public ChunkCoordinate CurrentCoordinate { get; set; }

    public void Initialize(ChunkCoordinate coordinate, Sprite sprite)
    {
        CurrentCoordinate = coordinate;
        spriteRenderer.sprite = sprite;
        transform.position = coordinate.startingWorldSpacePos;
        col.size = spriteRenderer.sprite.bounds.size;
    }
}
