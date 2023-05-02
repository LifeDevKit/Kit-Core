using System;
using UnityEngine;

namespace Kit.UIMGUI
{
	[Serializable]
	internal class ShaderData
	{
		public Shader Mesh;
		public Shader Procedural;

		public ShaderData Clone()
		{
			return (ShaderData)MemberwiseClone();
		}
	}
}
