using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
namespace Flow.Launcher.Plugin.RegFlow;

public class RegistryRoot{
    public const RegistryHive HKEY_CLASSES_ROOT = RegistryHive.ClassesRoot;
    public const RegistryHive HKEY_CURRENT_USER = RegistryHive.CurrentUser;
    public const RegistryHive HKEY_LOCAL_MACHINE = RegistryHive.LocalMachine;
    public const RegistryHive HKEY_USERS = RegistryHive.Users;
    public const RegistryHive HKEY_CURRENT_CONFIG = RegistryHive.CurrentConfig;

    public static readonly Dictionary<string, RegistryHive> RegistryRoots = new Dictionary<string, RegistryHive>(){
        {"HKEY_CLASSES_ROOT", RegistryHive.ClassesRoot},
        {"HKEY_CURRENT_USER", RegistryHive.CurrentUser},
        {"HKEY_LOCAL_MACHINE", RegistryHive.LocalMachine},
        {"HKEY_USERS", RegistryHive.Users},
        {"HKEY_CURRENT_CONFIG", RegistryHive.CurrentConfig}
    };
}

public class RegInfo
{
    public readonly string fullpath;
    public readonly Tuple<string, string, string>[] subValues;
    public readonly string[] subFolders;
    
    protected readonly RegistryKey regkey;

    public RegInfo(string fullpath, RegistryKey key)
    {
        this.fullpath = fullpath;
        this.subValues = key.GetValueNames().Select(
            name => new Tuple<string, string, string>
            (
                name, key.GetValueKind(name).ToString(), (key.GetValue(name) ?? "(null)").ToString() ?? "(null)"
            )
            ).ToArray();
        this.subFolders = key.GetSubKeyNames();
        this.regkey = key;
    }

    public static RegInfo? get(
        string path, 
        RegistryView view = RegistryView.Registry64
    ){
        ///<summary>
        /// Get registry key from path
        ///</summary>

        // split path to array
        // replace all \\ to /
        string replaced = path.Replace("\\", "/");

        string[] path_array = replaced.Split('/');

        // if first is computer, pop
        if (path_array[0] == "Computer")
        {
            path_array = path_array.Skip(1).ToArray();
        }

        // match first index to registry root
        RegistryHive hive = RegistryRoot.RegistryRoots[path_array[0]];
        RegistryKey? key = RegistryKey.OpenBaseKey(hive, view);

        for (int i = 1; i < path_array.Length; i++)
        {
            // if key is null, return null
            if (key == null)
            {
                return null;
            }

            // open registry key
            
            // open subkey
            try{
                key = key.OpenSubKey(path_array[i]);
            }catch(Exception){
                return null;
            }
            
        }
        if (key == null){
            return null;
        }

        // return key
        return new RegInfo(replaced, key);
    }

    public void unsafe_SetValue(string name, string value, RegistryValueKind kind)
    {
        this.regkey.SetValue(name, value, kind);
    }
}