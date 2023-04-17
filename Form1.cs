using System.Diagnostics;
using System.Numerics;

namespace trees
{
    public partial class Form1 : Form
    {
        Bitmap treeTexture;
        List<Branch> branches;
        List<Vector2> targetDots;
        static Random r = new();
        public float branchLen = 20;
        Rectangle branchBounds;
        int boundsBorder = 25;
        float dotSearchRadius = 20;
        float dotElimRadius = 25;
        Graphics g;
        float leafRadius = 25;
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            WindowState = FormWindowState.Maximized;
            Bounds = Screen.PrimaryScreen.Bounds;
            MaximizedBounds = Screen.PrimaryScreen.Bounds;
            dotSearchRadius = (float)numericUpDown1.Value;
            dotElimRadius = (float)numericUpDown2.Value;
            branchLen = (float)numericUpDown3.Value;
            boundsBorder = (int)numericUpDown4.Value;
            RegenerateDotBounds();
            Reset();
            DoubleBuffered = true;
            //for (int i = 0; i < 20; i++)
            //{
            //    UpdateBranches();
            //}
            DrawBranches();
        }
        void RegenerateDots()
        {
            targetDots = new();
            for (int i = 0; i < 3000; i++)
            {
                targetDots.Add(new(r.Next(branchBounds.Left, branchBounds.Right), r.Next(branchBounds.Top, branchBounds.Bottom)));
            }
        }
        void RegenerateDotBounds(Rectangle? rect = null)
        {
            if (rect==null)
            {
                branchBounds = new(boundsBorder, boundsBorder, ClientSize.Width - (boundsBorder * 2), ClientSize.Height - (boundsBorder * 2));

            }
            else
            {
                branchBounds = rect.Value;
            }
        }
        void Reset()
        {
            treeTexture = new Bitmap(ClientSize.Width, ClientSize.Height);
            branches = new();
            RegenerateDots();
            branches.Add(new(new(treeTexture.Width / 2, treeTexture.Height), new(treeTexture.Width / 2, treeTexture.Height - branchLen)));
            stem = branches.Last();

            g = Graphics.FromImage(treeTexture);
        }
        Branch stem = null;
        void UpdateBranches()
        {
            //List<Branch> childless = new();
            //foreach (Branch branch in branches)
            //{
            //    if (branch.Children.Count == 0)
            //    {
            //        childless.Add(branch);
            //    }
            //}
            if (stem.End.Y > branchBounds.Bottom - (branchBounds.Height / 2))
            {
                Branch b = new(new(stem.End.X, stem.End.Y), new(stem.End.X, stem.End.Y - branchLen));
                stem.AddChild(b);
                branches.Add(b);
                stem = b;
            }

            var list = branches.Where(x => !x.Finished).ToList();

            foreach (var branch in list)
            {
                List<Vector2> closeEnoughDots = new();
                foreach (var dot in targetDots.ToList())
                {
                    if (Vector2.Distance(dot, branch.End) < dotSearchRadius)
                    {
                        closeEnoughDots.Add(dot);
                    }
                }

                if (closeEnoughDots.Count > 0)
                {
                    Vector2 avg = Vector2.Zero;
                    foreach (var dot in closeEnoughDots)
                    {
                        avg += dot - branch.End;
                    }
                    avg /= closeEnoughDots.Count;
                    avg = Vector2.Normalize(avg);
                    Branch newBranch = new Branch(
                            new Vector2(
                                branch.End.X,
                                branch.End.Y),
                            branch.End + (avg * branchLen));
                    branch.AddChild(newBranch);
                    branches.Add(newBranch);
                    foreach (var dot in closeEnoughDots)
                    {
                        if (Vector2.Distance(dot, newBranch.End) < dotElimRadius)
                        {
                            targetDots.Remove(dot);
                        }
                    }
                }
                else
                {
                    //use this for optimization
                    branch.Finished = true;
                    //branches.Remove(branch);
                }
            }

        }
        Pen branchCol = new Pen(Color.FromArgb(255, Color.Brown), 5);
        void DrawBranches()
        {
            g.Clear(SystemColors.Control);
            foreach (Vector2 dot in targetDots)
            {
                g.FillRectangle(Brushes.Blue, dot.X - 2, dot.Y - 2, 5, 5);
            }

            //foreach (Branch branch in branches)
            //{
            //    g.FillEllipse(Brushes.DarkGreen, branch.End.X - leafRadius, branch.End.Y - leafRadius, leafRadius * 2, leafRadius * 2);
            //}
            foreach (Branch branch in branches)
            {
                g.DrawLine(branchCol, branch.Start.X, branch.Start.Y, branch.End.X, branch.End.Y);
            }

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(treeTexture, 0, 0);
            if (dragging)
            {
                e.Graphics.DrawRectangle(Pens.Blue,GetNormalizedRectangle(start,end));
            }
            else
            {
                e.Graphics.DrawRectangle(Pens.Blue, branchBounds);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dotSearchRadius = (float)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            dotElimRadius = (float)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            branchLen = (float)numericUpDown3.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
            DrawBranches();
            Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateBranches();
            DrawBranches();
            Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            boundsBorder = (int)numericUpDown4.Value;
            branchBounds = new(boundsBorder, boundsBorder, treeTexture.Width - (boundsBorder * 2), treeTexture.Height - (boundsBorder * 2));
            RegenerateDotBounds(branchBounds);
            RegenerateDots();
            //Reset();
            //UpdateBranches();
            //DrawBranches();
            Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                UpdateBranches();
            }
            DrawBranches();
            Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateBranches();
            DrawBranches();
            Invalidate();
        }
        Point start, end;
        bool dragging = false;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            start = PointToClient(Cursor.Position);
            dragging = true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                end = PointToClient(Cursor.Position);
            }
            //DrawBranches();
            Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            end = PointToClient(Cursor.Position);
            if (end.X < start.X || end.Y < start.Y)
            {
                Point temp = start;
            }
            dragging = false;
            branchBounds = GetNormalizedRectangle(start,end);
            //Reset();
            RegenerateDotBounds(branchBounds);
            RegenerateDots();
            DrawBranches();
            Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Space)
            {
                UpdateBranches();
                DrawBranches();
                Invalidate();
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private Rectangle GetNormalizedRectangle(Point p1, Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            return new Rectangle(x, y, width, height);
        }
    }
}