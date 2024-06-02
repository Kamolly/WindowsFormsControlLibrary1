using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsControlLibrary1
{
    public partial class UserControl1: UserControl
    {
        private List<PointF> points = new List<PointF>();
        private List<PointF> convexHull = new List<PointF>();
        private const int PointRadius = 3;

        public UserControl1()
        {
            InitializeComponent();
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            points.Clear();
            convexHull.Clear();
            int n = 20;

            for (int i = 0; i < n; i++)
            {
                points.Add(new PointF(rand.Next(50, 700), rand.Next(50, 450)));
            }

            convexHull = CalculateConvexHull(points);
            this.Invalidate();
        }

        private List<PointF> CalculateConvexHull(List<PointF> points)
        {
            if (points.Count < 3)
                return new List<PointF>(points);

            PointF start = points.OrderBy(p => p.Y).ThenBy(p => p.X).First();
            points.Remove(start);

            points = points.OrderBy(p => Math.Atan2(p.Y - start.Y, p.X - start.X)).ToList();

            List<PointF> hull = new List<PointF> { start };

            foreach (var point in points)
            {
                while (hull.Count >= 2 && CrossProduct(hull[hull.Count - 2], hull[hull.Count - 1], point) <= 0)
                {
                    hull.RemoveAt(hull.Count - 1);
                }
                hull.Add(point);
            }

            return hull;
        }

        private float CrossProduct(PointF a, PointF b, PointF c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen hullPen = new Pen(Color.Red, 2);
            Brush pointBrush = Brushes.Blue;

            foreach (var point in points)
            {
                g.FillEllipse(pointBrush, point.X - PointRadius, point.Y - PointRadius, PointRadius * 2, PointRadius * 2);
            }

            if (convexHull.Count > 1)
            {
                for (int i = 0; i < convexHull.Count; i++)
                {
                    PointF p1 = convexHull[i];
                    PointF p2 = convexHull[(i + 1) % convexHull.Count];
                    g.DrawLine(hullPen, p1, p2);
                }
            }
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            this.Width = 800;
            this.Height = 600;

            Button generateButton = new Button
            {
                Text = "Generate Points",
                Location = new Point(10, 10)
            };
            generateButton.Click += GenerateButton_Click;
            this.Controls.Add(generateButton);

            this.Paint += DrawingPanel_Paint;
        }
    }
}
