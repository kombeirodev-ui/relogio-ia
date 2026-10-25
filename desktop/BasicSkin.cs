using System;
using System.Drawing;
public static class BasicSkin {
public static void Draw(Graphics g,ClockSettings s,Theme theme,UsageResult usage,HardwareSample hardware,Image atlas,bool photo,bool reading,string error){
float height=WindowPlacement.ClockHeight(s);string FontName=s.Font=="mono"?"Consolas":"Tahoma";
        PaintTools.Header(g,theme);using(var edge=new Pen(theme.Border))g.DrawRectangle(edge,0,0,299,height-1);
        if(theme.Id=="winamp"){PaintTools.Line(g,Color.FromArgb(95,theme.Border),2,27,2,height-3);PaintTools.Line(g,Color.Black,297,27,297,height-3);}
        float y=31;
        if(s.ShowClock){
            if(!photo){if(theme.Id=="otimizepc")PaintTools.RoundBox(g,theme,new RectangleF(9,y,282,58));else PaintTools.Box(g,theme,new RectangleF(9,y,282,58));}
            bool right=theme.Id=="icq";PaintTools.Digits(g,DateTime.Now.ToString("HH:mm"),new RectangleF(right?137:17,y+7,146,43),theme.Accent,s.ClockStyle,atlas,s.TintDigits,FontName);
            PaintTools.Text(g,DateTime.Now.ToString("dd / MM"),12,theme.Text,right?20:185,y+10,FontName);PaintTools.Text(g,DateTime.Now.ToString("ddd").ToUpperInvariant(),10,theme.Muted,right?20:185,y+31,FontName);y+=66;
        }
        PaintTools.Text(g,error!=""?(usage.At.HasValue?"CODEX · LEITURA ANTERIOR":"CODEX · SEM LEITURA"):"CONSUMO DO "+usage.Provider.ToUpperInvariant(),10,theme.Text,12,y,FontName,FontStyle.Bold);y+=20;
        if(theme.Id=="otimizepc"){
            for(int i=0;i<2;i++){var q=usage.Windows.Count>i?usage.Windows[i]:null;float x=12+i*142;int h=s.Expanded?76:57;
                if(!photo)PaintTools.RoundBox(g,theme,new RectangleF(x,y,134,h));
                PaintTools.Text(g,q!=null?q.Label:"Sem leitura",10,theme.Muted,x+8,y+7,FontName);
                PaintTools.Text(g,q!=null?q.Used.ToString("0.#")+"%":"—",21,theme.Text,x+8,y+23,FontName,FontStyle.Bold);
                PaintTools.Fill(g,Color.FromArgb(50,theme.Muted),new RectangleF(x+8,y+49,118,3));if(q!=null)PaintTools.Fill(g,theme.Accent,new RectangleF(x+8,y+49,(float)(118*q.Used/100),3));
                if(s.Expanded)PaintTools.Text(g,q!=null?q.ResetText(DateTime.UtcNow):"Consulta pendente",8,theme.Muted,x+8,y+59,FontName);
            }y+=s.Expanded?84:65;
        }else for(int i=0;i<2;i++){
            var q=usage.Windows.Count>i?usage.Windows[i]:null;
            if(!photo&&theme.Id=="icq")PaintTools.Box(g,theme,new RectangleF(8,y-2,284,s.Expanded?43:30));
            if(theme.Id=="icq")using(var dot=new SolidBrush(q!=null?theme.Accent:theme.Muted))g.FillEllipse(dot,12,y+3,6,6);
            PaintTools.Text(g,q!=null?q.Label:"Cota indisponível",11,theme.Text,theme.Id=="icq"?23:12,y,FontName);PaintTools.Text(g,q!=null?q.Used.ToString("0.#")+"% usado":"Sem leitura",10,theme.Text,213,y,FontName);
            PaintTools.Fill(g,Color.FromArgb(60,theme.Muted),new RectangleF(12,y+17,276,5));if(q!=null)PaintTools.Fill(g,theme.Accent,new RectangleF(12,y+17,(float)(276*q.Used/100),5));
            if(s.Expanded)PaintTools.Text(g,q!=null?q.ResetText(DateTime.UtcNow):"Aguardando consulta da conta",9,theme.Muted,12,y+27,FontName);y+=s.Expanded?46:33;
        }
        if(s.Metrics.Count>0){y+=3;PaintTools.Line(g,theme.Border,10,y,290,y);y+=9;
            if((s.Expanded||theme.Id=="icq")){foreach(string key in s.Metrics){double? n;hardware.Values.TryGetValue(key,out n);if(theme.Id=="icq")using(var b=new SolidBrush(n.HasValue?theme.Accent:theme.Muted))g.FillEllipse(b,12,y+3,6,6);PaintTools.Text(g,(key=="disk"?hardware.DiskName:key.ToUpperInvariant()),10,theme.Text,theme.Id=="icq"?25:12,y,FontName);PaintTools.Text(g,n.HasValue?n.Value.ToString("0.#")+"%":"Sem leitura",10,theme.Text,214,y,FontName);y+=25;}}
            else{float cell=276f/s.Metrics.Count;for(int i=0;i<s.Metrics.Count;i++){string key=s.Metrics[i];double? n;hardware.Values.TryGetValue(key,out n);float x=12+i*cell;PaintTools.Text(g,(key=="disk"?hardware.DiskName:key.ToUpperInvariant()),9,theme.Muted,x,y,FontName);PaintTools.Text(g,n.HasValue?n.Value.ToString("0")+"%":"—",12,theme.Text,x,y+13,FontName);}y+=32;}
        }
        PaintTools.Text(g,reading?"Consultando…":usage.At.HasValue?"Lido "+usage.At.Value.ToLocalTime().ToString("HH:mm:ss")+" · "+s.RefreshSeconds+"s":"F5 para consultar",9,theme.Muted,12,height-25,FontName);
        PaintTools.Text(g,error!=""?"Sem atualização · veja configurações":"Arraste o topo · botão direito: opções",8,theme.Muted,12,height-12,FontName);

}}