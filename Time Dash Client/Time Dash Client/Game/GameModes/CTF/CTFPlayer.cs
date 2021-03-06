﻿using OpenTK;
using System;
using TKTools;

public class CTFPlayer : Player
{
	protected new CTFMap Map
	{
		get
		{
			return (CTFMap)base.Map;
		}
	}

	CTFFlag holdingFlag;
	Mesh arrowMesh;

	public bool HoldingFlag
	{
		get
		{
			return holdingFlag != null;
		}
	}

	public CTFPlayer(int id, string name, Vector2 position, Map m)
		: base(id, name, position, m)
	{
		arrowMesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);

		arrowMesh.Vertices = new Vector2[] {
			new Vector2(-1.4f, 0.4f),
			new Vector2(0f, 0f),
			new Vector2(-1.4f, -0.4f)
		};

		arrowMesh.UV = arrowMesh.Vertices;

		arrowMesh.Color = Color;
	}

	public override void Dispose()
	{
		arrowMesh.Dispose();

		base.Dispose();
	}

	public void GrabFlag(CTFFlag flag)
	{
		holdingFlag = flag;
		flag.holder = this;

		Map.FlagStolen(flag);
	}

	public override void Draw()
	{
		base.Draw();

		CTFFlag flag = Map.flags[Team.id];

		if ((!flag.IsInBase || flag.holder != null) && IsLocalPlayer)
		{
			Vector2 flagPosition = flag.holder == null ? flag.Position : flag.holder.Position;
			float dis = (flagPosition - Position).Length;
			Color c = Team.Color;
			c.A = MathHelper.Clamp((dis - 7f) / 2f, 0, 1);

			arrowMesh.Color = c;

			arrowMesh.Reset();

			arrowMesh.Translate(position);
			arrowMesh.Rotate(TKMath.GetAngle(flagPosition - position));
			arrowMesh.Translate(Math.Min(7f, (flagPosition - position).Length), 0f);
			arrowMesh.Scale(0.7f);

			arrowMesh.Draw();
		}
	}
}