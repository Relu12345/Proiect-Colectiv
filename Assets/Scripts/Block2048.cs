using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block2048 : MonoBehaviour
{
    public int Value;
    public Node2048 Node;
    public Block2048 MergingBlock;
    public bool Merging;

    public Vector2 Pos => transform.position;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;
    
    public void Init(BlockType type)
    {
        Value = type.Value;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();
    }

    public void SetBlock(Node2048 node)
    {
        if (Node != null)
            Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock(Block2048 BlockToMergeWith)
    {
        MergingBlock = BlockToMergeWith;

        Node.OccupiedBlock = null;

        BlockToMergeWith.Merging = true;
    }

    public bool CanMerge(int value) => value == Value && !Merging && MergingBlock == null;
}
