from pyspark.sql import SparkSession
import pyspark
from collections import Counter
import json
import findspark
findspark.init()

# spark = SparkSession.builder.master("local[1]").appName("test").getOrCreate()
spark = SparkSession.builder.getOrCreate()

rdd = spark.read.option("multiline", "true").json("Jobs.json").filter("startSalary != null")
# rdd.show(1) #hiện 1 dòng

dfSkills = rdd.select("skills")
print(dfSkills.collect())

