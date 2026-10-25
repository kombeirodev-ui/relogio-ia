using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

public class UsageResult {
    public string Provider="Codex";
    public List<Quota> Windows=new List<Quota>();
    public DateTime? At;
    public string Error="",Account="";
}
public static class CodexClient {
    static readonly object gate=new object();static Process active;
    public static void Cancel(){lock(gate){if(active!=null)try{if(!active.HasExited)active.Kill();}catch{}}}
    public static UsageResult Read() {
        var result=new UsageResult();Process process=null;
        try {
            string root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"OpenAI","Codex","bin");
            string exe=Directory.Exists(root)?Directory.GetFiles(root,"codex.exe",SearchOption.AllDirectories).OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault():null;
            process=Process.Start(new ProcessStartInfo(exe??"codex.exe","app-server --listen stdio://") {UseShellExecute=false,CreateNoWindow=true,RedirectStandardInput=true,RedirectStandardOutput=true,RedirectStandardError=true,StandardOutputEncoding=Encoding.UTF8});
            lock(gate)active=process;
            process.BeginErrorReadLine();var child=process;
            using(var watchdog=new System.Threading.Timer(s=>{try{if(!child.HasExited)child.Kill();}catch{}},null,12000,Timeout.Infinite)) {
                Action<object> send=o=>{process.StandardInput.WriteLine(Data.Json.Serialize(o));process.StandardInput.Flush();};
                send(new {id=1,method="initialize",@params=new {clientInfo=new {name="relogio_ia_desktop",title="Relogio IA",version="0.2.0"}}});
                string line;int bytes=0;
                while((line=process.StandardOutput.ReadLine())!=null) {
                    bytes+=line.Length;if(bytes>1000000)throw new Exception("Resposta excessiva");
                    Dictionary<string,object> message;try{message=Data.Map(Data.Json.DeserializeObject(line));}catch{continue;}
                    var id=Data.Number(Data.Get(message,"id"));if(!id.HasValue)continue;
                    if(Data.Get(message,"error")!=null)throw new Exception("A conta não disponibilizou limites");
                    if(id==1){send(new {method="initialized"});send(new {id=2,method="account/read",@params=new {refreshToken=false}});}
                    if(id==2){var account=Data.Map(Data.Get(Data.Map(Data.Get(message,"result")),"account"));result.Account=Convert.ToString(Data.Get(account,"type"));send(new {id=3,method="account/rateLimits/read"});}
                    if(id==3){result.Windows=QuotaParser.Read(Data.Json.Serialize(Data.Get(message,"result")));if(result.Windows.Count==0)throw new Exception("Sem limites informados para esta conta");result.At=DateTime.UtcNow;break;}
                }
            }
            if(!result.At.HasValue)result.Error="Consulta não concluída. Confira o login do Codex.";
        }catch(Exception e){result.Error=e.Message.StartsWith("Sem limites")?e.Message:"Não foi possível consultar a conta do Codex.";}
        finally{lock(gate){if(active==process)active=null;}if(process!=null){try{if(!process.HasExited)process.Kill();}catch{}process.Dispose();}}
        return result;
    }
}
