using System.Drawing;
using System.Drawing.Drawing2D;

// Original small, front-facing hardware illustrations. Materials and light from
// upper left; no cast shadow at these sizes. Not extracted Windows resources.
public static class AeroIcons {
    static void Material(Graphics g,RectangleF r,Color light,Color dark){using(var b=new LinearGradientBrush(r,light,dark,55f))g.FillRectangle(b,r);using(var p=new Pen(Color.FromArgb(65,82,90),.8f))g.DrawRectangle(p,r.X,r.Y,r.Width,r.Height);}
    public static void Draw(Graphics g,string key,RectangleF area){var state=g.Save();g.TranslateTransform(area.X,area.Y);g.ScaleTransform(area.Width/32,area.Height/32);g.SmoothingMode=SmoothingMode.AntiAlias;
        Color silver=Color.FromArgb(238,246,250),steel=Color.FromArgb(129,155,168),gold=Color.FromArgb(193,157,69),green=Color.FromArgb(41,116,73);
        if(key=="cpu"){
            using(var pins=new Pen(gold,2))for(int i=10;i<=22;i+=4){g.DrawLine(pins,i,3,i,7);g.DrawLine(pins,i,25,i,29);g.DrawLine(pins,3,i,7,i);g.DrawLine(pins,25,i,29,i);}
            Material(g,new RectangleF(6,6,20,20),Color.FromArgb(114,167,127),green);Material(g,new RectangleF(9,9,14,14),silver,steel);using(var p=new Pen(Color.White,.8f)){g.DrawLine(p,10,10,21,10);g.DrawLine(p,10,10,10,21);}using(var p=new Pen(Color.FromArgb(116,142,154),.6f))g.DrawRectangle(p,12,12,8,8);
        }else if(key=="gpu"){
            Material(g,new RectangleF(2,7,26,17),Color.FromArgb(127,178,118),green);Material(g,new RectangleF(28,4,3,23),silver,steel);using(var b=new SolidBrush(gold))g.FillRectangle(b,6,24,13,3);
            Material(g,new RectangleF(4,10,8,10),silver,steel);using(var p=new Pen(Color.FromArgb(103,126,139),.7f))for(int y=12;y<20;y+=2)g.DrawLine(p,5,y,11,y);
            using(var b=new SolidBrush(Color.FromArgb(176,193,204)))g.FillEllipse(b,14,9,12,12);using(var b=new SolidBrush(Color.FromArgb(39,57,67)))g.FillEllipse(b,15,10,10,10);using(var p=new Pen(Color.FromArgb(160,183,193),1.2f)){g.DrawLine(p,20,11,20,19);g.DrawLine(p,16,15,24,15);g.DrawLine(p,17,12,23,18);g.DrawLine(p,17,18,23,12);}using(var b=new SolidBrush(silver))g.FillEllipse(b,18,13,4,4);
        }else if(key=="ram"){
            Material(g,new RectangleF(2,9,28,14),Color.FromArgb(111,175,100),green);using(var b=new SolidBrush(gold))for(int x=3;x<29;x+=3)g.FillRectangle(b,x,23,2,4);
            for(int x=5;x<28;x+=8)Material(g,new RectangleF(x,12,6,8),Color.FromArgb(87,101,106),Color.FromArgb(23,39,44));using(var p=new Pen(Color.FromArgb(166,199,127),.7f))g.DrawLine(p,3,21,28,21);
        }else if(key=="disk"){
            Material(g,new RectangleF(5,2,22,28),silver,steel);using(var b=new LinearGradientBrush(new Rectangle(8,5,16,16),Color.White,Color.FromArgb(151,180,194),45f))g.FillEllipse(b,8,5,16,16);using(var p=new Pen(Color.FromArgb(106,140,159),.8f)){g.DrawEllipse(p,8,5,16,16);g.DrawEllipse(p,11,8,10,10);g.DrawLine(p,12,25,23,14);}using(var b=new SolidBrush(Color.FromArgb(73,119,145)))g.FillEllipse(b,14,11,4,4);using(var b=new SolidBrush(Color.FromArgb(70,99,111))){g.FillEllipse(b,10,23,4,4);g.FillRectangle(b,8,28,15,2);}
        }
        g.Restore(state);
    }
}
