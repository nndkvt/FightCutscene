
		


float4 FilterCellData(float4 data) {
    /*
#if defined(HEX_MAP_EDIT_MODE)
    data.xy = 1;
#endif*/
    return data;
}


float4 GetCellData_URP(Attributes v, int index) {
    float2 uv;
    uv.x = (v.texcoord2[index] + 0.5) * _HexCellData_TexelSize.x;
    float row = floor(uv.x);
    uv.x -= row;
    uv.y = (row + 0.5) * _HexCellData_TexelSize.y;
    float4 data = SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0);
    data.w *= 255;
    return FilterCellData(data);
}


float4 GetCellData_URP(float2 cellDataCoordinates) {
    float2 uv = cellDataCoordinates + 0.5;
    uv.x *= _HexCellData_TexelSize.x;
    uv.y *= _HexCellData_TexelSize.y;
    return FilterCellData(  SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0)  );
}


float FogOfWar_URP(float2 UV, TEXTURE2D_PARAM(PackedNoisesTex, Sampler_PackedNoisesTex))
{
    const float2 scaledUV = UV * 0.005f;
    const float scaledTime =_Time.y * 0.01f;
	
    return (
        SAMPLE_TEXTURE2D(PackedNoisesTex, Sampler_PackedNoisesTex,
        scaledUV + scaledTime).x +
        SAMPLE_TEXTURE2D(PackedNoisesTex, Sampler_PackedNoisesTex,
            scaledUV - scaledTime*2.0).y * .5f
            );
}