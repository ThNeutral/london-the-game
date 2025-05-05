from PIL import Image, ImageDraw
import itertools

tile_size = 100

basic_values = [0, 255]
colors = list(itertools.product(basic_values, repeat=3))

colors.append((127, 127, 127))

cols = rows = 3
width = tile_size * cols
height = tile_size * rows

image = Image.new("RGB", (width, height), (255, 255, 255))
draw = ImageDraw.Draw(image)

for index, color in enumerate(colors):
    x = (index % cols) * tile_size
    y = (index // cols) * tile_size
    draw.rectangle([ (x, y), (x + tile_size, y + tile_size) ], fill=color)

image.save("../Designs/Generated/colored_tiles_3x3.png")
print("Image saved as 'colored_tiles_3x3.png'")