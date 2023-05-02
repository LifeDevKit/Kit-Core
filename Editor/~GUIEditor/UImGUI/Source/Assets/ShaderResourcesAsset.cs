using UnityEngine;

namespace Kit.UIMGUI.Assets
{
	[CreateAssetMenu(menuName = "Dear ImGui/Shader Resources")]
	internal sealed class ShaderResourcesAsset : ScriptableObject
	{
		public ShaderData Shader;
		public ShaderProperties PropertyNames;
	}
}
 