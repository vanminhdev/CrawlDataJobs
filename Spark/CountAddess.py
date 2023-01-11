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
#print(dfAdress.collect())
addressArr = []
for address in dfAdress.collect():
    #print(address.asDict()['address'])
    lst = address.asDict()['address'].split(',')
    city = lst[-1].strip()
    if len(lst) >= 2:
        district = lst[-2].strip()
    addressArr.append(city + ',' + district)

countAddress = Counter(addressArr).items()
countAddressSorted = sorted(countAddress, key=lambda x: x[1], reverse=True)

#print(countAddress)

countHaNoi = list(filter(lambda x: 'Ha Noi' in x[0], countAddressSorted))
print(countHaNoi)
countHoChiMinh = list(filter(lambda x: 'Ho Chi Minh' in x[0], countAddressSorted))
print(countHoChiMinh)
countDaNang = list(filter(lambda x: 'Da Nang' in x[0], countAddressSorted))
print(countDaNang)