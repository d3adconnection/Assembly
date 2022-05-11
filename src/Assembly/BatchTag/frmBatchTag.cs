using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Assembly
{
    public partial class frmStatus : Form
    {
        public delegate void UpdateMap(int mapIdx, int mapCnt, String mapMsg);
        public delegate void UpdateTag(int tagIdx, int tagCnt, String tagMsg);
        public frmStatus()
        {
            InitializeComponent();
        }

        private void frmStatus_Load(object sender, EventArgs e)
        {

        }

        public async void UpdateMapStatus(int mapIdx, int mapCnt, String mapMsg)
        {
            if (mapCnt != pbMap.Maximum) { pbMap.Maximum = mapCnt; }
            if (mapIdx != pbMap.Value) { pbMap.Value = mapIdx; pbMap.Refresh(); }
            if (mapMsg != null && mapMsg.CompareTo(txtMap.Text) != 0) { txtMap.Text = mapMsg; txtMap.Refresh(); }
            await Dispatcher.Yield();
        }
        public async void UpdateTagStatus(int tagIdx, int tagCnt, String tagMsg)
        {

            if (tagCnt != pbTag.Maximum) { pbTag.Maximum = tagCnt; }
            if (tagIdx != pbTag.Value) { pbTag.Value = tagIdx; pbTag.Refresh(); }
            if (tagMsg != null && tagMsg.CompareTo(txtTag.Text) != 0) { txtTag.Text = tagMsg; txtTag.Refresh(); }
            await Dispatcher.Yield();
        }
        private void pbMap_Click(object sender, EventArgs e)
        {

        }

        private void lblMap_Click(object sender, EventArgs e)
        {

        }
    }
}
