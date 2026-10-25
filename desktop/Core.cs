using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

public static class Data {
    public static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = 6000000 };
    public static Dictionary<string,object> Map(object value) { return value as Dictionary<string,object> ?? new Dictionary<string,object>(); }
    public static object Get(Dictionary<string,object> map,string key) { object value; return map.TryGetValue(key,out value)?value:null; }
    public static double? Number(object value) { if(value==null || value is bool)return null; try { double n=Convert.ToDouble(value,CultureInfo.InvariantCulture);return Double.IsNaN(n)||Double.IsInfinity(n)?(double?)null:n; } catch{return null;} }
    public static string Hex(object value) { string s=value as string;return s!=null && System.Text.RegularExpressions.Regex.IsMatch(s,"^#[a-fA-F0-9]{6}$")?s:""; }
}

public class ClockSettings {
    public string Provider="codex";
    public string Theme="winamp", Accent="", TextColor="", Font="mono", ClockStyle="segments", Background="", Atlas="";
    public bool ShowMusic=true,AlertsEnabled=false;public int AlertRemaining=10;public string AlertSound="";
    public bool ShowClock=true, Expanded=false, TopMost=true, TintDigits=false;
    public List<string> Metrics=new List<string>{"cpu","gpu","ram"};
    public double Fade=0;
    public int Width=300, RefreshSeconds=15;
    public int? X=null,Y=null;
    public static ClockSettings Parse(string raw) {
        var s=new ClockSettings(); Dictionary<string,object> d;
        try { d=Data.Map(Data.Json.DeserializeObject(raw)); } catch{return s;}
        var enabled=Data.Get(d,"alertsEnabled");if(enabled is bool)s.AlertsEnabled=(bool)enabled;var threshold=Data.Number(Data.Get(d,"alertRemaining"));if(threshold.HasValue)s.AlertRemaining=(int)Math.Max(1,Math.Min(50,threshold.Value));s.AlertSound=SoundAlerts.Clean(Data.Get(d,"alertSound") as string);
        string provider=Data.Get(d,"provider") as string;if(provider=="claude")s.Provider=provider;
        string theme=Data.Get(d,"theme") as string;if(global::Theme.Ids.Contains(theme))s.Theme=theme;if(s.Theme=="xp")s.ClockStyle="text";
        s.Accent=Data.Hex(Data.Get(d,"accent"));s.TextColor=Data.Hex(Data.Get(d,"textColor"));
        string font=Data.Get(d,"font") as string;if(new[]{"sans","mono","serif","condensed","rounded"}.Contains(font))s.Font=font;
        string style=Data.Get(d,"clockStyle") as string;if(new[]{"segments","text","outline","atlas"}.Contains(style))s.ClockStyle=style;
        foreach(var key in new[]{"showMusic","showClock","expanded","topMost","tintDigits"}) {var v=Data.Get(d,key);if(!(v is bool))continue;bool b=(bool)v;if(key=="showMusic")s.ShowMusic=b;else if(key=="showClock")s.ShowClock=b;else if(key=="expanded")s.Expanded=b;else if(key=="topMost")s.TopMost=b;else s.TintDigits=b;}
        var array=Data.Get(d,"metrics") as IEnumerable;
        if(array!=null && !(array is string))s.Metrics=array.Cast<object>().OfType<string>().Where(x=>new[]{"cpu","gpu","ram","disk"}.Contains(x)).Distinct().OrderBy(key=>Array.IndexOf(new[]{"cpu","gpu","ram","disk"},key)).ToList();
        var fade=Data.Number(Data.Get(d,"fade"));if(fade.HasValue)s.Fade=Math.Max(0,Math.Min(1,fade.Value));
        var width=Data.Number(Data.Get(d,"width"));if(width.HasValue)s.Width=(int)Math.Max(240,Math.Min(500,width.Value));
        var refresh=Data.Number(Data.Get(d,"refreshSeconds"));if(refresh.HasValue)s.RefreshSeconds=(int)Math.Max(5,Math.Min(120,refresh.Value));
        var posX=Data.Number(Data.Get(d,"x"));var posY=Data.Number(Data.Get(d,"y"));if(posX.HasValue && posX.Value>=-100000 && posX.Value<=100000)s.X=(int)posX.Value;if(posY.HasValue && posY.Value>=-100000 && posY.Value<=100000)s.Y=(int)posY.Value;
        string image=Data.Get(d,"background") as string;if(ValidImage(image,2000000))s.Background=image;
        image=Data.Get(d,"atlas") as string;if(ValidImage(image,1000000) && image.StartsWith("data:image/png;base64,"))s.Atlas=image;
        return s;
    }
    static bool ValidImage(string s,int max) {return s!=null && s.Length<=max && (s.StartsWith("data:image/jpeg;base64,")||s.StartsWith("data:image/png;base64,"));}
    public string Serialize() {return Data.Json.Serialize(new {alertsEnabled=AlertsEnabled,alertRemaining=AlertRemaining,alertSound=AlertSound,provider=Provider,theme=Theme,accent=Accent,textColor=TextColor,font=Font,clockStyle=ClockStyle,background=Background,atlas=Atlas,showMusic=ShowMusic,showClock=ShowClock,metrics=Metrics,fade=Fade,width=Width,expanded=Expanded,topMost=TopMost,tintDigits=TintDigits,refreshSeconds=RefreshSeconds,x=X,y=Y});}
    public void Preset(string theme) { Theme=theme;Width=theme=="icq"?270:theme=="aero"?320:theme=="otimizepc"?330:new[]{"discord","napster","kazaa"}.Contains(theme)?340:300;Accent="";TextColor="";Background="";Fade=0;ClockStyle=theme=="xp"?"text":"segments";Font=theme=="winamp"?"mono":"sans"; }
}

