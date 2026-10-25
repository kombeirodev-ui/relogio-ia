using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

public static class RetroSkins {

    static readonly Color Light=Color.White,Shadow=Color.FromArgb(128,128,128),Black=Color.Black;
    public static bool Handles(string id){return id=="napster"||id=="kazaa";}
    public static int Height(ClockSettings s){return 28+(s.ShowClock?66:0)+24+24+2*(s.Expanded?55:39)+(s.Metrics.Count>0?25+s.Metrics.Count*25:0)+29;}
    static void F(Graphics g,Color c,float x,float y,float w,float h){PaintTools.Fill(g,c,new RectangleF(x,y,w,h));}
    static void L(Graphics g,Color c,float x,float y,float xx,float yy){PaintTools.Line(g,c,x,y,xx,yy);}
    static string Font(ClockSettings s){return s.Font=="mono"?"Consolas":s.Font=="serif"?"Georgia":s.Font=="condensed"?"Impact":s.Font=="rounded"?"Trebuchet MS":"Tahoma";}
    static void T(Graphics g,string value,float size,Color c,float x,float y,ClockSettings s,bool bold=false){PaintTools.Text(g,value,size,c,x,y,Font(s),bold?FontStyle.Bold:FontStyle.Regular);}
    static void Fit(Graphics g,string value,float size,Color c,RectangleF r,ClockSettings s,bool bold=false,bool center=false){using(var font=EmbeddedFonts.Make(Font(s),size,bold?FontStyle.Bold:FontStyle.Regular))using(var b=new SolidBrush(c))using(var sf=new StringFormat {Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap,LineAlignment=StringAlignment.Center,Alignment=center?StringAlignment.Center:StringAlignment.Near})g.DrawString(value,font,b,r,sf);}
    static void Bevel(Graphics g,RectangleF r,bool raised){L(g,raised?Light:Shadow,r.X,r.Y,r.Right,r.Y);L(g,raised?Light:Shadow,r.X,r.Y,r.X,r.Bottom);L(g,raised?Shadow:Light,r.X,r.Bottom,r.Right,r.Bottom);L(g,raised?Shadow:Light,r.Right,r.Y,r.Right,r.Bottom);}
    static void Bar(Graphics g,RectangleF r,double? value,Theme t){F(g,Color.White,r.X,r.Y,r.Width,r.Height);Bevel(g,r,false);if(value.HasValue){float w=(float)((r.Width-4)*Math.Max(0,Math.Min(100,value.Value))/100);if(w>0)using(var b=new LinearGradientBrush(new RectangleF(r.X+2,r.Y+2,Math.Max(1,w),r.Height-3),t.Id=="napster"?Color.FromArgb(249,132,122):Color.FromArgb(220,248,123),t.Id=="napster"&&t.Accent.ToArgb()==Color.Navy.ToArgb()?Color.FromArgb(178,47,47):t.Accent,90f))g.FillRectangle(b,r.X+2,r.Y+2,w,r.Height-3);}}
    static string P(double? n){return n.HasValue?n.Value.ToString("0.#")+"%":"—";}
    static void Stripe(Graphics g,Theme t,int y,string label,ClockSettings s){
        if(t.Id=="kazaa"){using(var b=new LinearGradientBrush(new Rectangle(7,y,286,20),Color.FromArgb(217,246,143),Color.FromArgb(140,183,47),90f))g.FillRectangle(b,7,y,286,20);L(g,Color.White,8,y+1,292,y+1);L(g,Color.FromArgb(107,139,39),7,y+20,293,y+20);}
        else{F(g,Color.FromArgb(218,218,218),7,y,286,20);Bevel(g,new RectangleF(7,y,286,20),true);}
        T(g,label,10,Black,14,y+5,s,true);
    }

