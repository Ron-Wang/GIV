Shader "Custom/Shader_Double" {
	Properties
    {
        _Color("Color Tint",Color) = (0.91,0.86,0.68,0.5)
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaScale("Alpha Scale",Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        LOD 100
		cull off

		Pass
        {
            ZWrite On
            ColorMask 0  //用于设置颜色通道的写掩码,0表示不写入任何通道
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            struct appdata
            {
                float3 normal:NORMAL;
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD2;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 pos : SV_POSITION;
            };
 
            fixed4 _Color;
            fixed _AlphaScale;
            sampler2D _MainTex;
            float4 _MainTex_ST;
             
            v2f vert (appdata v)
            {
                v2f o;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                return o;
            }
             
            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed4 texColor = tex2D(_MainTex,i.uv);
                fixed3 albedo = texColor.rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * (0.5*sqrt(dot(worldNormal,worldLightDir)*dot(worldNormal,worldLightDir)) + 0.5);
                return fixed4(ambient + diffuse,_AlphaScale);
            }
            ENDCG
        }
		}
}