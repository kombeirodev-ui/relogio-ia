using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

// Optional reader for the Claude Code CLI OAuth usage endpoint used by the
// studied ai-usagebar base. This endpoint is not a documented public API.
// Never copies, prints, refreshes or rewrites the CLI's credentials.
public static class ClaudeClient {
    static readonly object gate=new object();static UsageResult cached;static DateTime attempted=DateTime.MinValue;
    public static UsageResult Parse(string raw){
        var result=new UsageResult {Provider="Claude"};
        try{var root=Data.Map(Data.Json.DeserializeObject(raw));foreach(string key in new[]{"five_hour","seven_day"}){
            var value=Data.Map(Data.Get(root,key));double? used=Data.Number(Data.Get(value,"utilization"));if(!used.HasValue||used<0||used>100)continue;
            DateTimeOffset reset;long? epoch=null;if(DateTimeOffset.TryParse(Data.Get(value,"resets_at") as string,CultureInfo.InvariantCulture,DateTimeStyles.AssumeUniversal,out reset))epoch=(long)(reset.ToUniversalTime()-new DateTimeOffset(1970,1,1,0,0,0,TimeSpan.Zero)).TotalSeconds;
            result.Windows.Add(new Quota {Used=used.Value,Minutes=key=="five_hour"?300:10080,ResetsAt=epoch});
        }if(result.Windows.Count>0)result.At=DateTime.UtcNow;else result.Error="Claude não informou cotas para esta conta.";
        }catch{result.Error="A resposta de cotas do Claude não foi reconhecida.";}return result;
    }
    public static UsageResult Read(){lock(gate){
        if(cached!=null&&(DateTime.UtcNow-attempted).TotalSeconds<60)return cached;
        attempted=DateTime.UtcNow;var result=new UsageResult {Provider="Claude"};
        try{
            string directory=Environment.GetEnvironmentVariable("CLAUDE_CONFIG_DIR");if(String.IsNullOrEmpty(directory))directory=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".claude");
            string path=Path.Combine(directory,".credentials.json");
            if(!File.Exists(path)){result.Error="Sem login local do Claude Code. Abra o Claude Code e entre na sua conta.";return cached=result;}
            if(new FileInfo(path).Length>1000000){result.Error="Arquivo de login Claude inesperado.";return cached=result;}
            var data=Data.Map(Data.Json.DeserializeObject(File.ReadAllText(path)));var oauth=Data.Map(Data.Get(data,"claudeAiOauth"));string token=Data.Get(oauth,"accessToken") as string;double? expires=Data.Number(Data.Get(oauth,"expiresAt"));
            double now=(DateTime.UtcNow-new DateTime(1970,1,1)).TotalMilliseconds;
            if(String.IsNullOrEmpty(token)||expires.HasValue&&expires.Value<=now){result.Error="Login do Claude ausente ou expirado. Renove a sessão no Claude Code.";return cached=result;}
            ServicePointManager.SecurityProtocol|=SecurityProtocolType.Tls12;var request=(HttpWebRequest)WebRequest.Create("https://api.anthropic.com/api/oauth/usage");request.Method="GET";request.Timeout=8000;request.ReadWriteTimeout=8000;request.AllowAutoRedirect=false;request.Headers["Authorization"]="Bearer "+token;request.Headers["anthropic-beta"]="oauth-2025-04-20";request.UserAgent="RelogioIA/0.3";request.Accept="application/json";
            using(var response=request.GetResponse())using(var stream=response.GetResponseStream())using(var buffer=new MemoryStream()){byte[] chunk=new byte[8192];int n;while((n=stream.Read(chunk,0,chunk.Length))>0){if(buffer.Length+n>1000000)throw new IOException();buffer.Write(chunk,0,n);}result=Parse(Encoding.UTF8.GetString(buffer.ToArray()));}
        }catch(WebException e){var response=e.Response as HttpWebResponse;result.Error=response!=null&&response.StatusCode==HttpStatusCode.Unauthorized?"O Claude recusou o login. Renove a sessão no Claude Code.":response!=null&&(int)response.StatusCode==429?"Claude limitou a consulta. Aguarde antes de atualizar.":"Não foi possível consultar cotas do Claude agora.";if(response!=null)response.Dispose();}
        catch{result.Error="Não foi possível ler o login ou as cotas do Claude.";}
        return cached=result;
    }}
}
