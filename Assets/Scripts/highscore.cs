using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highscore : IComparable<highscore>
{
    public int Score { get; set; }
    public string Name { get; set; }
    public int ID { get; set; }

    public highscore(int id, string name, int score)
    {
        this.ID = id;
        this.Name = name;
        this.Score = score;
    }

    public int CompareTo(highscore other)
    {
        if (other.Score < this.Score)
        {
            return -1;
        }

        else if (other.Score > this.Score)
        {
            return 1;
        }

        else if (other.Name.CompareTo(this.Name) < 0)
        {
            return 1;
        }

        else if (other.Name.CompareTo(this.Name) > 0)
        {
            return -1;
        }

        return 0;
    }
}
