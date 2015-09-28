public partial class MeshDesigner : EditorUIForm
{
	public MeshDesign CurrentDesign
	{
		get
		{
			if (tabControl.SelectedTab == tabTile)
			{
				if (TilePicker.SelectedTile != null)
					return new MeshDesign(TilePicker.SelectedTile);
				else
					return new MeshDesign(ColorPicker.SelectedColor);
			}
			else return new MeshDesign(ColorPicker.SelectedColor);
		}
	}
	public TilePicker TilePicker { get { return tilePicker; } }
	public SolidColorPicker ColorPicker { get { return colorPicker; } }

	public MeshDesigner()
	{
		InitializeComponent();
	}
}
