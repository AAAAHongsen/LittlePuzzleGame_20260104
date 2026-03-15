using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1132044_final_exersice
{
    internal class PuzzleManager
    {
        public int SnapTolerance { get; set; } = 18;

        private readonly int _n;
        private readonly int _tileW;
        private readonly int _tileH;

        private readonly List<PieceForm> _forms = new List<PieceForm>();
        private readonly Dictionary<int, HashSet<PieceForm>> _groups = new Dictionary<int, HashSet<PieceForm>>();
        private int _nextGroupId = 1;

        // pieceId (col,row)
        private readonly Dictionary<int, Point> _gridPos = new Dictionary<int, Point>(); //PieceForm座標
        public PuzzleManager(int n, int tileW, int tileH)
        {
            _n = n;
            _tileW = tileW;
            _tileH = tileH;

            // 預設 pieceId = row*n + col
            for (int id = 0; id < n * n; id++)
            {
                int row = id / n;
                int col = id % n;
                _gridPos[id] = new Point(col, row);
            }
        }
        public void Register(PieceForm f)
        {
            _forms.Add(f);

            int gid = _nextGroupId++;
            HashSet<PieceForm> set = new HashSet<PieceForm>();
            set.Add(f);
            _groups[gid] = set;
            f.GroupId = gid;

            f.DragDelta += OnDragDelta;
            f.DragEnded += OnDragEnded;

            f.FormClosed += delegate (object sender, FormClosedEventArgs e)
            {
                Unregister(f);
            };
        }
        private void Unregister(PieceForm f)
        {
            _forms.Remove(f);

            if (_groups.ContainsKey(f.GroupId))
            {
                _groups[f.GroupId].Remove(f);
                if (_groups[f.GroupId].Count == 0)
                    _groups.Remove(f.GroupId);
            }
        }
        private void OnDragDelta(PieceForm leader, Point delta)
        {
            if (!_groups.ContainsKey(leader.GroupId)) return;
            HashSet<PieceForm> group = _groups[leader.GroupId];

            foreach (PieceForm f in group)
            {
                if (f == leader) continue;
                f.SuppressDragEvents = true;
                f.Location = new Point(f.Left + delta.X, f.Top + delta.Y);
                f.SuppressDragEvents = false;
            }
        }
        private void OnDragEnded(PieceForm leader)
        {
            TrySnapCorrectly(leader);
        }
        private void TrySnapCorrectly(PieceForm leader)
        {
            if (!_groups.ContainsKey(leader.GroupId)) return;
            HashSet<PieceForm> leaderGroup = _groups[leader.GroupId];

            // 其他群的所有碎片
            List<PieceForm> others = _forms.Where(f => f.GroupId != leader.GroupId).ToList();

            foreach (PieceForm a in leaderGroup)
            {
                foreach (PieceForm b in others)
                {
                    if (!AreNeighbors(a.PieceId, b.PieceId)) continue;

                    Point expected = ExpectedOffset(a.PieceId, b.PieceId);
                    Point actual = new Point(b.Left - a.Left, b.Top - a.Top);

                    if (Math.Abs(actual.X - expected.X) <= SnapTolerance &&
                        Math.Abs(actual.Y - expected.Y) <= SnapTolerance)
                    {
                        // 把 b 一群移到正確位置
                        Point correction = new Point(expected.X - actual.X, expected.Y - actual.Y);
                        MoveGroup(b.GroupId, correction);

                        MergeGroups(leader.GroupId, b.GroupId);
                        return;
                    }
                }
            }
        }
        private bool AreNeighbors(int idA, int idB)
        {
            Point pA = _gridPos[idA];
            Point pB = _gridPos[idB];

            int dc = Math.Abs(pA.X - pB.X);
            int dr = Math.Abs(pA.Y - pB.Y);

            return (dc + dr) == 1;
        }
        private Point ExpectedOffset(int idA, int idB)
        {
            Point pA = _gridPos[idA];
            Point pB = _gridPos[idB];

            int dCol = pB.X - pA.X;
            int dRow = pB.Y - pA.Y;

            return new Point(dCol * _tileW, dRow * _tileH);
        }
        private void MoveGroup(int groupId, Point delta)
        {
            if (!_groups.ContainsKey(groupId)) return;
            HashSet<PieceForm> group = _groups[groupId];

            foreach (PieceForm f in group)
            {
                f.SuppressDragEvents = true;
                f.Location = new Point(f.Left + delta.X, f.Top + delta.Y);
                f.SuppressDragEvents = false;
            }
        }
        private void MergeGroups(int gidKeep, int gidMerge)
        {
            if (gidKeep == gidMerge) return;
            if (!_groups.ContainsKey(gidKeep) || !_groups.ContainsKey(gidMerge)) return;

            HashSet<PieceForm> A = _groups[gidKeep];
            HashSet<PieceForm> B = _groups[gidMerge];

            foreach (PieceForm f in B)
            {
                f.GroupId = gidKeep;
                A.Add(f);
            }
            _groups.Remove(gidMerge);
            if (!_completedFired && CheckCompleted())
            {
                _completedFired = true;
                if (Completed != null) Completed();
            }
        }
        public event Action Completed;
        private bool _completedFired = false;
        public bool CheckCompleted()
        {
            // 全部碎片開啟
            if (_forms.Count < _n * _n) return false;
            if (_groups.Count == 1) return true;

            return false;
        }
        //關視窗
        public List<PieceForm> GetAllForms()
        {
            return new List<PieceForm>(_forms);
        }
        public bool IsCompleted()
        {
            return _groups.Count == 1;
        }
    }
}
