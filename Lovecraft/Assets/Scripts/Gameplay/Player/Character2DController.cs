using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum AnimationFacingDirections
{
    DOWN_LEFT,
    UP_LEFT,
    DOWN_RIGHT,
    UP_RIGHT,
}

public class Character2DController : MonoBehaviour
{
    public Material DisplayMaterial;
    public int animationColumn;
    public AnimationFacingDirections facingDir;
    public AnimationFacingDirections prevFacingDir;
    public Transform Player;

    public float animationSpeed = 0.25f;
    private float animationBucket = 0f;
    private float[] animationOffsets;
    private float[] animationFacingOffsets;
    private int animationIndex = 0;
    private Transform PC;

    // Start is called before the first frame update
    void Start()
    {
        animationOffsets = new float[3] { 0, 0.33f, 0.66f };
        animationFacingOffsets = new float[4] { 0.5f, 0, 0.5f, 0 };
    }

    // Update is called once per frame
    void Update()
    {
        RotateToCursorPositon();
        if ( facingDir != prevFacingDir )
        {
            switch(facingDir)
            {
                case AnimationFacingDirections.DOWN_LEFT:
                case AnimationFacingDirections.UP_LEFT:
                    if( transform.localScale.x < 0)
                    {
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }
                    break;
                case AnimationFacingDirections.DOWN_RIGHT:
                case AnimationFacingDirections.UP_RIGHT:
                    if (transform.localScale.x > 0)
                    {
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }
                    break;
            }
        }

        animationBucket += Time.deltaTime;
        if( animationBucket >= animationSpeed)
        {
            animationBucket = 0;
            animationIndex++;
            if( animationIndex > 2) { animationIndex = 0; }
            DisplayMaterial.SetTextureOffset("_MainTex", new Vector2(animationOffsets[animationIndex], animationFacingOffsets[(int)facingDir]));
        }

        prevFacingDir = facingDir;
    }

    void RotateToCursorPositon()
    {
        float yRot = Player.localRotation.eulerAngles.y;

        if( yRot > 330 || yRot <= 60 )
        {
            // UP_RIGHT
            facingDir = AnimationFacingDirections.UP_RIGHT;
        }
        else if (yRot < 150 && yRot >= 60)
        {
            // DOWN_RIGHT
            facingDir = AnimationFacingDirections.DOWN_RIGHT;
        } else if(yRot < 240 && yRot >= 150)
        {
            // DOWN_LEFT
            facingDir = AnimationFacingDirections.DOWN_LEFT;
        } else if (yRot < 330 && yRot >= 240)
        {
            // UP_LEFT
            facingDir = AnimationFacingDirections.UP_LEFT;
        }
    }
}
