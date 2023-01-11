from collections import Counter

array = ['ReactJS', 'Python', 'Database', 'Data Analyst', 'React Native', 'Android', 'iOS', 'Python', 'Django', 'AWS', 'Java', 'NodeJS', 'Python', 'Linux', 'English', 'Fresher Accepted', 'Python', 'Django', 'JavaScript', '.NET', 'C#', 'ASP.NET', 'QA QC', 'Team Leader', 'C++', 'Python', 'C language', 'Fresher Accepted', 'Java', '.NET', 'Golang']

countArray = Counter(array).items()

countArraySorted = sorted(countArray, key=lambda x: x[1], reverse=True)
print(countArraySorted)