using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

public static class DesktopSkins {
    [ThreadStatic]static string fontOverride;
    public static bool Handles(string id){return id=="otimizepc"||id=="msn"||id=="discord";}
    public static int Height(ClockSettings s){
        int rows=(s.Metrics.Count+1)/2;
        if(s.Theme=="otimizepc")return 34+(s.ShowClock?72:0)+22+(s.Expanded?88:72)+(s.Metrics.Count>0?24+(s.Expanded?rows*52:58):0)+28;
        if(s.Theme=="msn")return (s.ShowMusic?18:0)+28+(s.ShowClock?78:30)+24+2*(s.Expanded?67:53)+(s.Metrics.Count>0?26+s.Metrics.Count*33:0)+27;
        return 53+(s.ShowClock?68:0)+20+2*(s.Expanded?72:58)+(s.Metrics.Count>0?24+rows*44:0)+33;
    }
    static Color C(string c){return Theme.C(c);}
    static void F(Graphics g,Color c,float x,float y,float w,float h){PaintTools.Fill(g,c,new RectangleF(x,y,w,h));}
    static void L(Graphics g,Color c,float x,float y,float xx,float yy){PaintTools.Line(g,c,x,y,xx,yy);}
    static GraphicsPath Path(RectangleF r,float radius){float d=Math.Min(radius*2,Math.Min(r.Width,r.Height));var p=new GraphicsPath();p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
    static void Card(Graphics g,RectangleF r,Color bg,Color border,float radius=6){using(var p=Path(r,radius)){using(var b=new SolidBrush(bg))g.FillPath(b,p);using(var pen=new Pen(border))g.DrawPath(pen,p);}}
    static void Gradient(Graphics g,RectangleF r,Color top,Color bottom){using(var b=new LinearGradientBrush(r,top,bottom,90))g.FillRectangle(b,r);}
    static void T(Graphics g,string value,float size,Color c,float x,float y,string font="Inter",bool bold=false){PaintTools.Text(g,value,size,c,x,y,fontOverride??font,bold?FontStyle.Bold:FontStyle.Regular);}
    static void Fit(Graphics g,string value,float size,Color c,RectangleF r,string font="Inter",bool bold=false){using(var f=EmbeddedFonts.Make(fontOverride??font,size,bold?FontStyle.Bold:FontStyle.Regular))using(var b=new SolidBrush(c))using(var sf=new StringFormat {Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap,LineAlignment=StringAlignment.Center})g.DrawString(value,f,b,r,sf);}
    static string Percent(double? n){return n.HasValue?n.Value.ToString("0.#")+"%":"—";}
    static string Metric(string key,HardwareSample sample){return key=="disk"?sample.DiskName:key.ToUpperInvariant();}
    static Color Severity(double? n,Color normal){return !n.HasValue?C("#7e8998"):n>=90?C("#f07878"):n>=70?C("#FFC907"):normal;}
    static void Bar(Graphics g,RectangleF r,double? value,Color fill,Color track){Card(g,r,track,track,2);if(value.HasValue&&value.Value>0){float w=(float)(r.Width*Math.Max(0,Math.Min(100,value.Value))/100);F(g,fill,r.X,r.Y,w,r.Height);}}
    static string Status(UsageResult u,bool reading,string error,int seconds){return reading?"Consultando a conta…":error!=""?(u.At.HasValue?"Leitura anterior · F5 para atualizar":"Sem leitura · F5 para tentar"):u.At.HasValue?"Atualizado "+u.At.Value.ToLocalTime().ToString("HH:mm:ss")+" · "+seconds+"s":"Aguardando leitura da conta";}
    static void Clock(Graphics g,ClockSettings s,Theme t,Image atlas,RectangleF r,string font){PaintTools.Digits(g,DateTime.Now.ToString("HH:mm"),r,t.Accent,s.ClockStyle,atlas,s.TintDigits,fontOverride??font);}
    public static void Draw(Graphics g,ClockSettings s,Theme t,UsageResult u,HardwareSample sample,Image atlas,bool photo,bool reading,string error){
        string previous=fontOverride; fontOverride=s.Font=="mono"?"Consolas":s.Font=="serif"?"Georgia":s.Font=="condensed"?"Impact":s.Font=="rounded"?"Trebuchet MS":null;
        try{if(s.Theme=="otimizepc")Otimize(g,s,t,u,sample,atlas,photo,reading,error);
        else if(s.Theme=="msn")Msn(g,s,t,u,sample,atlas,photo,reading,error);
        else Discord(g,s,t,u,sample,atlas,photo,reading,error);}finally{fontOverride=previous;}
    }
    static void Otimize(Graphics g,ClockSettings s,Theme t,UsageResult u,HardwareSample sample,Image atlas,bool photo,bool reading,string error){
        int h=Height(s);F(g,t.Header,0,0,300,32);BrandArt.Otimize(g,new RectangleF(10,3,22,25));T(g,"RELÓGIO IA",11,t.Text,41,6,"Inter",true);T(g,"OtimizePC",8.5f,t.Muted,41,20,"Roboto");L(g,t.Border,0,32,300,32);int y=42;
        if(s.ShowClock){Card(g,new RectangleF(11,y,177,57),photo?Color.FromArgb(155,t.Inset):t.Inset,t.Border);Clock(g,s,t,atlas,new RectangleF(22,y+10,152,39),"Inter");
            Card(g,new RectangleF(196,y,93,57),photo?Color.FromArgb(155,t.Panel):t.Panel,t.Border);T(g,DateTime.Now.ToString("dd / MM"),15,t.Text,209,y+10,"Inter",true);T(g,DateTime.Now.ToString("ddd").ToUpperInvariant(),9,t.Muted,210,y+34,"Roboto");F(g,C("#FFC907"),196,y+53,93,3);y+=72;
        }
        T(g,"CONSUMO DO "+u.Provider.ToUpperInvariant(),10,t.Text,12,y,"Inter",true);F(g,C("#0078D4"),12,y+15,26,2);y+=22;
        for(int i=0;i<2;i++){var q=u.Windows.Count>i?u.Windows[i]:null;int x=12+i*142,cardH=s.Expanded?78:62;Card(g,new RectangleF(x,y,134,cardH),photo?Color.FromArgb(150,t.Panel):t.Panel,t.Border);T(g,q!=null?q.Label:i==0?"5 horas":"Semanal",10,t.Muted,x+10,y+9,"Roboto");T(g,q==null?"—":Percent(q.Used),23,t.Text,x+9,y+24,"Inter",true);Bar(g,new RectangleF(x+10,y+54,114,4),q==null?(double?)null:q.Used,Severity(q==null?(double?)null:q.Used,t.Accent),t.Border);if(s.Expanded)Fit(g,q==null?"Sem leitura confirmada":q.ResetText(DateTime.UtcNow),8,t.Muted,new RectangleF(x+10,y+62,115,11),"Roboto");}
        y+=s.Expanded?88:72;
        if(s.Metrics.Count>0){T(g,"SEU COMPUTADOR",9,t.Muted,12,y,"Inter",true);y+=20;
            if(s.Expanded){for(int i=0;i<s.Metrics.Count;i++){string key=s.Metrics[i];double? n;sample.Values.TryGetValue(key,out n);int x=12+(i%2)*142,yy=y+(i/2)*52;Card(g,new RectangleF(x,yy,134,45),photo?Color.FromArgb(150,t.Panel):t.Panel,t.Border);BrandArt.Sensor(g,key,new RectangleF(x+9,yy+10,16,16),C("#0078D4"));Fit(g,Metric(key,sample),9,t.Muted,new RectangleF(x+32,yy+5,65,13),"Roboto");T(g,Percent(n),13,t.Text,x+32,yy+19,"Inter",true);Bar(g,new RectangleF(x+9,yy+37,116,3),n,Severity(n,t.Accent),t.Border);}y+=((s.Metrics.Count+1)/2)*52;}
            else{float cell=276f/s.Metrics.Count;for(int i=0;i<s.Metrics.Count;i++){string key=s.Metrics[i];double? n;sample.Values.TryGetValue(key,out n);float x=12+i*cell;Card(g,new RectangleF(x,y,cell-5,50),t.Panel,t.Border);BrandArt.Sensor(g,key,new RectangleF(x+7,y+7,12,12),C("#0078D4"));Fit(g,Metric(key,sample),8,t.Muted,new RectangleF(x+22,y+5,cell-25,15),"Roboto");T(g,Percent(n),12,t.Text,x+7,y+25,"Inter",true);Bar(g,new RectangleF(x+7,y+43,cell-19,3),n,Severity(n,t.Accent),t.Border);}}
        }
        L(g,t.Border,12,h-25,288,h-25);Fit(g,Status(u,reading,error,s.RefreshSeconds),8,t.Muted,new RectangleF(12,h-23,276,18),"Roboto");
    }
    static void Msn(Graphics g,ClockSettings s,Theme t,UsageResult u,HardwareSample sample,Image atlas,bool photo,bool reading,string error){
        int h=Height(s);if(!photo){Gradient(g,new RectangleF(0,0,300,h),C("#d9e8fb"),C("#f0f4fc"));F(g,Color.White,26,106,268,h-132);}Gradient(g,new RectangleF(1,1,298,25),C("#6594f6"),C("#0754d8"));L(g,C("#bbd7ff"),3,2,297,2);BrandArt.Buddy(g,new RectangleF(5,2,22,22),true);T(g,"Relógio IA",11,Color.White,31,7,"Tahoma",true);int y=31;
        Gradient(g,new RectangleF(6,y,288,(s.ShowClock?71:26)+(s.ShowMusic?18:0)),C("#edf4ff"),C("#c5d9ef"));
        if(s.ShowClock){Card(g,new RectangleF(13,y+7,56,56),C("#fafcff"),C("#8da7ca"),3);BrandArt.Buddy(g,new RectangleF(18,y+11,46,46),u.At.HasValue);Clock(g,s,t,atlas,new RectangleF(84,y+6,131,39),"Tahoma");Card(g,new RectangleF(227,y+6,58,38),C("#f7fbff"),C("#a7bdd7"),3);T(g,DateTime.Now.ToString("dd/MM"),11,t.Text,233,y+11,"Tahoma",true);T(g,DateTime.Now.ToString("ddd").ToUpperInvariant(),9,t.Muted,234,y+27,"Tahoma");T(g,u.At.HasValue?u.Provider+" (leitura disponível)":u.Provider+" (aguardando leitura)",9,t.Text,83,y+51,"Tahoma");y+=78;}
        else{BrandArt.Buddy(g,new RectangleF(12,y+2,23,23),u.At.HasValue);T(g,"Relógio IA · conta "+u.Provider,10,t.Text,42,y+8,"Tahoma");y+=30;}
        if(s.ShowMusic)y+=18;
        L(g,C("#9eb8d2"),6,y-3,294,y-3);F(g,C("#d0dfef"),6,y,17,h-y-28);BrandArt.Buddy(g,new RectangleF(6,y+12,17,19),true);
        T(g,"− Cotas da conta (2)",11,t.Text,33,y+5,"Tahoma",true);y+=24;
        for(int i=0;i<2;i++){var q=u.Windows.Count>i?u.Windows[i]:null;int rowH=s.Expanded?67:53;Card(g,new RectangleF(31,y,260,rowH-6),photo?Color.FromArgb(190,Color.White):Color.White,C("#d4e0ed"),2);BrandArt.Buddy(g,new RectangleF(37,y+5,24,24),q!=null);T(g,q==null?(i==0?"5 horas":"Semanal"):q.Label,11,t.Text,66,y+8,"Tahoma",true);Fit(g,q==null?"Sem leitura":Percent(q.Used)+" usado",10,q!=null&&q.Used>=90?C("#b53632"):t.Text,new RectangleF(201,y+6,83,18),"Tahoma",true);Bar(g,new RectangleF(66,y+29,211,5),q==null?(double?)null:q.Used,q!=null&&q.Used>=90?C("#d45642"):C("#72b848"),C("#e4ebf1"));if(s.Expanded)T(g,q==null?"Aguardando consulta":q.ResetText(DateTime.UtcNow),9,t.Muted,66,y+43,"Tahoma");y+=rowH;}
        if(s.Metrics.Count>0){T(g,"− Meu computador ("+s.Metrics.Count+")",11,t.Text,33,y+4,"Tahoma",true);y+=26;foreach(string key in s.Metrics){double? n;sample.Values.TryGetValue(key,out n);BrandArt.Sensor(g,key,new RectangleF(38,y+4,17,17),C("#4075a6"));Fit(g,Metric(key,sample),10,t.Text,new RectangleF(65,y,102,20),"Tahoma");Fit(g,Percent(n),10,t.Text,new RectangleF(231,y,49,20),"Tahoma",true);Bar(g,new RectangleF(65,y+22,212,3),n,C("#76b94d"),C("#dce6ef"));y+=33;}}
        Gradient(g,new RectangleF(5,h-25,290,21),C("#f3f7fc"),C("#c5d7ec"));L(g,C("#a6bcd7"),5,h-26,295,h-26);Fit(g,Status(u,reading,error,s.RefreshSeconds),8,t.Muted,new RectangleF(12,h-23,278,17),"Tahoma");using(var p=new Pen(C("#708fae")))g.DrawRectangle(p,0,0,299,h-1);
    }
    static void Discord(Graphics g,ClockSettings s,Theme t,UsageResult u,HardwareSample sample,Image atlas,bool photo,bool reading,string error){
        int h=Height(s);F(g,C("#1e1f22"),0,0,300,22);T(g,"Relógio IA",9,t.Muted,10,6,"Inter",true);F(g,C("#1e1f22"),0,22,37,h-22);F(g,C("#2b2d31"),37,22,78,h-22);L(g,C("#1f2023"),115,22,115,h);F(g,C("#232428"),37,22,78,30);T(g,"Relógio IA",10,t.Text,43,33,"Inter",true);
        using(var b=new SolidBrush(C("#5865F2")))g.FillEllipse(b,5,31,27,27);BrandArt.Controller(g,new RectangleF(9,38,20,15),Color.White);F(g,Color.White,0,36,3,17);L(g,C("#35363c"),6,67,30,67);
        using(var b=new SolidBrush(C("#313338")))g.FillEllipse(b,6,79,25,25);T(g,"IA",10,t.Text,12,88,"Inter",true);using(var b=new SolidBrush(C("#23a55a")))g.FillEllipse(b,23,98,7,7);
        T(g,"⌄ PAINEL",8,t.Muted,43,66,"Inter",true);Card(g,new RectangleF(41,82,70,25),C("#404249"),C("#404249"),3);T(g,"# consumo",9,t.Text,46,90,"Inter",true);if(s.Metrics.Count>0)T(g,"# hardware",9,t.Muted,46,119,"Inter");if(h>=260){T(g,"COTAS",8,t.Muted,44,154,"Inter",true);T(g,"• 5 horas",9,t.Muted,45,174,"Inter");T(g,"• semanal",9,t.Muted,45,194,"Inter");}
        F(g,C("#232428"),37,h-39,78,39);using(var b=new SolidBrush(C("#5865F2")))g.FillEllipse(b,43,h-30,22,22);T(g,"IA",8,Color.White,49,h-23,"Inter",true);T(g,u.Provider,9,t.Text,70,h-29,"Inter",true);T(g,u.At.HasValue?"Leitura OK":"Sem leitura",7.5f,t.Muted,70,h-15,"Inter");
        F(g,t.Header,116,22,184,29);T(g,"#",17,t.Muted,125,27,"Inter");T(g,"consumo",11,t.Text,143,31,"Inter",true);L(g,C("#232428"),116,51,300,51);int y=59;
        if(s.ShowClock){Card(g,new RectangleF(124,y,166,57),photo?Color.FromArgb(170,t.Inset):t.Inset,t.Inset,4);Clock(g,s,t,atlas,new RectangleF(133,y+6,149,33),"Inter");T(g,DateTime.Now.ToString("ddd, dd / MM"),9,t.Muted,134,y+43,"Inter");y+=68;}
        T(g,"CONTA "+u.Provider.ToUpperInvariant(),8,t.Muted,125,y,"Inter",true);y+=20;
        for(int i=0;i<2;i++){var q=u.Windows.Count>i?u.Windows[i]:null;int rowH=s.Expanded?72:58;Card(g,new RectangleF(124,y,166,rowH-7),photo?Color.FromArgb(170,t.Panel):t.Panel,t.Panel,4);T(g,q==null?(i==0?"5 horas":"Semanal"):q.Label,10,t.Text,134,y+8,"Inter",true);T(g,q==null?"—":Percent(q.Used),18,t.Text,134,y+25,"Inter",true);Bar(g,new RectangleF(134,y+47,145,4),q==null?(double?)null:q.Used,q!=null&&q.Used>=90?C("#ed4245"):C("#5865F2"),C("#1e1f22"));if(s.Expanded)Fit(g,q==null?"Sem leitura":q.ResetText(DateTime.UtcNow),8,t.Muted,new RectangleF(134,y+53,145,11),"Inter");y+=rowH;}
        if(s.Metrics.Count>0){T(g,"HARDWARE",8,t.Muted,125,y+2,"Inter",true);y+=24;for(int i=0;i<s.Metrics.Count;i++){string key=s.Metrics[i];double? n;sample.Values.TryGetValue(key,out n);int x=124+i%2*85,yy=y+i/2*44;Card(g,new RectangleF(x,yy,81,38),photo?Color.FromArgb(170,t.Inset):t.Inset,t.Inset,4);Fit(g,Metric(key,sample),8,t.Muted,new RectangleF(x+7,yy+3,68,12),"Inter");T(g,Percent(n),12,t.Text,x+7,yy+18,"Inter",true);}}
        L(g,C("#232428"),124,h-28,290,h-28);Fit(g,Status(u,reading,error,s.RefreshSeconds),7.5f,t.Muted,new RectangleF(124,h-25,167,20),"Inter");
    }
}
