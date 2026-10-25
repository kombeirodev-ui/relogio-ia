using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

public static class AeroSkin {
    static Bitmap backdrop;
    static readonly Color Navy=Color.FromArgb(14,61,88),Blue=Color.FromArgb(2,110,164),Green=Color.FromArgb(74,155,20);
    static void F(Graphics g,Color c,RectangleF r){PaintTools.Fill(g,c,r);}
    static GraphicsPath Shape(RectangleF r,float radius){float d=Math.Min(radius*2,Math.Min(r.Width,r.Height));var p=new GraphicsPath();p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
    public static Region WindowRegion(int width,int height){using(var p=Shape(new RectangleF(0,0,width,height),10*width/300f))return new Region(p);}
    static float Unit(ClockSettings s){return System.Math.Max(1,300f/s.Width);}
    public static float TextSize(float requested,int width){return System.Math.Max(12,requested)*System.Math.Max(1,300f/width);}
    static int Columns(ClockSettings s){return s.Width<300?1:2;}
    public static int Height(ClockSettings s){int columns=Columns(s);return (int)System.Math.Ceiling((36+(s.ShowClock?74:0)+24+(s.Expanded?90:74)*(2/columns)+(s.Metrics.Count>0?24+((s.Metrics.Count+columns-1)/columns)*50:0)+32)*Unit(s));}
    static string Font(ClockSettings s){return s.Font=="mono"?"Consolas":s.Font=="serif"?"Georgia":s.Font=="condensed"?"Impact":s.Font=="rounded"?"Trebuchet MS":"Segoe UI";}
    static void T(Graphics g,string value,float size,Color c,float x,float y,ClockSettings s,bool bold=false){PaintTools.Text(g,value,TextSize(size,s.Width),c,x,y,Font(s),bold?FontStyle.Bold:FontStyle.Regular);}
    static void Fit(Graphics g,string value,float size,Color c,RectangleF r,ClockSettings s){using(var f=EmbeddedFonts.Make(Font(s),TextSize(size,s.Width),FontStyle.Regular))using(var b=new SolidBrush(c))using(var sf=new StringFormat {Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap,LineAlignment=StringAlignment.Center})g.DrawString(value,f,b,r,sf);}
    static string P(double? n){return n.HasValue?n.Value.ToString("0.#")+"%":"—";}
    static void Glass(Graphics g,RectangleF r){using(var shadow=Shape(new RectangleF(r.X+1,r.Y+2,r.Width,r.Height),8))using(var b=new SolidBrush(Color.FromArgb(35,0,53,93)))g.FillPath(b,shadow);
        using(var p=Shape(r,8))using(var b=new LinearGradientBrush(r,Color.FromArgb(225,255,255,255),Color.FromArgb(164,216,246,255),90f)){g.FillPath(b,p);var saved=g.Save();g.SetClip(p,CombineMode.Intersect);using(var shine=new LinearGradientBrush(new RectangleF(r.X,r.Y,r.Width,r.Height/2),Color.FromArgb(140,255,255,255),Color.FromArgb(0,255,255,255),90f))g.FillRectangle(shine,r.X,r.Y,r.Width,r.Height/2);g.Restore(saved);using(var border=new Pen(Color.FromArgb(230,255,255,255)))g.DrawPath(border,p);}
    }
    static void Orb(Graphics g,RectangleF r,Color color){using(var p=new GraphicsPath()){p.AddEllipse(r);using(var b=new PathGradientBrush(p)){b.CenterColor=Color.FromArgb(250,255,255,255);b.CenterPoint=new PointF(r.X+r.Width*.32f,r.Y+r.Height*.25f);b.SurroundColors=new[]{color};g.FillPath(b,p);}using(var border=new Pen(Color.FromArgb(210,255,255,255)))g.DrawPath(border,p);}using(var shine=new SolidBrush(Color.FromArgb(130,255,255,255)))g.FillEllipse(shine,r.X+r.Width*.18f,r.Y+r.Height*.08f,r.Width*.55f,r.Height*.3f);}
    static void Bar(Graphics g,RectangleF r,double? n,Color c){using(var p=Shape(r,4)){using(var b=new SolidBrush(Color.FromArgb(120,71,114,139)))g.FillPath(b,p);if(n.HasValue&&n.Value>0){var saved=g.Save();g.SetClip(p,CombineMode.Intersect);float w=(float)(r.Width*Math.Max(0,Math.Min(100,n.Value))/100);using(var b=new LinearGradientBrush(new RectangleF(r.X,r.Y,Math.Max(1,w),r.Height),Color.FromArgb(230,255,255),c,90f))g.FillRectangle(b,r.X,r.Y,w,r.Height);g.Restore(saved);}using(var border=new Pen(Color.FromArgb(150,255,255,255)))g.DrawPath(border,p);}}
    public static Bitmap ControlIcon(string kind){var image=new Bitmap(20,20);using(var g=Graphics.FromImage(image)){g.SmoothingMode=SmoothingMode.AntiAlias;using(var p=Shape(new RectangleF(1,1,18,18),3)){using(var b=new LinearGradientBrush(new Rectangle(1,1,18,18),kind=="close"?Color.FromArgb(251,182,163):Color.FromArgb(171,228,247),kind=="close"?Color.FromArgb(173,57,36):Color.FromArgb(39,112,153),90f))g.FillPath(b,p);using(var b=new Pen(Color.FromArgb(220,245,255)))g.DrawPath(b,p);}using(var pen=new Pen(Color.White,1.3f)){if(kind=="close"){g.DrawLine(pen,6,6,14,14);g.DrawLine(pen,14,6,6,14);}else if(kind=="minimize")g.DrawLine(pen,5,13,15,13);else if(kind=="expand"){g.DrawRectangle(pen,5,5,10,9);g.DrawLine(pen,5,8,15,8);}else{g.DrawEllipse(pen,5,5,10,10);g.DrawEllipse(pen,8,8,4,4);}}}return image;}
    public static void Draw(Graphics g,ClockSettings s,Theme t,UsageResult u,HardwareSample sample,Image atlas,bool photo,bool reading,string error){
        int h=Height(s);
        if(!photo){if(backdrop==null)using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("AeroAquatic"))using(var im=Image.FromStream(stream))backdrop=new Bitmap(im);float scale=Math.Max(300f/backdrop.Width,(float)h/backdrop.Height);float w=backdrop.Width*scale,hh=backdrop.Height*scale;g.DrawImage(backdrop,new RectangleF((300-w)/2,h-hh,w,hh));if(s.Fade>0)F(g,Color.FromArgb((int)(s.Fade*255),Color.Black),new RectangleF(0,0,300,h));}
        using(var header=new LinearGradientBrush(new Rectangle(0,0,300,31),Color.FromArgb(155,87,160,194),Color.FromArgb(175,12,88,136),90f))g.FillRectangle(header,0,0,300,31);
        float k=Unit(s);int cols=Columns(s);float cell=cols==1?276:134,y=36*k;
        Orb(g,new RectangleF(9,5,20,20),Color.FromArgb(24,144,191));T(g,"Relógio IA",12,Color.FromArgb(110,0,44,82),37,9,s);T(g,"Relógio IA",12,Color.White,36,8,s);
        if(s.ShowClock){Glass(g,new RectangleF(12,y,276,64*k));PaintTools.Digits(g,DateTime.Now.ToString("HH:mm"),new RectangleF(24,y+10*k,153,41*k),t.Accent,s.ClockStyle,atlas,s.TintDigits,Font(s));PaintTools.Line(g,Color.FromArgb(90,54,124,157),194,y+9*k,194,y+54*k);T(g,DateTime.Now.ToString("dd/MM"),15,t.Text,207,y+11*k,s);T(g,DateTime.Now.ToString("ddd"),12,Navy,207,y+36*k,s);y+=74*k;}
        Glass(g,new RectangleF(12,y,276,20*k));T(g,"Consumo do "+u.Provider,12,Navy,21,y+2*k,s);y+=24*k;
        float quotaStep=(s.Expanded?90:74)*k;
        for(int i=0;i<2;i++){var q=u.Windows.Count>i?u.Windows[i]:null;float x=12+i%cols*142,yy=y+i/cols*quotaStep,hh=(s.Expanded?84:68)*k;
            Glass(g,new RectangleF(x,yy,cell,hh));Orb(g,new RectangleF(x+10,yy+10*k,10*k,10*k),i==0?Blue:Green);T(g,q==null?(i==0?"5 horas":"Semanal"):q.Label,12,Navy,x+28,yy+6*k,s);T(g,q==null?"—":P(q.Used),22,t.Text,x+11,yy+25*k,s);Bar(g,new RectangleF(x+11,yy+54*k,cell-22,5*k),q==null?(double?)null:q.Used,q!=null&&q.Used>=90?Color.FromArgb(218,94,52):i==0?t.Accent:Green);if(s.Expanded)Fit(g,q==null?"Sem leitura":q.ResetText(DateTime.UtcNow),12,Navy,new RectangleF(x+11,yy+63*k,cell-22,18*k),s);
        }y+=(2/cols)*quotaStep;
        if(s.Metrics.Count>0){Glass(g,new RectangleF(12,y,276,20*k));T(g,"Meu computador",12,Navy,21,y+2*k,s);y+=24*k;
            for(int i=0;i<s.Metrics.Count;i++){string key=s.Metrics[i];double? n;sample.Values.TryGetValue(key,out n);float x=12+i%cols*142,yy=y+i/cols*50*k;Glass(g,new RectangleF(x,yy,cell,44*k));AeroIcons.Draw(g,key,new RectangleF(x+9,yy+8*k,24*k,24*k));Fit(g,key=="disk"?sample.DiskName:key.ToUpperInvariant(),12,Navy,new RectangleF(x+43,yy+2*k,cell-51,17*k),s);T(g,P(n),16,t.Text,x+43,yy+18*k,s);Bar(g,new RectangleF(x+10,yy+38*k,cell-20,3*k),n,Green);}
        }
        Glass(g,new RectangleF(12,h-28*k,276,22*k));Fit(g,reading?"Consultando "+u.Provider+"…":error!=""?(u.At.HasValue?"Leitura anterior · F5 atualiza":"Sem leitura · F5 tenta novamente"):u.At.HasValue?"Lido "+u.At.Value.ToLocalTime().ToString("HH:mm:ss")+" · "+s.RefreshSeconds+"s":"Aguardando leitura da conta",12,Navy,new RectangleF(21,h-27*k,258,20*k),s);
        using(var frame=Shape(new RectangleF(1,1,297,h-3),9))using(var p=new Pen(Color.FromArgb(235,234,255,255),1.2f))g.DrawPath(p,frame);
    }
}
