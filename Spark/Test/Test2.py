from unidecode import unidecode

address = 'Nguyễn Trãi, Thanh Xuân, Hà Nội'
lst = address.split(',')

print(unidecode(lst[-1].strip()))
print(unidecode(lst[-2].strip()))