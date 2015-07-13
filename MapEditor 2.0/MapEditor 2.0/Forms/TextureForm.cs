using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKTools;

public partial class TextureForm : Form
{
	public class TextureItem : ListViewItem
	{
		TextureSet textureSet;

		public TextureSet TextureSet
		{
			get { return textureSet; }
		}

		public Bitmap Bitmap
		{
			get { return textureSet.Bitmap; }
		}

		public TextureItem(TextureSet set)
			:base(set.Name)
		{
			textureSet = set;
		}
	}

	public TextureForm()
	{
		InitializeComponent();
	}

	private void ListIndexChanged(object sender, EventArgs e)
	{
		if (textureList.SelectedItems.Count > 0)
		{
			previewBox.BackgroundImage = (textureList.SelectedItems[0] as TextureItem).Bitmap;
		}

		deleteButton.Enabled = editButton.Enabled = textureList.SelectedItems.Count > 0;
	}

	private void AddButtonPressed(object sender, EventArgs e)
	{
		OpenFileDialog dialog = new OpenFileDialog();
		dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
		dialog.Multiselect = true;

		if (dialog.ShowDialog() == DialogResult.OK)
		{
			foreach(string f in dialog.FileNames)
			{
				Bitmap b = new Bitmap(f);

				TextureSet set = new TextureSet("Untitled", b);
				Editor.CurrentEditor.CreateTextureSet(set);
				ListViewItem i = textureList.Items.Add(new TextureItem(set));
				i.Selected = true;
				i.BeginEdit();
			}
		}
	}

	private void DeleteButtonPressed(object sender, EventArgs e)
	{
		foreach(TextureItem t in textureList.SelectedItems)
		{
			Editor.CurrentEditor.RemoveTextureSet(t.TextureSet);
			textureList.Items.Remove(t);
		}

		previewBox.BackgroundImage = null;
	}

	private void EditButtonPressed(object sender, EventArgs e)
	{
		TileEditor form = new TileEditor(textureList.SelectedItems[0] as TextureItem);
		form.ShowDialog();
	}

	private void TextureNameChanged(object sender, LabelEditEventArgs e)
	{
		if (e.Label == "" || e.Label == null)
		{
			e.CancelEdit = true;
			return;
		}

		textureList.SelectedItems.Clear();

		(textureList.Items[e.Item] as TextureItem).TextureSet.Name = e.Label;
		textureList.Items[e.Item].Selected = true;
	}

	private void OnClose(object sender, FormClosingEventArgs e)
	{
		this.Hide();
		e.Cancel = true;
	}
}
