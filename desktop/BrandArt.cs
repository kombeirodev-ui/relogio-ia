using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

public static class BrandArt {
    static Bitmap symbol;
    static Rectangle symbolBounds;
    public static void Otimize(Graphics g,RectangleF area){
        if(symbol==null){using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream("OtimizeSymbol"))using(var b=new Bitmap(s))symbol=new Bitmap(b);
            int left=symbol.Width,top=symbol.Height,right=0,bottom=0;
            for(int y=0;y<symbol.Height;y++)for(int x=0;x<symbol.Width;x++)if(symbol.GetPixel(x,y).A>12){left=Math.Min(left,x);top=Math.Min(top,y);right=Math.Max(right,x);bottom=Math.Max(bottom,y);}
            symbolBounds=Rectangle.FromLTRB(left,top,right+1,bottom+1);
        }
        float scale=Math.Min(area.Width/symbolBounds.Width,area.Height/symbolBounds.Height);float w=symbolBounds.Width*scale,h=symbolBounds.Height*scale;
        g.DrawImage(symbol,new RectangleF(area.X+(area.Width-w)/2,area.Y+(area.Height-h)/2,w,h),symbolBounds,GraphicsUnit.Pixel);
    }
    public static void Flower(Graphics g,RectangleF area){var state=g.Save();g.TranslateTransform(area.X,area.Y);g.ScaleTransform(area.Width/32,area.Height/32);g.SmoothingMode=SmoothingMode.AntiAlias;using(var p=new Pen(Color.FromArgb(47,122,65),2.5f))g.DrawLine(p,16,28,16,14);using(var path=new GraphicsPath()){path.AddBezier(16,19,1,20,1,7,4,6);path.AddBezier(4,6,17,5,20,15,16,19);using(var b=new SolidBrush(Color.FromArgb(77,191,96)))g.FillPath(b,path);}using(var path=new GraphicsPath()){path.AddBezier(17,13,16,2,28,1,30,3);path.AddBezier(30,3,31,14,23,18,17,13);using(var b=new SolidBrush(Color.FromArgb(144,220,88)))g.FillPath(b,path);}using(var b=new SolidBrush(Color.FromArgb(255,210,74)))g.FillEllipse(b,23,22,6,6);g.Restore(state);}
    public static void Buddy(Graphics g,RectangleF area,bool online){var saved=g.Save();g.TranslateTransform(area.X,area.Y);g.ScaleTransform(area.Width/40,area.Height/40);g.SmoothingMode=SmoothingMode.AntiAlias;using(var b=new LinearGradientBrush(new Rectangle(2,2,35,35),Color.White,Color.FromArgb(147,197,232),90f))g.FillEllipse(b,2,2,35,35);Person(g,10,5,Color.FromArgb(182,215,246),Color.FromArgb(45,112,177));using(var b=new SolidBrush(online?Color.FromArgb(106,194,67):Color.Gray))g.FillEllipse(b,28,28,10,10);using(var p=new Pen(Color.White,2))g.DrawEllipse(p,28,28,10,10);g.Restore(saved);}
    static void Person(Graphics g,int x,int y,Color light,Color dark){using(var b=new LinearGradientBrush(new Rectangle(x,y,20,31),light,dark,25f)){
        using(var p=new GraphicsPath()){p.AddBezier(x+1,y+29,x+1,y+18,x+4,y+15,x+10,y+15);p.AddBezier(x+10,y+15,x+17,y+15,x+19,y+20,x+19,y+29);p.AddBezier(x+19,y+29,x+14,y+32,x+6,y+32,x+1,y+29);p.CloseFigure();g.FillPath(b,p);using(var pen=new Pen(Color.FromArgb(140,dark),.6f))g.DrawPath(pen,p);}
        using(var head=new GraphicsPath()){head.AddEllipse(x+4,y,13,14);using(var shine=new PathGradientBrush(head)){shine.CenterPoint=new PointF(x+8,y+4);shine.CenterColor=Color.FromArgb(240,light);shine.SurroundColors=new[]{dark};g.FillPath(shine,head);}using(var p=new Pen(Color.FromArgb(150,dark),.6f))g.DrawPath(p,head);}
    }}
    public static void Controller(Graphics g,RectangleF r,Color color){var saved=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/28,r.Height/22);using(var b=new SolidBrush(color)){g.FillRectangle(b,2,8,5,12);g.FillRectangle(b,11,2,5,18);g.FillRectangle(b,20,6,5,14);}g.Restore(saved);}
    public static void Sensor(Graphics g,string key,RectangleF r,Color color){var saved=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/20,r.Height/20);using(var p=new Pen(color,1.2f)){
        if(key=="cpu"){g.DrawRectangle(p,4,4,12,12);g.DrawRectangle(p,7,7,6,6);for(int i=6;i<16;i+=4){g.DrawLine(p,i,1,i,4);g.DrawLine(p,i,16,i,19);g.DrawLine(p,1,i,4,i);g.DrawLine(p,16,i,19,i);}}
        else if(key=="gpu"){g.DrawRectangle(p,1,4,17,12);g.DrawEllipse(p,5,6,8,8);g.DrawLine(p,1,17,1,19);g.DrawLine(p,14,17,18,17);}
        else if(key=="ram"){g.DrawRectangle(p,1,5,18,10);for(int i=3;i<18;i+=5){g.DrawRectangle(p,i,7,3,5);g.DrawLine(p,i,16,i,18);}}
        else{g.DrawRectangle(p,2,2,16,16);g.DrawEllipse(p,5,5,9,9);g.DrawLine(p,10,10,16,16);}
    }g.Restore(saved);}
    public static Bitmap ControlIcon(string kind,Theme t){var image=new Bitmap(20,20);using(var g=Graphics.FromImage(image)){g.SmoothingMode=SmoothingMode.AntiAlias;if(t.Id=="msn"){using(var b=new LinearGradientBrush(new Rectangle(0,0,20,20),kind=="close"?Color.FromArgb(244,156,116):Color.FromArgb(130,174,249),kind=="close"?Color.FromArgb(193,53,25):Color.FromArgb(28,82,186),90f))g.FillRectangle(b,1,1,18,18);using(var border=new Pen(Color.FromArgb(220,235,255)))g.DrawRectangle(border,1,1,18,18);}Color c=t.Id=="msn"?Color.White:t.Text;using(var p=new Pen(c,1.35f)){
        if(kind=="close"){g.DrawLine(p,6,6,14,14);g.DrawLine(p,14,6,6,14);}
        else if(kind=="minimize")g.DrawLine(p,5,13,15,13);
        else if(kind=="expand"){g.DrawRectangle(p,5,5,10,10);g.DrawLine(p,5,8,15,8);}
        else{for(int i=0;i<3;i++){int y=5+i*5;g.DrawLine(p,4,y,16,y);int x=i==1?12:7;PaintTools.Fill(g,t.Header,new RectangleF(x-2,y-2,4,4));g.DrawEllipse(p,x-2,y-2,4,4);}}
    }}return image;}
}
