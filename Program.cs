using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using System;
using System.Collections.Generic;

namespace Searchfight
{
    class Program
    {
        static readonly List<string> EngineSearchList = new List<string>();
        static readonly List<string> KeywordList = new List<string>();
        static List<Search> SearchList = new List<Search>();

        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (!ProcessArguments(args))
            {
                Console.WriteLine("There is a problem with the arguments");
                return;
            }

            PopulateEngineSearchList();
            ProcessSearch();
            ShowResults();

            Console.ReadLine();
        }

        static void PopulateEngineSearchList()
        {
            EngineSearchList.Add("Google");
            EngineSearchList.Add("Bing");
        }

        static bool ProcessArguments(string[] args)
        {
            //First position is solution dll
            for (int i = 1; i < args.Length; i++)
            {
                KeywordList.Add(args[i]);
            }

            //buscar en cada posición si existe un espacio en blanco con eso sabremos que ha tenido comillas

            if (KeywordList.Count == 0)
            {
                Console.WriteLine("Arguments are required");
                return false;
            }

            if (KeywordList.Count < 2)
            {
                Console.WriteLine("At least two arguments are required");
                return false;
            }

            return true;
        }

        static void ShowResults()
        {
            foreach (var keywordItem in KeywordList)
            {
                Console.Write(keywordItem + ": ");
                foreach (var searchItem in SearchList)
                {
                    if (keywordItem == searchItem.Query)
                    {
                        Console.Write(searchItem.SearchEngine + ": " + searchItem.Count + " ");
                    }
                }
                Console.WriteLine();
            }

            long mayor;
            string queryWinner;

            for (int i = 0; i < EngineSearchList.Count; i++)
            {
                mayor = 0;
                queryWinner = "";
                for (int j = 0; j < SearchList.Count; j++)
                {
                    long temp = long.Parse(SearchList[j].Count);
                    if (EngineSearchList[i] == SearchList[j].SearchEngine && temp > mayor)
                    {
                        mayor = temp;
                        queryWinner = SearchList[j].Query;
                    }
                }
                Console.WriteLine(EngineSearchList[i] + " winner: " + queryWinner);
            }

            long totalWinner = 0;
            long tempTotalWinner;
            int posWinner = 0;
            string queryTotalWinner = "";
            for (int i = 0; i < KeywordList.Count; i++)
            {
                tempTotalWinner = 0;
                for (int j = 0; j < SearchList.Count; j++)
                {
                    long temp = long.Parse(SearchList[j].Count);
                    if (KeywordList[i] == SearchList[j].Query)
                    {
                        tempTotalWinner += temp;
                        posWinner = i;
                    }
                }

                if (tempTotalWinner > totalWinner)
                {
                    totalWinner = tempTotalWinner;
                    queryTotalWinner = KeywordList[posWinner];
                }
            }

            Console.WriteLine("Total winner: " + queryTotalWinner);
        }

        static void SearchGoogle(string query)
        {
            try
            {
                string apikey = "AIzaSyDs_0plXnFdk6Ns6KfMQcY7bUXeElGYPrs";
                string searchEngineID = "c6d5f90b0b3bb4e99";
                var customerSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = apikey, ApplicationName = "Searchfight" });
                CseResource.ListRequest listRequest = customerSearchService.Cse.List();
                listRequest.Cx = searchEngineID;
                listRequest.Q = query;

                var count = listRequest.Execute().SearchInformation.TotalResults;
                AddSearch(query, "Google", count);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + " - " + e.StackTrace);
                throw;
            }
        }

        static void SearchBing(string query)
        {
            Random r = new Random();
            var count = r.Next(1, 1430000000).ToString();
            AddSearch(query, "Bing", count);
        }

        static void AddSearch(string query, string searchEngine, string count)
        {
            Search s = new Search();
            s.Query = query;
            s.SearchEngine = searchEngine;
            s.Count = count;
            SearchList.Add(s);
        }

        static void ProcessSearch()
        {
            for (int i = 0; i < KeywordList.Count; i++)
            {
                SearchGoogle(KeywordList[i]);
                SearchBing(KeywordList[i]);
            }
        }
    }

    class Search
    {
        public string Query { get; set; }
        public string SearchEngine { get; set; }
        public string Count { get; set; }
    }
}
