#include "pch.h"
#include "algorithm.h"
#include <thread>
#include <chrono>

void calculatePixelColor(std::uint8_t& pixel, double contrast){
	int new_value = (pixel - 128) * contrast + 128;

	if (new_value > 255){
		new_value = 255;
	}
	else if (new_value < 0){
		new_value = 0;
	}

	pixel = new_value;
}

void changeContrast(std::uint8_t* pixels, int img_width, int img_height, double contrast, int* local_progress, int start_row, int end_row){
	bool local_progress_ok = (local_progress != nullptr);
	bool pixels_ok = (pixels != nullptr);

	if (!pixels_ok)
		return;

	int stride = img_width * 4;
	int bytes_size = stride * img_height;
	int start_byte = start_row * stride;
	int end_byte = (end_row + 1) * stride - 1;

	std::uint8_t lookup_table[256];

	for (int i = 0; i < 256; i++){
		lookup_table[i] = static_cast<std::uint8_t>(i);
		calculatePixelColor(lookup_table[i], contrast);
	}

	int local_progress_buffer = 0;

	for (int i = start_byte; i <= end_byte; i += 4){
		// BLUE
		if (i < bytes_size){
			pixels[i] = lookup_table[pixels[i]];
			local_progress_buffer++;

			// GREEN
			if (i + 1 < bytes_size){
				pixels[i + 1] = lookup_table[pixels[i + 1]];
				local_progress_buffer++;

				// RED
				if (i + 2 < bytes_size){
					pixels[i + 2] = lookup_table[pixels[i + 2]];
					local_progress_buffer++;

					// ALPHA
					if (i + 3 < bytes_size)
						local_progress_buffer++;
				}
			}
		}

		if (i % 1024 == 0 && local_progress_ok)
			*local_progress = local_progress_buffer;
	}

	if (local_progress_ok)
		*local_progress = local_progress_buffer;
}