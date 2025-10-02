cmake_minimum_required(VERSION 3.20)
project(adsp21xx_pad32 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

add_library(adsp21xx_pad32 STATIC
    source/adsp21xx_drum_engine.cpp
)

target_include_directories(adsp21xx_pad32 PUBLIC include)

target_compile_definitions(adsp21xx_pad32 PUBLIC ADSP21XX_PAD_COUNT=32)

add_executable(adsp21xx_pad32_demo
    source/adsp21xx_pad32_demo.cpp
    source/adsp21xx_pad32_vst3_stub.cpp
)

target_link_libraries(adsp21xx_pad32_demo PRIVATE adsp21xx_pad32)
