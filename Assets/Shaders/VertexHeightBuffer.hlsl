#ifndef VERTEX_HEIGHT_BUFFER_INCLUDED
#define VERTEX_HEIGHT_BUFFER_INCLUDED

StructuredBuffer<float> _VertexHeightBuffer;

void GetVertexHeight_float(float index, out float value)
{
    uint i = (uint)floor(index);
    value = _VertexHeightBuffer[i];
}

#endif