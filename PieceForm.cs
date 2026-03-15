using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1132044_final_exersice
{
    public partial class PieceForm : Form
    {
        public int PieceId { get; private set; }
        public int GroupId { get; set; }
        public PieceForm(int pieceId, Image pieceImage)
        {
            InitializeComponent();
            PieceId = pieceId;
            SuppressDragEvents = false;

            this.ClientSize = new Size(232, 264);
            //this.Text = $"碎片 {pieceId}";
            this.Text = $"碎片";

            pic.Image = pieceImage;
        }
        //==================================================
        public bool SuppressDragEvents { get; set; }
        public delegate void DragDeltaHandler(PieceForm sender, Point delta);
        public event DragDeltaHandler DragDelta;
        public delegate void DragEndedHandler(PieceForm sender);
        public event DragEndedHandler DragEnded;

        private bool isDragging = false;
        private Point _lastMouseScreen;
        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            isDragging = true;
            _lastMouseScreen = MousePosition;
        }
        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;

            Point now = MousePosition;
            Point delta = new Point(now.X - _lastMouseScreen.X, now.Y - _lastMouseScreen.Y);
            _lastMouseScreen = now;

            this.Location = new Point(this.Left + delta.X, this.Top + delta.Y);

            if (!SuppressDragEvents && DragDelta != null)
                DragDelta(this, delta);
        }
        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;
            isDragging = false;

            if (!SuppressDragEvents && DragEnded != null)
                DragEnded(this);
        }
    }
}
