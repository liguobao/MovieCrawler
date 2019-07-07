CREATE SCHEMA `movie_map` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci ;



CREATE TABLE `movie` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Intro` varchar(4000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Cover` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Link` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Type` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PublishTime` timestamp NULL DEFAULT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Resources` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_link` (`Link`),
  KEY `idx_name` (`Name`),
  KEY `idx_type` (`Type`),
  KEY `idx_create_time` (`CreateTime`),
  KEY `idx_pub` (`PublishTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
