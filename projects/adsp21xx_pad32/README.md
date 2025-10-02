# ADSP21xx Pad32 Drum Sampler

This prototype demonstrates a 32–pad drum sampler whose DSP core is implemented using
ADSP21xx-style fixed-point arithmetic. The code focuses on deterministic, integer-only
math so it can be ported to the ADSP21xx family or emulated on other targets. The
sampler supports:

- 32 pads with independent sample memory, gain, pan, pitch and trigger enable switches.
- Fixed-point playback with saturating arithmetic and guard bits that emulate the
  accumulator behaviour of the ADSP21xx MAC unit.
- A lightweight voice allocator that can choke pads when their trigger switch is off
  ("pad not trigger" behaviour) while keeping the pad selectable from the UI.
- Utilities to translate floating point authoring data to the fixed-point domain.
- A VST3-facing stub that shows how the processor and controller can be registered in a
  factory. This repository snapshot ships without the full Steinberg VST3 SDK, so the
  stub provides self-contained minimal types that mirror the SDK entry points.

The sampler engine itself is implemented in `include/adsp21xx_drum_engine.h` and
`source/adsp21xx_drum_engine.cpp`. The VST3 stub lives in
`source/adsp21xx_pad32_vst3_stub.cpp`. A tiny command line harness in
`source/adsp21xx_pad32_demo.cpp` renders a few audio frames to illustrate the fixed-point
signal path.

> **Note**
> This repository snapshot does not contain the official Steinberg VST3 headers. The
> provided stub mimics the minimum API surface so the DSP can be exercised and integrated
> later when the real SDK is available.

## Building the demo

```
cmake -S projects/adsp21xx_pad32 -B build/adsp21xx_pad32
cmake --build build/adsp21xx_pad32
```

The resulting `adsp21xx_pad32_demo` executable renders a short buffer to stdout with the
mixed left/right samples in floating point so it can be inspected quickly.
