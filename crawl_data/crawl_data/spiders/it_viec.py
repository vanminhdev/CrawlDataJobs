import scrapy
from scrapy.linkextractors import LinkExtractor
from scrapy.spiders import CrawlSpider, Rule


class ItViecSpider(CrawlSpider):
    name = 'it_viec'
    allowed_domains = ['itviec.com']
    start_urls = ['http://itviec.com/viec-lam-it']

    rules = (
        Rule(LinkExtractor(allow=r'Items/'), callback='parse_item', follow=True),
    )

    def parse_item(self, response):
        #for url in 
        self.logger.warning('No item received for %s', response.url)
        with open('index.html', 'wb') as f:
            f.write(response.body)
        item = {}

        #item['domain_id'] = response.xpath('//input[@id="sid"]/@value').get()
        #item['name'] = response.xpath('//div[@id="name"]').get()
        #item['description'] = response.xpath('//div[@id="description"]').get()
        return item
