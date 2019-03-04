using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CannyEdgeDetectionDemo
{
    public partial class EdgeDetectionForm : Form
    {
        public EdgeDetectionForm()
        {
            InitializeComponent();
        }

        public void SetImage(Bitmap bm)
        {
            EdgeDetectionImage.Image = bm;
        }
    }
}
