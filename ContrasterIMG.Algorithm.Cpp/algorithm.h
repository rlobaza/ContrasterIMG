#pragma once

#include <cstdint>

void calculatePixelColor(std::uint8_t& pixel, double contrast);

extern "C" __declspec(dllexport)
void changeContrast(std::uint8_t* pixels, int img_width, int img_height, int no_threads, double contrast, int* local_progress, int start_row, int end_row);