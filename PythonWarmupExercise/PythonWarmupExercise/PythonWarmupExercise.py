fahrenheit = input("Input a fahrenheit ")

while fahrenheit < 0:
    fahrenheit = input("Input a fahrenheit ")

#converting fahrenheit to celsius, first you lower the fahrenheit with the freezing point of water ( 32 degrees Fahrenheit ) and then you increase it witht the scale of fahrenheit ( 1⁄180 )
celsius = (float(fahrenheit)  -  32)  *  1.8
print 'Celsius is' , round(celsius, 2);

#converting celsius to kelvin,
kelvin = celsius + 273.15
print 'Kelvin is', round(kelvin, 0);

absolute = input("Input a number and then I make it an absolute value ")

#making from the input an absolute number by flipping the input when it's lower then 0
if absolute < 0:
    absolute = -(absolute) 
print absolute