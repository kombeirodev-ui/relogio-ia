using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

public class QuotaAlertPolicy {
    class Seen {public double Used;public bool Sent;}
    readonly Dictionary<string,Seen> buckets=new Dictionary<string,Seen>();
    public bool Observe(string provider,double minutes,long? reset,double used,int remaining,DateTime now){
        if(Double.IsNaN(used)||Double.IsInfinity(used)||used<0||used>100)return false;
        string key=provider+":"+minutes+":"+(reset.HasValue?reset.Value.ToString():"unknown");Seen previous;
        if(!buckets.TryGetValue(key,out previous)){if(buckets.Count>64)buckets.Clear();buckets[key]=new Seen {Used=used};return false;}
        // Some providers omit reset timestamps: a clear drop starts a new cycle.
        if(used+10<previous.Used)previous.Sent=false;
        double limit=100-Math.Max(1,Math.Min(50,remaining));bool fire=!previous.Sent&&previous.Used<limit&&used>=limit;previous.Used=used;if(fire)previous.Sent=true;return fire;
    }
    public void Clear(){buckets.Clear();}
}

public sealed class SoundAlerts : IDisposable {
    readonly QuotaAlertPolicy policy=new QuotaAlertPolicy();SoundPlayer player;MemoryStream stream;string loaded="";
    public static bool ValidWave(byte[] bytes){if(bytes==null||bytes.Length<44||bytes.Length>2000000)return false;if(System.Text.Encoding.ASCII.GetString(bytes,0,4)!="RIFF"||System.Text.Encoding.ASCII.GetString(bytes,8,4)!="WAVE")return false;int offset=12;bool format=false,data=false;while(offset+8<=bytes.Length){string id=System.Text.Encoding.ASCII.GetString(bytes,offset,4);uint size=BitConverter.ToUInt32(bytes,offset+4);if(size>bytes.Length-offset-8)return false;if(id=="fmt "){if(size<16)return false;ushort encoding=BitConverter.ToUInt16(bytes,offset+8),channels=BitConverter.ToUInt16(bytes,offset+10),bits=BitConverter.ToUInt16(bytes,offset+22);uint rate=BitConverter.ToUInt32(bytes,offset+12);format=encoding==1&&channels>=1&&channels<=2&&rate>=8000&&rate<=192000&&(bits==8||bits==16);}if(id=="data")data=size>0;long next=offset+8L+size+(size%2);if(next>Int32.MaxValue)return false;offset=(int)next;}return format&&data;}
    public static string Clean(string value){if(String.IsNullOrEmpty(value)||value.Length>2800000||!value.StartsWith("data:audio/wav;base64,"))return "";try{return ValidWave(Convert.FromBase64String(value.Substring(22)))?value:"";}catch{return "";}}
    public void Observe(ClockSettings settings,UsageResult usage){if(!settings.AlertsEnabled){policy.Clear();return;}if(!usage.At.HasValue)return;bool play=false;foreach(var quota in usage.Windows)if(policy.Observe(usage.Provider,quota.Minutes,quota.ResetsAt,quota.Used,settings.AlertRemaining,DateTime.UtcNow))play=true;if(play)Play(settings.AlertSound);}
    public void Play(string value){try{if(String.IsNullOrEmpty(value)){SystemSounds.Asterisk.Play();return;}if(value!=loaded){DisposePlayer();stream=new MemoryStream(Convert.FromBase64String(value.Substring(22)));player=new SoundPlayer(stream);player.Load();loaded=value;}player.Play();}catch{/* Optional audio failure never interrupts usage monitoring. */}}
    void DisposePlayer(){if(player!=null){player.Stop();player.Dispose();player=null;}if(stream!=null){stream.Dispose();stream=null;}loaded="";}
    public void Dispose(){DisposePlayer();}
}
