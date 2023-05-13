# Script intended for Model Researcher Pro
# Reads TRD3 col.col file for boundaries

import mrp

f = mrp.get_bfile()
verts = []
faces = []

f.seek(0x1C)
face_count = f.readInt()

f.seek(0x20)
vert_count = f.readInt()

f.seek(0x28)

for i in range(0, vert_count):
    x = float(f.readShort()) / 16777215.0
    y = float(f.readShort()) / 16777215.0
    z = float(f.readShort()) / 16777215.0
    yscale = float(f.readByte() * 65536.0) / 16777215.0
    zscale = float(f.readByte() * 65536.0) / 16777215.0
    verts.append(((y + yscale) * 1000, x * 1000, (z + zscale) * 1000)) # Rescale x1000 for Model Researcher

f.seek(0x28 + (vert_count * 0x08) + (face_count * 4))

for i in range(0, face_count):
    f1 = f.readShort()
    f2 = f.readShort()
    f3 = f.readShort()

    faces.append((f1, f2, f3))
    
mesh = mrp.get_mesh()
mesh.set_vertices(verts)
mesh.set_faces(faces)
mrp.render()