    static void Cat(Graphics g,RectangleF r){PublicArt.Wave(g,r,Color.FromArgb(93,136,219));}
    static void KazaaLogo(Graphics g,RectangleF r){PublicArt.Bars(g,r,Color.FromArgb(122,166,36));}
    public static Bitmap ControlIcon(string kind,Theme t){var image=new Bitmap(20,20);using(var g=Graphics.FromImage(image)){F(g,t.Background,0,0,20,20);Bevel(g,new RectangleF(1,2,17,16),true);using(var p=new Pen(Black,1.2f)){if(kind=="close"){g.DrawLine(p,6,6,13,13);g.DrawLine(p,13,6,6,13);}else if(kind=="minimize")g.DrawLine(p,5,13,14,13);else if(kind=="expand"){g.DrawRectangle(p,5,5,9,8);g.DrawLine(p,5,7,14,7);}else{g.DrawLine(p,5,7,14,7);g.DrawLine(p,5,12,14,12);g.DrawEllipse(p,7,5,3,4);g.DrawEllipse(p,11,10,3,4);}}}return image;}
    public static void Draw(Graphics g,ClockSettings s,Theme t,UsageResult u,HardwareSample sample,Image atlas,bool photo,bool reading,string error){
        int h=Height(s);bool lime=t.Id=="kazaa";
        F(g,t.Header,2,2,296,23);if(lime){using(var b=new SolidBrush(Color.FromArgb(152,202,42)))g.FillEllipse(b,6,5,17,17);T(g,"IA",9,Black,8,8,s,true);}else Cat(g,new RectangleF(5,3,20,20));
        T(g,"Relógio IA",11,lime?Black:Light,29,8,s,true);int y=32;
        if(s.ShowClock){F(g,photo?Color.FromArgb(200,t.Panel):t.Panel,8,y,184,56);Bevel(g,new RectangleF(8,y,184,56),false);PaintTools.Digits(g,DateTime.Now.ToString("HH:mm"),new RectangleF(20,y+9,151,38),t.Accent,s.ClockStyle,atlas,s.TintDigits,Font(s));F(g,t.Panel,201,y,91,56);Bevel(g,new RectangleF(201,y,91,56),false);F(g,t.Background,203,y+2,87,19);Fit(g,DateTime.Now.ToString("dddd"),10,Black,new RectangleF(205,y+3,83,17),s,false,true);Fit(g,DateTime.Now.ToString("dd/MM"),16,lime?Color.FromArgb(81,112,18):Color.Navy,new RectangleF(205,y+23,83,28),s,true,true);y+=66;}
        Stripe(g,t,y,lime?"Monitor de uso — Conta "+u.Provider:"Biblioteca de uso — Conta "+u.Provider,s);y+=24;
        F(g,lime?Color.FromArgb(248,249,240):Color.FromArgb(239,239,239),8,y,65,s.Expanded?132:100);Bevel(g,new RectangleF(8,y,65,s.Expanded?132:100),false);
        if(lime){KazaaLogo(g,new RectangleF(12,y+6,57,34));L(g,Color.FromArgb(203,212,175),14,y+46,67,y+46);F(g,t.Accent,13,y+52,3,28);T(g,"Fonte",9,t.Muted,21,y+51,s);T(g,u.Provider,11,Black,21,y+65,s,true);if(s.Expanded){L(g,Color.FromArgb(215,220,199),14,y+85,67,y+85);T(g,"Consulta",9,t.Muted,14,y+91,s);T(g,s.RefreshSeconds+" s",11,Black,14,y+105,s,true);}}
        else{F(g,Color.FromArgb(211,211,211),10,y+2,61,17);Bevel(g,new RectangleF(10,y+2,61,17),true);T(g,"Conta",9,Black,15,y+6,s,true);using(var b=new SolidBrush(u.At.HasValue&&error==""?Color.Green:Color.Gray))g.FillEllipse(b,14,y+28,5,5);T(g,u.Provider,10,Black,23,y+25,s);L(g,Color.FromArgb(185,185,185),13,y+44,68,y+44);T(g,"Monitor",9,t.Muted,14,y+50,s);F(g,Color.Navy,11,y+68,59,18);F(g,Color.FromArgb(244,207,66),15,y+73,10,8);T(g,"Cotas",9,Light,29,y+73,s);if(s.Expanded){Cat(g,new RectangleF(25,y+95,31,31));}}
        F(g,t.Background,80,y,212,20);Bevel(g,new RectangleF(80,y,212,20),true);T(g,"Período",9,Black,85,y+6,s,true);L(g,Shadow,168,y+2,168,y+18);T(g,"Usado",9,Black,174,y+6,s,true);L(g,Shadow,224,y+2,224,y+18);T(g,"Livre",9,Black,236,y+6,s,true);y+=24;
        for(int i=0;i<2;i++){var q=u.Windows.Count>i?u.Windows[i]:null;int rowH=s.Expanded?55:39;F(g,photo?Color.FromArgb(225,Color.White):Color.White,80,y,212,rowH-2);Bevel(g,new RectangleF(80,y,212,rowH-2),false);T(g,q==null?(i==0?"5 horas":"Semanal"):q.Label,10,Black,86,y+7,s,true);T(g,q==null?"—":P(q.Used),10,Black,176,y+7,s,true);T(g,q==null?"—":P(100-q.Used),10,Black,237,y+7,s);Bar(g,new RectangleF(86,y+25,198,7),q==null?(double?)null:q.Used,t);if(s.Expanded)Fit(g,q==null?"Aguardando leitura":q.ResetText(DateTime.UtcNow),8,t.Muted,new RectangleF(86,y+36,198,14),s);y+=rowH;}
        if(s.Metrics.Count>0){Stripe(g,t,y,"Meu computador — Windows",s);y+=25;foreach(string key in s.Metrics){double? n;sample.Values.TryGetValue(key,out n);F(g,photo?Color.FromArgb(225,Color.White):Color.White,8,y,284,24);Bevel(g,new RectangleF(8,y,284,24),false);BrandArt.Sensor(g,key,new RectangleF(14,y+5,14,14),lime?Color.FromArgb(100,143,25):Color.FromArgb(55,66,94));Fit(g,key=="disk"?sample.DiskName:key.ToUpperInvariant(),10,Black,new RectangleF(35,y+2,72,20),s);Bar(g,new RectangleF(111,y+7,118,9),n,t);Fit(g,P(n),10,Black,new RectangleF(241,y+2,47,20),s,true);y+=25;}}
        F(g,t.Background,5,h-25,290,21);Bevel(g,new RectangleF(5,h-25,290,21),false);string status=reading?"Consultando "+u.Provider+"…":error!=""?(u.At.HasValue?"Leitura anterior · F5 atualiza":"Sem leitura · F5 tenta novamente"):u.At.HasValue?"Lido "+u.At.Value.ToLocalTime().ToString("HH:mm:ss")+" · "+s.RefreshSeconds+" s":"Aguardando leitura da conta";Fit(g,status,8,t.Muted,new RectangleF(11,h-23,275,17),s);Bevel(g,new RectangleF(0,0,299,h-1),true);
    }
}
