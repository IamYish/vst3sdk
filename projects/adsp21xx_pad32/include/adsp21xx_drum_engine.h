#pragma once

#include "adsp21xx_fixed_math.h"

#include <array>
#include <cstdint>
#include <optional>
#include <span>
#include <string>
#include <vector>

namespace adsp21xx
{
constexpr int kMaxPads = ADSP21XX_PAD_COUNT;

struct SampleBuffer
{
    std::vector<Fixed24::storage_type> samples;
    uint32_t sampleRate {48000};
};

struct PadParameters
{
    Fixed24 gain {Fixed24::fromFloat(1.f)};
    Fixed24 panLeft {makePanCoefficient(0.f, true)};
    Fixed24 panRight {makePanCoefficient(0.f, false)};
    uint32_t pitchIncrement {1u << 24};
    bool triggerEnabled {true};
};

struct PadState
{
    SampleBuffer buffer;
    PadParameters params;
};

struct PadVoice
{
    const SampleBuffer* buffer {nullptr};
    PadParameters params;
    uint32_t phase {0};
    bool active {false};

    void trigger(const SampleBuffer& buf, const PadParameters& parameters, Fixed24 velocity) noexcept;
    void stop() noexcept { active = false; }
    void process(FixedAccumulator& left, FixedAccumulator& right) noexcept;
};

class DrumEngine
{
public:
    DrumEngine();

    void setPadSample(int padIndex, SampleBuffer buffer);
    void setPadGain(int padIndex, float gainLinear);
    void setPadPan(int padIndex, float pan);
    void setPadPitchRatio(int padIndex, float ratio);
    void setPadTriggerEnabled(int padIndex, bool enabled);

    [[nodiscard]] bool padTriggerEnabled(int padIndex) const;
    [[nodiscard]] const SampleBuffer* padSample(int padIndex) const;

    void triggerPad(int padIndex, float velocity);
    void releasePad(int padIndex);

    void processBlock(std::span<float> outLeft, std::span<float> outRight);

    void setSelectedPad(int padIndex);
    [[nodiscard]] int selectedPad() const noexcept { return selectedPadIndex; }

private:
    std::array<PadState, kMaxPads> pads;
    std::array<PadVoice, kMaxPads> voices;
    int selectedPadIndex {0};
};

SampleBuffer makeSampleFromFloat(const std::vector<float>& input, uint32_t sampleRate);

} // namespace adsp21xx
