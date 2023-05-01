with open("MockedCameraImageJPEG.jpeg", "rb") as image:
    f = image.read()
    b = bytearray(f)

print(b[0])
