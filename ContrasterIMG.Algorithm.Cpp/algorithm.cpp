#include "pch.h"
#include "algorithm.h"
#include <thread>
#include <chrono>

void calculatePixelColor(std::uint8_t& pixel, double contrast)
{
	int new_value = (pixel - 128) * contrast + 128;

	if (new_value > 255)
	{
		new_value = 255;
	}
	else if (new_value < 0)
	{
		new_value = 0;
	}

	pixel = new_value;
}

void changeContrast(std::uint8_t* pixels, int img_width, int img_height, int no_threads, double contrast, int* progress)
{
	int stride = img_width * 4;
	int pixels_size = stride * img_height;

	for (int i = 0; i < pixels_size; i += 4)
	{
		// B
		if (i < pixels_size)
		{
			calculatePixelColor(pixels[i], contrast);
			*progress = i + 1;
		}
		// G
		if (i < pixels_size)
		{
			calculatePixelColor(pixels[i + 1], contrast);
			*progress = i + 2;
		}
		// R
		if (i < pixels_size)
		{
			calculatePixelColor(pixels[i + 2], contrast);
			*progress = i + 3;
		}
		if (i < pixels_size)
		{
			*progress = i + 4;
		}
	}
}