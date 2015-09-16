size = input('What size has the piramide to be? ');


for i in range(0, size):
    if i == 0:
        print " " * size + "#"
    else:
        print " " * ((size) - i) + i * "#" + "#" * (i + 1)