#pragma once

#include <array>
#include <cstdint>
#include <limits>

namespace adsp21xx
{
struct Fixed24
{
    using storage_type = int32_t;
    static constexpr int FractionBits = 23;
    static constexpr storage_type FractionMask = (1u << FractionBits) - 1u;
    static constexpr storage_type One = 1u << FractionBits;
    static constexpr storage_type Min = std::numeric_limits<storage_type>::min() >> (31 - FractionBits);
    static constexpr storage_type Max = std::numeric_limits<storage_type>::max() >> (31 - FractionBits);

    storage_type value {0};

    constexpr Fixed24() = default;
    explicit constexpr Fixed24(storage_type raw) : value(raw) {}

    static constexpr Fixed24 fromFloat(float f) noexcept
    {
        const float scaled = f * static_cast<float>(One);
        const auto raw = static_cast<storage_type>(scaled);
        return Fixed24 {raw};
    }

    constexpr float toFloat() const noexcept
    {
        return static_cast<float>(value) / static_cast<float>(One);
    }

    static constexpr Fixed24 fromInt(int32_t v) noexcept
    {
        return Fixed24 {static_cast<storage_type>(v) << FractionBits};
    }

    static constexpr storage_type rawAdd(storage_type a, storage_type b) noexcept
    {
        const int64_t sum = static_cast<int64_t>(a) + static_cast<int64_t>(b);
        return clamp(sum);
    }

    static constexpr storage_type rawSub(storage_type a, storage_type b) noexcept
    {
        const int64_t diff = static_cast<int64_t>(a) - static_cast<int64_t>(b);
        return clamp(diff);
    }

    static constexpr storage_type rawMul(storage_type a, storage_type b) noexcept
    {
        const int64_t product = static_cast<int64_t>(a) * static_cast<int64_t>(b);
        const int64_t shifted = product >> FractionBits;
        return clamp(shifted);
    }

    static constexpr storage_type rawMac(storage_type acc, storage_type a, storage_type b) noexcept
    {
        const int64_t product = static_cast<int64_t>(a) * static_cast<int64_t>(b);
        const int64_t shifted = product >> FractionBits;
        const int64_t sum = static_cast<int64_t>(acc) + shifted;
        return clamp(sum);
    }

    static constexpr storage_type clamp(int64_t v) noexcept
    {
        const int64_t minVal = static_cast<int64_t>(Min);
        const int64_t maxVal = static_cast<int64_t>(Max);
        return static_cast<storage_type>(v < minVal ? minVal : (v > maxVal ? maxVal : v));
    }

    constexpr Fixed24 operator+(Fixed24 rhs) const noexcept { return Fixed24 {rawAdd(value, rhs.value)}; }
    constexpr Fixed24 operator-(Fixed24 rhs) const noexcept { return Fixed24 {rawSub(value, rhs.value)}; }
    constexpr Fixed24 operator*(Fixed24 rhs) const noexcept { return Fixed24 {rawMul(value, rhs.value)}; }

    constexpr Fixed24& operator+=(Fixed24 rhs) noexcept
    {
        value = rawAdd(value, rhs.value);
        return *this;
    }

    constexpr Fixed24& operator-=(Fixed24 rhs) noexcept
    {
        value = rawSub(value, rhs.value);
        return *this;
    }

    constexpr Fixed24& operator*=(Fixed24 rhs) noexcept
    {
        value = rawMul(value, rhs.value);
        return *this;
    }
};

struct FixedAccumulator
{
    int64_t value {0};

    constexpr void clear() noexcept { value = 0; }

    constexpr void mac(Fixed24 sample, Fixed24 gain) noexcept
    {
        value += static_cast<int64_t>(sample.value) * static_cast<int64_t>(gain.value);
    }

    [[nodiscard]] constexpr Fixed24 asFixed() const noexcept
    {
        const int64_t shifted = value >> Fixed24::FractionBits;
        return Fixed24 {Fixed24::clamp(shifted)};
    }
};

constexpr Fixed24 makePanCoefficient(float pan, bool left) noexcept
{
    const float clipped = pan < -1.f ? -1.f : (pan > 1.f ? 1.f : pan);
    const float gain = left ? (clipped <= 0.f ? 1.f : 1.f - clipped) : (clipped >= 0.f ? 1.f : 1.f + clipped);
    return Fixed24::fromFloat(gain);
}

} // namespace adsp21xx
