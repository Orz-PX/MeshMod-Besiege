using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MeshEntityData
{
    public long ID { get; set; }
    public Color Color { get; set; }

    public MeshEntityData() { }
    public MeshEntityData(long id, Color color)
    {
        ID = id; Color = color;
    }
    public MeshEntityData(string dataString)
    {
        string[] vs = dataString.Split('|');
        ID = long.Parse(vs[0]);
        Color = new Color(float.Parse(vs[1]), float.Parse(vs[2]), float.Parse(vs[3]), float.Parse(vs[4]));
    }

    public override string ToString()
    {
        return string.Format("{0}|{1}|{2}|{3}|{4}", ID, Color.r, Color.g, Color.b, Color.a);
    }
}
