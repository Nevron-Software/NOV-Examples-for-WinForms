using System;
using System.Windows.Forms;

using Nevron.Nov.UI;
using Nevron.Nov.Windows.Forms;
using Nevron.Nov.Dom;
using Nevron.Nov.Graphics;

namespace Nevron.Nov.Examples.WinForm
{
    public partial class NMainForm : Form
    {
        public NMainForm()
		{
			InitializeComponent();
			Text = "Nevron Open Vision Examples for Windows Forms";
			WindowState = FormWindowState.Maximized;

			// place a NOV WinForms Control that contains an NExampleContent widget
			Controls.Add(new NNOVWidgetHost<NExamplesContent>());
		}
    }
}