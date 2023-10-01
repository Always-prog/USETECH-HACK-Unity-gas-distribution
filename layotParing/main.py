import cv2 as cv
import numpy as np
from stl import mesh

from utils import areaFilter

im = cv.imread('layot.jpg')

# Convert to float and divide by 255:
imgFloat = im.astype(np.float) / 255.

# Calculate channel K:
kChannel = 1 - np.max(imgFloat, axis=2)

# Convert back to uint 8:
kChannel = (255 * kChannel).astype(np.uint8)

binaryThresh = 190
_, binaryImage = cv.threshold(kChannel, binaryThresh, 255, cv.THRESH_BINARY)

# Filter small blobs:
minArea = 100
binaryImage = areaFilter(minArea, binaryImage)

# Use a little bit of morphology to clean the mask:
# Set kernel (structuring element) size:
kernelSize = 3
# Set morph operation iterations:
opIterations = 2
# Get the structuring element:
morphKernel = cv.getStructuringElement(cv.MORPH_RECT, (kernelSize, kernelSize))
# Perform closing:
binaryImage = cv.morphologyEx(binaryImage, cv.MORPH_CLOSE, morphKernel, None, None, opIterations, cv.BORDER_REFLECT101)

# To SVG
width = im.shape[0]
height = im.shape[1]


contours, hierarchy = cv.findContours(binaryImage, cv.RETR_TREE, cv.CHAIN_APPROX_SIMPLE)

svg_file = open('contours.svg', 'w+')
svg_file.write(f'<svg width="{width}" height="{height}" xmlns="http://www.w3.org/2000/svg">\n')

for contour in contours:
    path_data = 'M'
    for point in contour:
        x, y = point[0]
        path_data += f'{x} {y} '
    svg_file.write(f'<path d="{path_data}" /> \n')

svg_file.write('</svg>')
svg_file.close()

height3d = 10.0  # Высота в метрах

faces = []
for contour in contours:
    vertices = []
    for point in contour:
        x, y = point[0]
        vertices.append([x, y, height3d])

    if len(vertices) >= 3:
        for i in range(1, len(vertices) - 1):
            face = [vertices[0], vertices[i], vertices[i + 1]]
            faces.append(face)

mesh_3d = mesh.Mesh(np.zeros(len(faces), dtype=mesh.Mesh.dtype))
for i, face in enumerate(faces):
    for j in range(3):
        mesh_3d.vectors[i][j] = face[j]

mesh_3d.save('output.stl')
cv.imwrite('result.png', binaryImage)