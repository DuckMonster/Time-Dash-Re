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
		EventTemplate selectedTemplate;

		List<int> parameters = new List<int>();

		public EventForm(EventObject[] objects)
		{
			InitializeComponent();
			eventObjects = objects;

			UpdateEventList();
			UpdateButtons();

			if (objects.Length > 0)
			{
				string str = "";

				for(int i=0; i<objects[0].parameters.Count; i++)
				{
					str += objects[0].parameters[i];
					if (i < objects[0].parameters.Count - 1)
						str += ", ";
				}

				textBoxParams.Text = str;
			}
		}

		void UpdateButtons()
		{
			bool selected = selectedTemplate != null;

			buttonRemove.Enabled = selected;
			buttonEdit.Enabled = selected;
			buttonOK.Enabled = selected;
		}

		public void UpdateEventList()
		{
			eventList.Items.Clear();

			foreach (EventTemplate t in EventTemplate.eventList)
				eventList.Items.Add(new ListViewItem(new string[] { t.ID.ToString(), t.Name }));
		}

		void Apply()
		{
			if (selectedTemplate != null)
				foreach (EventObject o in eventObjects)
				{
					o.SetTemplate(selectedTemplate);
					o.SetParameters(parameters);
				}
		}

		private void eventList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (eventList.SelectedItems.Count > 0)
				selectedTemplate = EventTemplate.eventList[eventList.SelectedItems[0].Index];
			else
				selectedTemplate = null;

			UpdateButtons();			
		}

		private void eventList_DoubleClick(object sender, EventArgs e)
		{
			Apply();
			Close();
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			EventCreator c = new EventCreator(this);
			c.ShowDialog();
		}

		private void buttonEdit_Click(object sender, EventArgs e)
		{
			EventCreator c = new EventCreator(this, selectedTemplate);
			c.ShowDialog();
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (selectedTemplate == null) return;

			EventTemplate.eventList.Remove(selectedTemplate);
			UpdateEventList();
			UpdateButtons();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			Apply();
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void textBoxParams_TextChanged(object sender, EventArgs e)
		{
			string[] par = textBoxParams.Text.Split(',');
			parameters.Clear();

			for(int i=0; i<par.Length; i++)
			{
				if (par[i] == "") continue;

				par[i] = par[i].Trim();
				int p;

				int.TryParse(par[i], out p);
				parameters.Add(p);
			}
		}
	}
}
