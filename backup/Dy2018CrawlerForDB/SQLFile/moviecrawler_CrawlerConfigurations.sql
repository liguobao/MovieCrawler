-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: owin.jhonge.net    Database: moviecrawler
-- ------------------------------------------------------
-- Server version	5.7.14-1+deb.sury.org~trusty+0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `CrawlerConfigurations`
--

DROP TABLE IF EXISTS `CrawlerConfigurations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CrawlerConfigurations` (
  `Id` char(36) NOT NULL,
  `ConfigconfigurationName` varchar(255) DEFAULT NULL,
  `ConfigconfigurationValue` varchar(512) DEFAULT NULL,
  `IsEnabled` tinyint(1) NOT NULL,
  `DataCreateTime` datetime(6) NOT NULL,
  `ConfigconfigurationKey` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CrawlerConfigurations`
--

LOCK TABLES `CrawlerConfigurations` WRITE;
/*!40000 ALTER TABLE `CrawlerConfigurations` DISABLE KEYS */;
INSERT INTO `CrawlerConfigurations` VALUES ('3fd1df69-1122-4484-280e-08d42f264a85','Dy2018CrawlerList','http://www.dy2018.com/html/bikan/',1,'2016-12-28 21:34:47.874685',100),('d8335348-470d-40cf-280c-08d42f264a85','Dy2018CrawlerList','http://www.dy2018.com/4/',1,'2016-12-28 21:34:47.442069',4),('f7ce4bf4-a993-42a7-6d88-08d42f33d204','Dy2018CrawlerPageCount','Dy2018CrawlerPageCount',1,'2016-12-28 23:11:37.799166',10),('fbe3320e-c84f-40d1-280b-08d42f264a85','Dy2018CrawlerList','http://www.dy2018.com/html/gndy/dyzz/',1,'2016-12-28 21:34:47.091396',2),('fe9bd109-2fc8-4f61-280d-08d42f264a85','Dy2018CrawlerList','http://www.dy2018.com/8/',1,'2016-12-28 21:34:47.671235',8);
/*!40000 ALTER TABLE `CrawlerConfigurations` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-12-28 23:30:43
