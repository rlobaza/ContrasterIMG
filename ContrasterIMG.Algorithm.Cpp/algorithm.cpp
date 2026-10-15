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

void changeContrast(std::uint8_t* pixels, int img_width, int img_height, int no_threads, double contrast, int* local_progress, int start_row, int end_row)
{
	int stride = img_width * 4;
	int pixels_size = stride * img_height;
	int start_byte = start_row * stride;
	int end_byte = (end_row + 1) * stride - 1;

	for (int i = start_byte; i <= end_byte; i += 4)
	{
		// B
		if (i < pixels_size)
		{
			calculatePixelColor(pixels[i], contrast);
			(*local_progress)++;

			// G
			if (i + 1 < pixels_size)
			{
				calculatePixelColor(pixels[i + 1], contrast);
				(*local_progress)++;

				// R
				if (i + 2 < pixels_size)
				{
					calculatePixelColor(pixels[i + 2], contrast);
					(*local_progress)++;

					// A
					if (i + 3 < pixels_size)
					{
						(*local_progress)++;
					}
				}
			}
		}
	}
}