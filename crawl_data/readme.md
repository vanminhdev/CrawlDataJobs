1. install
```shell
    pip install attrs
    pip install Scrapy
```
- Trường hợp lỗi attrs
```shell
    pip uninstall attrs
    pip install attrs
```
2. run
```shell
    cd ./crawl_data
    scrapy crawl <name của spider>
```
3. code
- Tạo crawl mới `scrapy genspider -t crawl <my_domain> <mydomain.com>`
    - File sinh ra `<my_domain>_spider.py`
- Viết code vào `./crawl_data/spiders/`