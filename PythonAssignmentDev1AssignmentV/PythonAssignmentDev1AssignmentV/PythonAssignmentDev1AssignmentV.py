inputString = raw_input('Hello put in a string that I can revert ')

reverse = ''

for i in range(0, len(inputString)):
    b = len(inputString) - (i + 1)
    reverse += inputString[b]

print reverse

# Alternatives
#print inputString[len(inputString)::-1]
#print inputString[::-1]

password = raw_input('Fill in your password ')
spaces = 0
specialCases = 0
specialKeys = ['^','[','~','!','@','#','$','%','^','&','*','(',')','_','+','{','}','"',':',';',']','+','$']


for i in range(0, len(password)):
    if password[i] == ' ':
        spaces += 1
    elif password[i].islower():
        specialCases += 1
    elif password[i].isupper():
        specialCases += 1
    elif password[i].isdigit():
        specialCases += 1
    else:
        for c in range(0, len(specialKeys)):
            if password[i] == specialKeys[c]:
                specialCases += 1

if spaces > 2:
    print 'Strong Password'
elif specialCases > 3:
    print 'Medium Password'
else:
    print 'Weak Password'

string = raw_input('Fill in a text ')
offset = input('Fill in a number ')
converted = ''

for n in range(0, len(string)):
    if string[n].isalpha():
        if string[n].isupper():
            start = 65
        else:
            start = 97
        num = ord(string[n]) - start
        num = start + ((num + offset) % 26) # 26 is for all the letters in the alphabet, if higher > 26 takes the modulo
    else:
        num = ord(string[n])
    converted += chr(num)

print converted