using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System.Linq;
using System.Reflection;
using RestSharp;
using MovieCrawler.API.Model;

namespace MovieCrawler.API.Service
{

    public class ElasticService
    {
        private readonly AppSettings _config;
        private readonly ElasticClient _elasticClient;

        public ElasticService(IOptions<AppSettings> configuration)
        {
            _config = configuration.Value;
            _elasticClient = InitElasticClient(configuration.Value);
        }


        private ElasticClient InitElasticClient(AppSettings config)
        {
            var connSettings = new ConnectionSettings(new Uri(config.ESURL));
            if (!string.IsNullOrEmpty(config.ESUserName)
            && !string.IsNullOrEmpty(config.ESPassword))
            {
                connSettings.BasicAuthentication(config.ESUserName, config.ESPassword);
            }
            connSettings.DisableDirectStreaming();
            return new ElasticClient(connSettings);
        }



        public List<MovieDetail> Query(string keyword, int page = 0, int size = 1000)
        {
            var searchRsp = _elasticClient.Search<MovieDetail>(s => s
            .Index("movies-*")
            .Explain()
            .From(page * size)
            .Size(size)
            .Sort(sort => sort.Descending(h => h.UpdateTime))
            .Query(q => ConvertToQuery(keyword, q))
            );
            if (searchRsp.IsValid)
            {
                return searchRsp.Documents
                .GroupBy(h => h.Link).Select(items => items.FirstOrDefault()).ToList();
            }
            else
            {
                // LogHelper.Info(searchRsp.DebugInformation);
            }
            return new List<MovieDetail>();
        }

        private static QueryContainer ConvertToQuery(string keyword, QueryContainerDescriptor<MovieDetail> q)
        {
            List<string> keywords = GetKeywords(keyword);
            var qcList = keywords.Select(k => ConvertToQueryContainer(k)).ToArray();
            var query = q.Bool(b => b.Should(qcList).MinimumShouldMatch(1));
            return query;
        }



        private static List<string> GetKeywords(string keyword)
        {
            var keywords = new List<string>();
            if (keyword.Contains(","))
            {
                keywords = keyword.Split(',').ToList();
            }
            else if (keyword.Contains("|"))
            {
                keywords = keyword.Split('|').ToList();
            }
            else
            {
                keywords.Add(keyword);
            }
            return keywords;
        }

        private static QueryContainer ConvertToQueryContainer(string word)
        {
            return new QueryContainerDescriptor<MovieDetail>().MatchPhrase(m => m.Field(p => p.Intro).Query(word));
        }

        public void Save(List<MovieDetail> movies)
        {
            if (movies == null || !movies.Any())
            {
                return;
            }
            var movieIndex = "movies-data";
            var index = _elasticClient.IndexExists(movieIndex);
            if (!index.Exists && index.IsValid)//判断索引是否存在和有效
            {
                CreateIndex(movieIndex);
                CreateMapping(movieIndex);
            }
            //批量创建索引和文档
            IBulkResponse bulkRs = _elasticClient.IndexMany(movies, movieIndex);
            if (bulkRs.Errors)//如果异常
            {
                // LogHelper.Info("SaveHouses error,index:" + houseIndex + ",DebugInformation:" + bulkRs.DebugInformation);
            }
        }



        private void CreateMapping(string index)
        {
            var client = new RestClient($"{_config.ESURL}/{index}/MovieDetail/_mapping");
            var request = new RestRequest(Method.PUT);

            var mappingData = new
            {
                properties = new
                {
                    Name = new
                    {
                        type = "text"
                    },
                    Intro = new
                    {
                        type = "text",
                        analyzer = "ik_max_word",
                        search_analyzer = "ik_max_word"
                    },
                    Cover = new
                    {
                        type = "text"
                    },
                    Link = new
                    {
                        type = "text"
                    },
                    UpdateTime = new
                    {
                        type = "text"
                    },
                    Resources = new
                    {
                        type = "text",
                        fields = new
                        {
                            keyword = new
                            {
                                type = "keyword",
                                ignore_above = 256
                            }
                        }
                    }
                }
            };
            request.AddParameter("application/json", mappingData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        private void CreateIndex(string index)
        {
            var client = new RestClient($"{_config.ESURL}/{index}");
            var request = new RestRequest(Method.PUT);
            IRestResponse response = client.Execute(request);
        }
    }



}