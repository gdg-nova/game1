require 'rubygems'
require 'chunky_png'

include ChunkyPNG

def is_premultiplied(image)
	(0..(image.height - 1)).each do |y|
		(0..(image.width - 1)).each do |x|
			c = image[x, y]

			r = Color.r(c)
			g = Color.g(c)
			b = Color.b(c)
			a = Color.a(c)

			if a != 255
				if r > a or g > b or b > a
					return false
				end
			end
		end
	end

	return true
end

if $0 == __FILE__
	input = ARGV[0]
	if input.nil?
		puts "usage: #{$0} filename"
		exit 1
	end

	image = Image.from_file(input)

	puts "Image #{input}"
	print "Premultiplied Alpha Channel: "

	if is_premultiplied(image)
		puts "YES"
	else
		puts "NO"
	end
end