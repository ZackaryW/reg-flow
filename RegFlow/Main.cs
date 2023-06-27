using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.RegFlow
{
    public class RegFlow : IPlugin
    {
        private PluginInitContext _context = null!;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        protected Result makeActionableQuery(
            string title,
            string subTitle,
            string icoPath,
            string queryString
        ){
            // remove repeated / in query string
            queryString = queryString.Replace("//", "/");

            return new Result(){
                Title = title,
                SubTitle = subTitle,
                IcoPath = icoPath,
                Action = e =>
                {
                    _context.API.ChangeQuery($"{_context.CurrentPluginMetadata.ActionKeyword} {queryString}");
                    return false;
                }
            };
        }

        public List<Result> defaultResults(){

            return new List<Result>(){
                makeActionableQuery(
                    "HKEY_CLASSES_ROOT",
                    "Registry Root",
                    "Images\\reg.png",
                    "HKEY_CLASSES_ROOT"
                ),
                makeActionableQuery(
                    "HKEY_CURRENT_USER",
                    "Registry Root",
                    "Images\\reg.png",
                    "HKEY_CURRENT_USER"
                ),
                makeActionableQuery(
                    "HKEY_LOCAL_MACHINE",
                    "Registry Root",
                    "Images\\reg.png",
                    "HKEY_LOCAL_MACHINE"
                ),
                makeActionableQuery(
                    "HKEY_USERS",
                    "Registry Root",
                    "Images\\reg.png",
                    "HKEY_USERS"
                ),
                makeActionableQuery(
                    "HKEY_CURRENT_CONFIG",
                    "Registry Root",
                    "Images\\reg.png",
                    "HKEY_CURRENT_CONFIG"
                ),
                
        };
    }




        public List<Result> Query(Query query)
        ///<summary>
        /// Query for registry,
        /// @val for just values
        /// @sub for just subkeys
        ///</summary>
        {
            if (query.Search == "" || query.Search == "computer")
            {
                return defaultResults();
            }

            // query get string
            string queryBaseString = query.Search;
            // split query string by space
            string[] queryArray = queryBaseString.Split(' ');
            if (queryArray.Length >= 2 || (queryArray.Length == 1 && queryArray[0].Contains("@")))
            {
                return new List<Result>(){
                    new Result(){
                        Title = "Error",
                        SubTitle = "Invalid Query",
                        IcoPath = "Images\\error.png"
                    }
                };
            }

            // determine which is query string
            bool includeValues = true;
            bool includeSubKeys = true;
            string queryString;
            if (queryArray.Length == 1)
            {
                // query string is queryArray[0]
                queryString = queryArray[0];
            }
            else if (queryArray[0] == "@val")
            {
                // query string is queryArray[1]
                queryString = queryArray[1];
                includeSubKeys = false;
            }
            else if (queryArray[0] == "@sub")
            {
                // query string is queryArray[1]
                queryString = queryArray[1];
                includeValues = false;
            }
            else if (queryArray[1] == "@val")
            {
                // query string is queryArray[0]
                queryString = queryArray[0];
                includeSubKeys = false;
            }
            else if (queryArray[1] == "@sub")
            {
                // query string is queryArray[0]
                queryString = queryArray[0];
                includeValues = false;
            }
            else
            {
                return new List<Result>(){
                    new Result(){
                        Title = "Error",
                        SubTitle = "Invalid Query",
                        IcoPath = "Images\\error.png"
                    }
                };
            }


            RegInfo? regInfo;
            try{
                regInfo = RegInfo.get(queryString);
                if (regInfo == null){
                    return new List<Result>(){
                        new Result(){
                            Title = "Error",
                            SubTitle = "No Result",
                            IcoPath = "Images\\error.png"
                        }
                    };
                }
            }catch(Exception e){
                return new List<Result>(){
                    new Result(){
                        Title = "Error",
                        SubTitle = e.Message,
                        IcoPath = "Images\\error.png"
                    }
                };
            }


            if (regInfo.subFolders.Length == 0 && regInfo.subValues.Length == 0)
            {
                return new List<Result>(){
                    new Result(){
                        Title = "No Result",
                        SubTitle = "No Result",
                        IcoPath = "Images\\error.png"
                    }
                };
            }

            List<Result> results = new List<Result>();
            
            if (includeSubKeys)
            {
                foreach (string subFolder in regInfo.subFolders)
                {
                    results.Add(
                        makeActionableQuery(
                            subFolder,
                            "Registry Subkey",
                            "Images\\folder.png",
                            $"{queryBaseString}/{subFolder} "
                        )
                    );
                }
            }

            if (includeValues)
            {
                foreach (Tuple<string, string, string> subValue in regInfo.subValues)
                {
                    results.Add(new Result()
                    {
                        Title = subValue.Item3,
                        SubTitle = "[" + subValue.Item2 + "] " + subValue.Item1,
                        IcoPath = "Images\\data.png",
                    });
                }
            }

            return results;
        }
    }
}