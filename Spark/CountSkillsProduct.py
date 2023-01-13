from pyspark.sql import SparkSession
import pyspark
from collections import Counter
import json
import findspark
findspark.init()

# spark = SparkSession.builder.master("local[1]").appName("test").getOrCreate()
spark = SparkSession.builder.getOrCreate()

rdd = spark.read.option("multiline", "true").json("Jobs.json").filter("company.companyType == 'Sản phẩm'")
# rdd.show(1) #hiện 1 dòng

dfSkills = rdd.select("skills")
skills = []
for skill in dfSkills.collect():
    # print(skill.asDict())
    skills = skills + skill.asDict()['skills']

# print(skills)
# Đếm kỹ năng nào đang yêu cầu nhiều
countSkills = Counter(skills).items()
countSkillsSorted = sorted(countSkills, key=lambda x: x[1], reverse=True)
#print(countSkillsSorted)

dicSkills = []
for item in countSkillsSorted:
    dicSkills.append({ "name": item[0], "count": item[1] })
f = open("CountSkillsProduct.json", "w")
f.write(json.dumps(dicSkills))

