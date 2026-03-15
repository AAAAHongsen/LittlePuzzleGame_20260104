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
    public partial class GameForm : Form
    {
        private PuzzleManager puzzleManager;
        Dictionary<int, Image> pieceImages;
        public GameForm()
        {
            InitializeComponent();
            pieceImages = new Dictionary<int, Image>();
            MessageBox.Show("使用WASD移動，蒐集碎片完成拼圖吧！", "遊玩教學");
            player.Image = imageListPlayer.Images[6];

            ItemsSet();
            puzzleManager = new PuzzleManager(3, 232, 264);
            puzzleManager.SnapTolerance = 18;
            puzzleManager.Completed += PuzzleManager_Completed;
        }
        private void ItemsSet()
        {
            pieceImages[0] = imageListPieces.Images[0];
            pieceImages[1] = imageListPieces.Images[1];
            pieceImages[2] = imageListPieces.Images[2];
            pieceImages[3] = imageListPieces.Images[3];
            pieceImages[4] = imageListPieces.Images[4];
            pieceImages[5] = imageListPieces.Images[5];
            pieceImages[6] = imageListPieces.Images[6];
            pieceImages[7] = imageListPieces.Images[7];
            pieceImages[8] = imageListPieces.Images[8];
            items.Add(item1); items.Add(item2); items.Add(item3); items.Add(item4);items.Add(item5); items.Add(item6); items.Add(item7); items.Add(item8); items.Add(item9);
            item1.Tag = 0; item2.Tag = 1; item3.Tag = 2; item4.Tag = 3;item5.Tag = 4; item6.Tag = 5; item7.Tag = 6; item8.Tag = 7; item9.Tag = 8;
        }
        //==================================================
        private void timer1_Tick(object sender, EventArgs e)
        {
            playerMove();
            touchItem();
        }
        //==================================================
        bool moveUp, moveDown, moveLeft, moveRight;
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    moveUp=true;
                    break;
                case Keys.S:
                    moveDown=true;
                    break;
                case Keys.A:
                    moveLeft=true;
                    break;
                case Keys.D:
                    moveRight=true;
                    break;
            }
        }
        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    moveUp = false;
                    player.Image = imageListPlayer.Images[0];
                    break;
                case Keys.S:
                    moveDown = false;
                    player.Image = imageListPlayer.Images[6];
                    break;
                case Keys.A:
                    moveLeft = false;
                    player.Image = imageListPlayer.Images[12];
                    break;
                case Keys.D:
                    moveRight = false;
                    player.Image = imageListPlayer.Images[18];
                    break;
            }
        }
        //--------------------------------------------------
        int playerPic = 0;
        int frame = 0; //一組6張
        private void playerMove()
        {
            int step = 5;
            frame++;
            if (frame >= 5)
            {
                playerPic = (playerPic + 1) % 6;
                frame = 0;
            }
            if (moveUp && player.Top > 17)
            {
                player.Top -= step;
                player.Image = imageListPlayer.Images[0 + playerPic];
            }
            else if (moveDown && (player.Top + player.Height) < this.ClientSize.Height-40)
            {
                player.Top += step;
                player.Image = imageListPlayer.Images[6 + playerPic];
            }
            else if (moveLeft && player.Left > 7)
            {
                player.Left -= step;
                player.Image = imageListPlayer.Images[12 + playerPic];
            }
            else if (moveRight && (player.Left + player.Width) < this.ClientSize.Width-9)
            {
                player.Left += step;
                player.Image = imageListPlayer.Images[18 + playerPic];
            }
        }

        //==================================================
        List<PictureBox> items = new List<PictureBox>();
        int itemNum = 0;
        bool have9items = false;
        private void touchItem()
        {
            var collectedItems = items.Where(it => player.Bounds.IntersectsWith(it.Bounds)).ToList();
            foreach (var item in collectedItems)
            {
                CollectItem(item);
            }
            if (itemNum >= 9 && !have9items)
            {
                have9items = true;
                MessageBox.Show("碎片蒐集完成，點擊碎片並拖曳完成拼圖吧！", "已蒐集完碎片");
            }
        }
        private void CollectItem(PictureBox item)
        {
            //消除
            item.Visible = false;
            items.Remove(item);
            itemNum++;
            //放入物品欄
            var icon = new PictureBox();
            icon.Width = 20;
            icon.Height = 20;
            icon.BackgroundImageLayout = ImageLayout.Zoom;
            icon.BackgroundImage = item.BackgroundImage;
            icon.Tag = item.Tag; //ID
            icon.Cursor = Cursors.Hand;
            icon.Click += Icon_Click;

            flowLayoutPanel1.Controls.Add(icon);
        }
        //==================================================
        Dictionary<int, PieceForm> openedPieceForms = new Dictionary<int, PieceForm>();
        private void Icon_Click(object sender, EventArgs e) //開視窗
        {
            var icon = (PictureBox)sender;
            int pieceId = (int)icon.Tag;

            if (openedPieceForms.ContainsKey(pieceId))
            {
                openedPieceForms[pieceId].Activate();
                return;
            }
            Image pieceImage = pieceImages[pieceId];

            PieceForm pieceForm = new PieceForm(pieceId, pieceImage);
            puzzleManager.Register(pieceForm);

            pieceForm.FormClosed += delegate (object s, FormClosedEventArgs args)
            {
                openedPieceForms.Remove(pieceId);
            };
            openedPieceForms[pieceId] = pieceForm;
            pieceForm.Show();
        }
        //==================================================
        private void PuzzleManager_Completed() //拼圖完成
        {
            // 在 UI 執行緒
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(PuzzleManager_Completed));
                return;
            }

            foreach (var pf in puzzleManager.GetAllForms())
            {
                if (!pf.IsDisposed)
                    pf.Close();
            }
            openedPieceForms.Clear();

            CompletedForm completedForm = new CompletedForm();
            completedForm.Show();
            MessageBox.Show("完成拼圖了！恭喜！", "拼圖已完成");
        }
    }
}
