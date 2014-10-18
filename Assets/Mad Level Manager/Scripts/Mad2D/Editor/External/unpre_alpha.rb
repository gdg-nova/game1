require 'rubygems'
require 'chunky_png'
require 'pre_alpha_check.rb'

include ChunkyPNG

input = ARGV[0]
output = ARGV[1]

if output.nil?
	puts "Usage: #{$0} input_file output_file"
	exit 1
end

image = Image.from_file(input)

unless is_premultiplied(image)
	puts "Image not premultiplied!"
	exit 1
end

(0..(image.height - 1)).each do |y|
	(0..(image.width - 1)).each do |x|
		c = image[x, y]
		af = Color.a(c) / 255.0
		if (af != 0)
			nc = Color.rgba(
				(Color.r(c) / af).to_i,
				(Color.g(c) / af).to_i,
				(Color.b(c) / af).to_i,
				Color.a(c))

			image[x, y] = nc
		end
	end
end

image.save(output);