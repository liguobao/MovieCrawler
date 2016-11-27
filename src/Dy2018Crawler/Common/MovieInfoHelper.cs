using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dy2018Crawler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dy2018Crawler
{
    public class MovieInfoHelper
    {

        private  ConcurrentDictionary<string, MovieInfo> _cdMovieInfo = new ConcurrentDictionary<string, MovieInfo>();

        private  string _movieJsonFilePath = "";

        /// <summary>
        /// 初始化电影列表
        /// </summary>
        /// <param name="jsonFilePath"></param>
        public  MovieInfoHelper(string jsonFilePath)
        {
            _movieJsonFilePath = jsonFilePath;
            if (!File.Exists(jsonFilePath))
            {
                var pvFile = File.Create(jsonFilePath);
                pvFile.Flush();
                return;

            }
            using (var stream = new FileStream(jsonFilePath, FileMode.OpenOrCreate))
            {
                try
                {
                    StreamReader sr = new StreamReader(stream);
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new JavaScriptDateTimeConverter() }
                    };
                    //构建Json.net的读取流  
                    using (var reader = new JsonTextReader(sr))
                    {
                        var lstMovie = serializer.Deserialize<List<MovieInfo>>(reader);
                        foreach (var movie in lstMovie.GroupBy(m => m.Dy2018OnlineUrl))
                        {
                            if (!_cdMovieInfo.ContainsKey(movie.Key))
                                _cdMovieInfo.TryAdd(movie.Key, movie.FirstOrDefault());
                        }
                    }

                }
                catch (Exception ex)
                {
                    //write 
                }
            }


        }

        /// <summary>
        /// 获取当前的电影列表
        /// </summary>
        /// <returns></returns>
        public  List<MovieInfo> GetListMoveInfo()
        {
            return _cdMovieInfo.Values.OrderByDescending(m=>m.PubDate).ToList();
        }

        /// <summary>
        /// 添加到电影字典（线程安全）
        /// </summary>
        /// <param name="movieInfo"></param>
        /// <returns></returns>
        public  bool AddToMovieDic(MovieInfo movieInfo)
        {
            if (!_cdMovieInfo.ContainsKey(movieInfo.Dy2018OnlineUrl))
            {
                WriteToJsonFile();
                return _cdMovieInfo.TryAdd(movieInfo.Dy2018OnlineUrl, movieInfo);
            }
            return true;
        }

        public  bool IsContainsMoive(string onlieURL)
        {
            return _cdMovieInfo.ContainsKey(onlieURL);
        }


        public MovieInfo GetMovieInfo(String key)
        {
            if (_cdMovieInfo.ContainsKey(key))
                return _cdMovieInfo[key];
            else
                return null;
        }

        /// <summary>
        /// 写入json文件
        /// </summary>
        public  void WriteToJsonFile(bool isWriteNow = false)
        {
            if (_cdMovieInfo.Count % 10 == 0 || isWriteNow)
            {
                using (var stream = new FileStream(_movieJsonFilePath, FileMode.Open))
                {
                    StreamWriter sw = new StreamWriter(stream);
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new JavaScriptDateTimeConverter() }
                    };
                    //构建Json.net的写入流  
                    JsonWriter writer = new JsonTextWriter(sw);
                    //把模型数据序列化并写入Json.net的JsonWriter流中  
                    serializer.Serialize(writer, _cdMovieInfo.Values.OrderBy(m=>m.PubDate).ToList());
                    sw.Flush();
                    writer.Close();
                    
                }
            }
        }

    }
}
