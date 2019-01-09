# 电影爬虫

## dotnet core环境准备

- [安装dotnet core SDK](https://www.microsoft.com/net/download/)

- [安装 visual studio code](https://code.visualstudio.com/)

## 运行程序

```sh
dotnet run;
```

或者直接使用VS code debug

## 项目文件说明

### src 项目代码

#### Crawler

爬虫代码

- BaseCrawler 通用父类,实现启动爬虫,获取爬虫配置,存储爬取回来的数据

- 其他子类需要重写BaseCrawler LoadHTML方法实现不同的加载网页需求, 同时也需要重写 ParseMovies实现从HTML/Json中提取数据

- 子类实现后需要在Startup.cs中注明BaseCrawler的实现类为xxx,参考现有代码声明一下即可.

#### Controllers

MVC中的Controller,继承于Controller,REST API或者普通Controller都在这里面

####  Service

逻辑层代码,业务逻辑都应该封装在这里

#### Models

MVC中的M,一般是View中使用的Model,不复杂的话DBModel和ViewModel可以共用

####  appsettings.json

配置文件,数据库连接字符串/邮箱账号密码都在里面

####  Program.cs

程序主入口,指定端口号之类的直接在此处bind

####  Startup.cs

各种启动环境 + 依赖注入配置 + 各种中间件注入

- ConfigureServices 依赖注入

- Configure 中间件引入

### test 单元测试
