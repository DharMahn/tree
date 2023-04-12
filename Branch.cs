using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
namespace trees
{
    internal class Branch
    {
        public Vector2 Start;
        public Vector2 End;
        public List<Branch> Children;
        public bool Finished = false;
        public Branch(Vector2 start, Vector2 end)
        {
            Start = start; End = end;
            Children= new List<Branch>();
        }
        public void AddChild(Branch child)
        {
            Children.Add(child);
        }
        public void Draw(Graphics g)
        {
            g.DrawLine(Pens.Brown, Start.X, Start.Y, End.X, End.Y);
        }
    }
}
