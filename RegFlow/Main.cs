using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.RegFlow
{
    public class RegFlow : IPlugin
    {
        private PluginInitContext _context = null!;
        private bool _includeValues = true;
        private bool _includeSubKeys = true;

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

            if (query.FirstSearch == query.Search){
                _includeSubKeys = true;
                _includeValues = true;
            }

            if (query.Search == "" || query.Search == "computer")
            {
                return defaultResults();
            }

            // query get string
            string queryBaseString = query.Search;

            // determine which is query string
            bool includeValues = _includeValues;
            bool includeSubKeys = _includeSubKeys;
            string queryString = queryBaseString;
            if (queryBaseString.Contains("@") && queryBaseString.StartsWith("@"))
            {
                string[] queryArray = queryBaseString.Split(' ', 1);
                if (queryArray[0] == "@val"){
                    includeSubKeys = false;
                }
                if (queryArray[0] == "@sub"){
                    includeValues = false;
                }
                queryString = queryArray[1]; 
            }

            //!SECTION
            queryString = queryString.Replace("//", "/");
            queryString = queryString.Replace("\\", "/");

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
            // add return
            if (queryString.Contains("/"))
            {
            

                results.Add(
                    makeActionableQuery(
                        "Return",
                        "Return to previous folder",
                        "Images\\return.png",
                        queryString.Substring(0, queryString.LastIndexOf('/'))
                    )
                );
            }

            if (includeSubKeys)
            {
                foreach (string subFolder in regInfo.subFolders)
                {
                    results.Add(
                        makeActionableQuery(
                            subFolder,
                            "Registry Subkey",
                            "Images\\folder.png",
                            $"{queryString}/{subFolder}" 
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