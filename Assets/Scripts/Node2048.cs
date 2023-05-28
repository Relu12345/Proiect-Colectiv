using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node2048 : MonoBehaviour
{
    public Vector2 Pos => transform.position;
    
    public Block2048 OccupiedBlock;
}
