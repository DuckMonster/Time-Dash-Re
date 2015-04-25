using System;
using System.Windows.Forms;
using TKTools;

namespace MapEditor
{
	public partial class EventCreator : Form
	{
		int id = -1;
		string name;
		Color? color;

		EventForm eventForm;

		EventTemplate editTemplate;

		public EventCreator(EventForm eventForm)
		{
			InitializeComponent();
			this.eventForm = eventForm;

			CheckIfFinished();
		}

		public EventCreator(EventForm eventForm, EventTemplate t)
			:this(eventForm)
		{
			editTemplate = t;
			textBoxID.Text = editTemplate.ID.ToString();
			textBoxName.Text = editTemplate.Name;
			colorButton.BackColor = System.Drawing.Color.FromArgb((int)(255 * t.Color.R), (int)(255 * t.Color.G), (int)(255 * t.Color.B));

			id = editTemplate.ID;
			name = editTemplate.Name;
			color = editTemplate.Color;

			CheckIfFinished();
		}

		void CheckIfFinished()
		{
			buttonOK.Enabled = ((name != null || name != "") && (id >= 0 && !EventTemplate.EventExists(id)) && color != null);
		}

		private void textBoxID_TextChanged(object sender, EventArgs e)
		{
			int.TryParse(textBoxID.Text, out id);
			CheckIfFinished();

			textBoxID.BackColor = System.Drawing.Color.FromArgb((int)(EventTemplate.EventExists(id) ? 0xFFFF0000 : 0xFFFFFFFF));
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			name = textBoxName.Text;
			CheckIfFinished();
		}

		private void colorButton_Click(object sender, EventArgs e)
		{
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				System.Drawing.Color c = colorDialog.Color;
				color = new Color(c.R, c.G, c.B);

				colorButton.BackColor = c;
			}

			CheckIfFinished();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (editTemplate == null)
				EventTemplate.CreateEvent(name, id, color.Value);
			else
				editTemplate.Edit(id, name, color.Value);

			eventForm.UpdateEventList();
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
