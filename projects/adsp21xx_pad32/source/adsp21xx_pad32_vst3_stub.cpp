#include "adsp21xx_drum_engine.h"

#include <array>
#include <cstdint>
#include <iostream>
#include <string>
#include <vector>

namespace vst3stub
{
struct AudioBusBuffers
{
    std::array<float*, 2> channels {nullptr, nullptr};
    int32_t numChannels {2};
    int32_t numSamples {0};
};

struct ProcessData
{
    AudioBusBuffers outputs;
    int32_t numSamples {0};
};

struct NoteEvent
{
    int32_t padIndex {0};
    float velocity {1.f};
};
} // namespace vst3stub

class Pad32Processor
{
public:
    void trigger(const vst3stub::NoteEvent& event)
    {
        engine.triggerPad(event.padIndex, event.velocity);
    }

    void process(vst3stub::ProcessData& data)
    {
        if (data.outputs.numChannels < 2)
            return;

        std::span<float> left {data.outputs.channels[0], static_cast<size_t>(data.numSamples)};
        std::span<float> right {data.outputs.channels[1], static_cast<size_t>(data.numSamples)};
        engine.processBlock(left, right);
    }

    adsp21xx::DrumEngine& drumEngine() noexcept { return engine; }

private:
    adsp21xx::DrumEngine engine;
};

class Pad32Controller
{
public:
    void selectPad(int index)
    {
        engine.setSelectedPad(index);
    }

    void setPadGain(float gain)
    {
        engine.setPadGain(engine.selectedPad(), gain);
    }

    void setPadPan(float pan)
    {
        engine.setPadPan(engine.selectedPad(), pan);
    }

    void setPadTriggerEnabled(bool enabled)
    {
        engine.setPadTriggerEnabled(engine.selectedPad(), enabled);
    }

    adsp21xx::DrumEngine& drumEngine() noexcept { return engine; }

private:
    adsp21xx::DrumEngine engine;
};

void printFactoryInfo()
{
    std::cout << "ADSP21xx Pad32 Drum Sampler (VST3 stub)" << std::endl;
    std::cout << "- 32 pads" << std::endl;
    std::cout << "- Fixed-point DSP" << std::endl;
}

