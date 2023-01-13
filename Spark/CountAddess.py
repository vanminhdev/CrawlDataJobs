from unidecode import unidecode
from pyspark.sql import SparkSession
import pyspark
from collections import Counter
import json
import findspark
findspark.init()

# spark = SparkSession.builder.master("local[1]").appName("test").getOrCreate()
spark = SparkSession.builder.getOrCreate()

rdd = spark.read.option("multiline", "true").json("Jobs.json")
# rdd.show(1) #hiện 1 dòng

dfAdress = rdd.select("company.address")
# print(dfAdress.collect())
addressArr = []
for address in dfAdress.collect():
    # print(address.asDict()['address'])
    lst = address.asDict()['address'].split(',')
    city = unidecode(lst[-1].strip())
    if len(lst) >= 2:
        district = unidecode(lst[-2].replace('Quan', '').replace('Quận', '').strip())
    addressArr.append(city + ',' + district)

countAddress = Counter(addressArr).items()
countAddressSorted = sorted(countAddress, key=lambda x: x[1], reverse=True)

# print(countAddress)

countHaNoi = list(filter(lambda x: 'Ha Noi' in x[0], countAddressSorted))
resultHaNoi = []
for item in countHaNoi:
    split = item[0].split(',')
    resultHaNoi.append({'quan': split[1], 'count': item[1]})
f = open("CountAddressHaNoi.json", "w")
f.write(json.dumps(resultHaNoi))
# print(countHaNoi)

countHoChiMinh = list(
    filter(lambda x: 'Ho Chi Minh' in x[0], countAddressSorted))
resultHoChiMinh = []
for item in countHoChiMinh:
    split = item[0].split(',')
    resultHoChiMinh.append({'quan': split[1], 'count': item[1]})
f = open("CountAddressHoChiMinh.json", "w")
f.write(json.dumps(resultHoChiMinh))
# print(countHoChiMinh)

countDaNang = list(filter(lambda x: 'Da Nang' in x[0], countAddressSorted))
resultDaNang = []
for item in countDaNang:
    split = item[0].split(',')
    resultDaNang.append({'quan': split[1], 'count': item[1]})
f = open("CountAddressDaNang.json", "w")
f.write(json.dumps(resultDaNang))
# print(countDaNang)
