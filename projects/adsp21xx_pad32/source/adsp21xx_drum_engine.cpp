#include "adsp21xx_drum_engine.h"

#include <algorithm>
#include <cassert>
#include <cstring>

namespace adsp21xx
{
namespace
{
constexpr uint32_t kPhaseOne = 1u << 24;

inline uint32_t clampPadIndex(int padIndex)
{
    return static_cast<uint32_t>(std::clamp(padIndex, 0, kMaxPads - 1));
}

Fixed24::storage_type fetchSample(const SampleBuffer& buffer, uint32_t phase)
{
    if (buffer.samples.empty())
        return 0;

    const uint32_t index = phase >> 24;
    if (index >= buffer.samples.size())
        return 0;

    return buffer.samples[index];
}
} // namespace

void PadVoice::trigger(const SampleBuffer& buf, const PadParameters& parameters, Fixed24 velocity) noexcept
{
    buffer = &buf;
    params = parameters;
    params.gain *= velocity;
    phase = 0;
    active = params.triggerEnabled && !buf.samples.empty();
}

void PadVoice::process(FixedAccumulator& left, FixedAccumulator& right) noexcept
{
    if (!active || buffer == nullptr)
        return;

    const auto sampleValue = Fixed24 {fetchSample(*buffer, phase)};
    if (sampleValue.value == 0)
    {
        phase += params.pitchIncrement;
        if ((phase >> 24) >= buffer->samples.size())
            active = false;
        return;
    }

    left.mac(sampleValue, params.panLeft * params.gain);
    right.mac(sampleValue, params.panRight * params.gain);

    phase += params.pitchIncrement;
    if ((phase >> 24) >= buffer->samples.size())
        active = false;
}

DrumEngine::DrumEngine()
{
    for (auto& pad : pads)
    {
        pad.params.panLeft = makePanCoefficient(0.f, true);
        pad.params.panRight = makePanCoefficient(0.f, false);
        pad.params.gain = Fixed24::fromFloat(1.f);
        pad.params.pitchIncrement = kPhaseOne;
        pad.params.triggerEnabled = true;
    }
}

void DrumEngine::setPadSample(int padIndex, SampleBuffer buffer)
{
    pads[clampPadIndex(padIndex)].buffer = std::move(buffer);
}

void DrumEngine::setPadGain(int padIndex, float gainLinear)
{
    pads[clampPadIndex(padIndex)].params.gain = Fixed24::fromFloat(gainLinear);
}

void DrumEngine::setPadPan(int padIndex, float pan)
{
    pads[clampPadIndex(padIndex)].params.panLeft = makePanCoefficient(pan, true);
    pads[clampPadIndex(padIndex)].params.panRight = makePanCoefficient(pan, false);
}

void DrumEngine::setPadPitchRatio(int padIndex, float ratio)
{
    const float safeRatio = std::clamp(ratio, 0.125f, 8.f);
    const uint32_t increment = static_cast<uint32_t>(safeRatio * static_cast<float>(kPhaseOne));
    pads[clampPadIndex(padIndex)].params.pitchIncrement = std::max<uint32_t>(1, increment);
}

void DrumEngine::setPadTriggerEnabled(int padIndex, bool enabled)
{
    pads[clampPadIndex(padIndex)].params.triggerEnabled = enabled;
}

bool DrumEngine::padTriggerEnabled(int padIndex) const
{
    return pads[clampPadIndex(padIndex)].params.triggerEnabled;
}

const SampleBuffer* DrumEngine::padSample(int padIndex) const
{
    return &pads[clampPadIndex(padIndex)].buffer;
}

void DrumEngine::triggerPad(int padIndex, float velocity)
{
    const uint32_t index = clampPadIndex(padIndex);
    Fixed24 fixedVelocity = Fixed24::fromFloat(std::clamp(velocity, 0.f, 1.f));
    voices[index].trigger(pads[index].buffer, pads[index].params, fixedVelocity);
}

void DrumEngine::releasePad(int padIndex)
{
    voices[clampPadIndex(padIndex)].stop();
}

void DrumEngine::processBlock(std::span<float> outLeft, std::span<float> outRight)
{
    assert(outLeft.size() == outRight.size());
    const size_t frames = outLeft.size();

    for (size_t i = 0; i < frames; ++i)
    {
        FixedAccumulator left;
        FixedAccumulator right;
        left.clear();
        right.clear();

        for (auto& voice : voices)
        {
            voice.process(left, right);
        }

        outLeft[i] = left.asFixed().toFloat();
        outRight[i] = right.asFixed().toFloat();
    }
}

void DrumEngine::setSelectedPad(int padIndex)
{
    selectedPadIndex = static_cast<int>(clampPadIndex(padIndex));
}

SampleBuffer makeSampleFromFloat(const std::vector<float>& input, uint32_t sampleRate)
{
    SampleBuffer buffer;
    buffer.sampleRate = sampleRate;
    buffer.samples.reserve(input.size());

    for (float sample : input)
    {
        auto fixed = Fixed24::fromFloat(std::clamp(sample, -0.9999f, 0.9999f));
        buffer.samples.push_back(fixed.value);
    }

    return buffer;
}

} // namespace adsp21xx
