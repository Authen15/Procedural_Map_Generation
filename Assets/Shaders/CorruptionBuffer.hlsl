#ifndef CORRUPTION_BUFFER_INCLUDED
#define CORRUPTION_BUFFER_INCLUDED

StructuredBuffer<uint> _CorruptionBuffer;

void GetCorruption_float(float index, out float value)
{
    uint i = (uint)floor(index);
    value = _CorruptionBuffer[i];
}

#endif