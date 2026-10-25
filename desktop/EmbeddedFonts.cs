using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;

public static class EmbeddedFonts {
    static readonly PrivateFontCollection collection=new PrivateFontCollection();
    static readonly Dictionary<string,FontFamily> families=new Dictionary<string,FontFamily>();
    static bool loaded;
    static void Load(){if(loaded)return;loaded=true;foreach(string name in new[]{"PlexSansRegular","PlexSansBold","PlexMono","InterRegular","InterBold","RobotoRegular"})using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream(name)){
        if(stream==null)continue;var data=new byte[stream.Length];int read=0;while(read<data.Length){int n=stream.Read(data,read,data.Length-read);if(n==0)break;read+=n;}
        // GDI+ AddMemoryFont silently drops families when several independent fonts
        // are added on this Windows build. AddFontFile loads the same embedded bytes
        // reliably; the private cache is not a Windows font installation.
        string digest;using(var hash=SHA256.Create())digest=BitConverter.ToString(hash.ComputeHash(data)).Replace("-","").Substring(0,12);
        string folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"RelogioIA-Publicacao","fonts");Directory.CreateDirectory(folder);
        string path=Path.Combine(folder,name+"-"+digest+".ttf");if(!File.Exists(path))File.WriteAllBytes(path,data);collection.AddFontFile(path);
    }foreach(var family in collection.Families){families[family.Name]=family;if(family.Name.StartsWith("Inter"))families["Inter"]=family;}}
    public static Font Make(string name,float size,FontStyle style){if(name.StartsWith("IBM Plex")||name=="Inter"||name=="Roboto"){Load();FontFamily family;if(families.TryGetValue(name,out family))return new Font(family,size,style,GraphicsUnit.Pixel);}return new Font(name,size,style,GraphicsUnit.Pixel);}
}
