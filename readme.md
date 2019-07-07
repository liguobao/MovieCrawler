# 电影资源爬虫

## 已支持的网站

- www.btbtdy.tv
- 

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

每次运行一个爬虫任务, 通过CRAWL_NAME环境变量控制, 爬取结果直接写入MySQL.

## api

- REST API风格

## 数据库

- 执行db.sql创建数据库
- 默认使用本地数据库

```log
server=127.0.0.1;port=3306;database=movie_map;uid=root;pwd=123;Allow User Variables=True;Connection Timeout=30;SslMode=None;Charset=utf8mb4;MaxPoolSize=1000;
```