using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
	public partial class EventForm : Form
	{
		EventObject[] eventObjects;

		public EventForm(EventObject[] objects)
		{
			InitializeComponent();
			eventObjects = objects;

			textBox1.Text = objects[0].EventID.ToString();
		}

		private void IDBox_Changed(object sender, EventArgs e)
		{
			int id;

			if (int.TryParse(textBox1.Text, out id))
			{
				foreach (EventObject o in eventObjects)
					o.EventID = id;
			}
		}

		private void colorButton_Click(object sender, EventArgs e)
		{
			DialogResult result = colorDialog1.ShowDialog();

			if (result == DialogResult.OK)
			{
				colorButton.BackColor = colorDialog1.Color;
				TKTools.Color c = new TKTools.Color(colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);

				foreach (EventObject eo in eventObjects)
					eo.color = c;
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
