# 电影资源爬虫

## 已支持的网站

- www.btbtdy.tv
- 其他的慢慢补

## 架构部分

- 爬虫部分:.NET Core Console
- REST API: .NET Core Web API
- UI : vue.js
- 数据库: MySQL 5.7

## crawler

- Crawlers: 现有的爬虫都在此
- Dapper: 数据库访问层
- AppSettings.cs :各种配置项
- Program.cs 程序启动文件

1. 每次运行一个爬虫任务, 通过CRAWL_NAME环境变量控制, 爬取结果直接写入MySQL.
2. 需要爬取的网站相关页面在appsettings.json里面配置, 运行的时候会读取的

### 支持一个新网站需要做的事情
- 集成BaseCrawler, 重写LoadHTML和ParseMovies两个方法
- 将实现的XXCrawler声明成BaseCrawler的实现类
- 在appsettings里面心如XXCrawler的同名配置, 配置大概长下面这样, name为类名小写, hosts为需要爬取的页面.

```json
    {
      "name": "dy2018",
      "hosts": [
        "https://www.dy2018.com/"
      ]
    }
```


## api

- REST API风格

## 数据库

- 执行db.sql创建数据库
- 默认使用本地数据库

```log
server=127.0.0.1;port=3306;database=movie_map;uid=root;pwd=123;Allow User Variables=True;Connection Timeout=30;SslMode=None;Charset=utf8mb4;MaxPoolSize=1000;
```

## docker支持

```sh
docker run -v ~/docker-data/movie-crawler/appsettings.json:/app/appsettings.json -e CRAWL_NAME=dy2018 --name movie-crawler  -d movie-crawler-dy2018

docker run -p 10900:80 -v ~/docker-data/movie-crawler/appsettings.json:/app/appsettings.json --restart=always --name movie-crawler-api  -d movie-crawler-api
```


