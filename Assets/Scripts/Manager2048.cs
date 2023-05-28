using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Manager2048 : MonoBehaviour
{
    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node2048 _nodePrefab;
    [SerializeField] private Block2048 _blockPrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private int _winCondition = 2048;
    [SerializeField] private TextMeshPro _finalPointsText, _loseScreenPoints;
    [SerializeField] private GameObject gameOverUI, highscoreWindow;

    public static int _finalPoints = 0;
    public static uint selection = 0;

    private List<Node2048> _nodes;
    private List<Block2048> _blocks;
    private GameState _state;
    private int _round;
    private bool enableTimer = false;
    private float ElapsedTime = 5.0f;

    private BlockType GetBlockTypeByValue(int value) => _types.First(t => t.Value == value);

    void Start()
    {
        ChangeState(GameState.GenerateLevel);
        _finalPointsText.text = "Points: " + _finalPoints.ToString();
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;
        
        switch (newState)
        {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Lose:
                gameOverUI.SetActive(true);
                enableTimer = true;
                Camera.main.transform.position = new Vector3(15, 0, -10);
                _loseScreenPoints.text = _finalPointsText.text;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    void Update()
    {
        if (enableTimer)
        {
            ElapsedTime -= Time.deltaTime;
            if (ElapsedTime < 0)
            {
                highscoreWindow.SetActive(true);
                gameOverUI.SetActive(false);
                enableTimer = false;
            }
        }
        if (_state != GameState.WaitingInput) 
            return;

        if ((selection == 2 || Input.GetKey(KeyCode.S)))
        {
            selection = 0;
            Shift(Vector2.down);
        }
        else if ((selection == 1 || Input.GetKey(KeyCode.W)))
        {
            selection = 0;
            Shift(Vector2.up);
        }
        else if ((selection == 4 || Input.GetKey(KeyCode.D)))
        {
            selection = 0;
            Shift(Vector2.right);
        }
        else if ((selection == 3 || Input.GetKey(KeyCode.A)))
        {
            selection = 0;
            Shift(Vector2.left);
        }
    }

    void GenerateGrid()
    {
        _round = 0;
        _nodes = new List<Node2048>();
        _blocks = new List<Block2048>();
        for(int x = 0; x < _width; x++)
            for(int y = 0; y < _height; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
                _nodes.Add(node);
            }

        var center = new Vector2((float) _width / 2 - 0.5f, (float) _height / 2 - 0.5f);

        var board = Instantiate(_boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);

        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        ChangeState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {
        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null).OrderBy(b => Random.value).ToList();

        foreach(var node in freeNodes.Take(amount))
        {
            SpawnBlock(node, Random.value > 0.8f ? 4 : 2);
        }

        if(freeNodes.Count() == 1)
        {
            //gameover
            ChangeState(GameState.Lose);
            return;
        }

        ChangeState(_blocks.Any(b => b.Value == _winCondition) ? GameState.Lose : GameState.WaitingInput);
    }

    void SpawnBlock(Node2048 node, int value)
    {
        var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        _finalPoints += value;
        _finalPointsText.text = "Points: " + _finalPoints.ToString();
        block.SetBlock(node);
        _blocks.Add(block);
    }

    void Shift(Vector2 dir)
    {
        ChangeState(GameState.Moving);
        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up)
            orderedBlocks.Reverse();

        foreach (var block in orderedBlocks)
        {
            var next = block.Node;
            do
            {
                block.SetBlock(next);

                var possbileNode = GetNodeAtPosition(next.Pos + dir);
                if(possbileNode != null)
                {
                    if(possbileNode.OccupiedBlock != null && possbileNode.OccupiedBlock.CanMerge(block.Value))
                    {
                        block.MergeBlock(possbileNode.OccupiedBlock);
                    }

                    else if (possbileNode.OccupiedBlock == null)
                        next = possbileNode;
                }

            } while (next != block.Node);

            block.transform.DOMove(block.Node.Pos, _travelTime);

        }

        var sequence = DOTween.Sequence();

        foreach(var block in orderedBlocks)
        {
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime));
        }

        sequence.OnComplete(() => {
            foreach (var block in orderedBlocks.Where(b => b.MergingBlock != null))
            {
                MergeBlocks(block.MergingBlock, block);
            }

            ChangeState(GameState.SpawningBlocks);
        });

    }

    void MergeBlocks(Block2048 baseBlock, Block2048 mergingBlock)
    {
        SpawnBlock(baseBlock.Node, baseBlock.Value * 2);
        _finalPoints -= baseBlock.Value * 2;
        _finalPointsText.text = "Points: " + _finalPoints.ToString();
        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block2048 block)
    {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    Node2048 GetNodeAtPosition(Vector2 pos)
    {
        return _nodes.FirstOrDefault(n => n.Pos == pos);
    }

}

[Serializable]
public struct BlockType
{
    public int Value;
    public Color Color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Lose
}