from PIL import Image, ImageDraw

tile_size = 100
colors = [(0, 255, 0),   # Green
          (255, 0, 0),   # Red
          (0, 0, 255)]   # Blue

width = tile_size * len(colors)
height = tile_size

image = Image.new("RGB", (width, height), (255, 255, 255))
draw = ImageDraw.Draw(image)

for i, color in enumerate(colors):
    top_left = (i * tile_size, 0)
    bottom_right = ((i + 1) * tile_size, tile_size)
    draw.rectangle([top_left, bottom_right], fill=color)

image.save("../Designs/Generated/colored_tiles.png")
print("Image saved as 'colored_tiles.png'")