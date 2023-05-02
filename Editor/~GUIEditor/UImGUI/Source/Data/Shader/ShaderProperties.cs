using System;

namespace Kit.UIMGUI
{
	[Serializable]
	internal class ShaderProperties
	{
		public string Texture;
		public string Vertices;
		public string BaseVertex;

		public ShaderProperties Clone()
		{
			return (ShaderProperties)MemberwiseClone();
		}
	}
}
