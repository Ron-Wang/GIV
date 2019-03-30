Shader "Custom/Shader_Color" {
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Pass{

        CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_instancing
		#include "UnityCG.cginc"

		#include "UnityCG.cginc"  
		#include "Lighting.cginc"  
		#include "AutoLight.cginc"  

		struct a2v {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		struct v2f
		{
			float4 pos : SV_POSITION;
			float3 worldNormal : TEXCOORD0;
			LIGHTING_COORDS(1, 2)
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)

		v2f vert(a2v v)
		{
			v2f o;

			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);

			o.pos = UnityObjectToClipPos(v.vertex);//投影变换
			o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);//法线变换到世界空间	
			TRANSFER_VERTEX_TO_FRAGMENT(o);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i)

			fixed3 worldNormal = normalize(i.worldNormal);//归一化法线
			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);//归一化光照方向 
			fixed3 lambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5; //半兰伯特模型计算顶点的光照,(-1,1)转换到(0,1)
			fixed3 shadow = 0.5 * LIGHT_ATTENUATION(i) + 0.5;//阴影，被遮挡变暗
			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 col = UNITY_ACCESS_INSTANCED_PROP(Props, _Color).rgb * (lambert * _LightColor0.rgb * shadow + ambient);
			fixed4 color = fixed4(col,1);
			return color;
		}
		ENDCG
	}
	}
		FallBack "Diffuse"
}