using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "RandomRuleTile", menuName = "Tiles/Random Rule Tile")]
public class RandomRuleTile : RuleTile
{
    public Sprite[] randomSprites;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (randomSprites != null && randomSprites.Length > 0)
        {
            int index = Random.Range(0, randomSprites.Length);
            m_DefaultSprite = randomSprites[index];
        }

        return base.StartUp(position, tilemap, go);
    }
}
