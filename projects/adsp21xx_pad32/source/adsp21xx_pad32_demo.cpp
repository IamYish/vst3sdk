#include "adsp21xx_drum_engine.h"

#include <array>
#include <cmath>
#include <iostream>
#include <numbers>
#include <vector>

using namespace adsp21xx;

namespace
{
std::vector<float> makeSine(float freq, uint32_t rate, size_t frames)
{
    std::vector<float> buffer(frames);
    const float phaseInc = 2.f * std::numbers::pi_v<float> * freq / static_cast<float>(rate);
    float phase = 0.f;
    for (size_t i = 0; i < frames; ++i)
    {
        buffer[i] = std::sin(phase) * 0.5f;
        phase += phaseInc;
    }
    return buffer;
}
}

int main()
{
    DrumEngine engine;
    engine.setSelectedPad(0);
    engine.setPadPan(0, -0.25f);
    engine.setPadGain(0, 0.85f);
    engine.setPadTriggerEnabled(0, true);

    auto sine = makeSine(220.f, 48000, 4800);
    engine.setPadSample(0, makeSampleFromFloat(sine, 48000));

    std::array<float, 64> left {};
    std::array<float, 64> right {};
    std::span leftSpan {left.data(), left.size()};
    std::span rightSpan {right.data(), right.size()};

    engine.triggerPad(0, 1.f);
    engine.processBlock(leftSpan, rightSpan);

    for (size_t i = 0; i < left.size(); ++i)
    {
        std::cout << left[i] << "," << right[i] << '\n';
    }
}