public class Quota {
    public double Used,Minutes;
    public long? ResetsAt;
    public string Label {get{return Minutes==10080?"Semanal":Minutes==43200?"30 dias":Minutes>=60?(Minutes/60).ToString("0.#",CultureInfo.InvariantCulture)+" horas":Minutes+" min";}}
    public string ResetText(DateTime utc) {
        if(!ResetsAt.HasValue)return "Renovação não informada";
        DateTime reset;try{reset=new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc).AddSeconds(ResetsAt.Value);}catch{return "Renovação não informada";}
        var left=reset-utc;if(left.TotalSeconds<=0)return "Renovação pendente · consultando";
        return left.TotalDays>=1?"Renova em "+(int)left.TotalDays+"d "+left.Hours+"h":"Renova em "+(int)left.TotalHours+"h "+left.Minutes+"m";
    }
}
public static class QuotaParser {
    public static List<Quota> Read(string raw) {
        var result=new List<Quota>();Dictionary<string,object> d;try{d=Data.Map(Data.Json.DeserializeObject(raw));}catch{return result;}
        var all=Data.Map(Data.Get(d,"rateLimitsByLimitId"));var bucket=Data.Map(Data.Get(all,"codex"));
        if(bucket.Count==0)bucket=Data.Map(Data.Get(d,"rateLimits"));
        foreach(var key in new[]{"primary","secondary"}) {
            var window=Data.Map(Data.Get(bucket,key));var used=Data.Number(Data.Get(window,"usedPercent"));var minutes=Data.Number(Data.Get(window,"windowDurationMins"));
            if(!used.HasValue||used.Value<0||used.Value>100||!minutes.HasValue||minutes.Value<=0)continue;
            var reset=Data.Number(Data.Get(window,"resetsAt"));
            result.Add(new Quota {Used=used.Value,Minutes=minutes.Value,ResetsAt=reset.HasValue && reset.Value>0 && reset.Value<253402300799?(long?)reset.Value:null});
        }
        return result;
    }
}
public static class SensorMath {
    public static double? Cpu(ulong idle,ulong kernel,ulong user) {double total=(double)kernel+user;return total<=0?(double?)null:Math.Max(0,Math.Min(100,100*(total-idle)/total));}
}
