### dy2018电影天堂爬虫

在线地址：[http://codelover.win/](http://codelover.win/)

详细介绍在知乎专栏：[手把手教你用.NET Core写爬虫](https://zhuanlan.zhihu.com/p/24151412)
现有两个版本：
[src/Dy2018Crawler](https://github.com/liguobao/Dy2018Crawler/tree/master/src/Dy2018Crawler)：此版本全部数据缓存于程序中，定期保存到json文件。
[src/Dy2018CrawlerForDB](https://github.com/liguobao/Dy2018Crawler/tree/master/src/Dy2018CrawlerForDB):此版本全部数据落地到Mysql数据库中，运行时时请确保数据库连接字符串的正确性。


//还原各种包文件
dotnet restore;
//发布到C:\code\website\Dy2018Crawler文件夹
dotnet publish -r ubuntu.14.04-x64 -c Release -o "C:\code\website\Dy2018Crawler";

