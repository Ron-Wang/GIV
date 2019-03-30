// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Shader_HalfLambert" {
	Properties
	{
		_AlphaScale("Alpha Scale",Range(0,1)) = 1
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		cull off

		Pass
		{
			ZWrite On
			ColorMask 0  //用于设置颜色通道的写掩码,0表示不写入任何通道
		}

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		Pass{
		Tags{ "LightMode" = "ForwardBase" }

		CGPROGRAM
		#pragma multi_compile_fwdbase  
		#pragma vertex vert  
		#pragma fragment frag  
		
		#include "UnityCG.cginc"  
		#include "Lighting.cginc"  
		#include "AutoLight.cginc"  

		struct a2v {
		    float4 vertex : POSITION;
			float3 normal : NORMAL;
			fixed4 color : COLOR;
		};
		struct v2f 
		{
		    float4 pos : SV_POSITION;
			float3 worldNormal : TEXCOORD0;
			fixed4 color : COLOR;
			LIGHTING_COORDS(1, 2)
		};
		fixed _AlphaScale;

		v2f vert(a2v v) 
		{
		    v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);//投影变换
			o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);//法线变换到世界空间	
			o.color = v.color;
			TRANSFER_VERTEX_TO_FRAGMENT(o);
			return o;
		}
		
		fixed4 frag(v2f i) : SV_Target 
		{
			fixed3 worldNormal = normalize(i.worldNormal);//归一化法线
			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);//归一化光照方向 
			fixed3 lambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5; //半兰伯特模型计算顶点的光照,(-1,1)转换到(0,1)
			fixed3 shadow = 0.5 * LIGHT_ATTENUATION(i) + 0.5;//阴影，被遮挡变暗
			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 col = i.color.rgb * (lambert * _LightColor0.rgb * shadow + ambient);
			fixed4 color = fixed4(col,_AlphaScale);
			return color;
		}
		ENDCG
	}
	}
		FallBack "Diffuse"
}