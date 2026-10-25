using System;
using System.Drawing;
using System.Drawing.Drawing2D;

// Original symbols for the publication edition. No imported application logos.
public static class PublicArt {
    public static void Wave(Graphics g,RectangleF r,Color color){var saved=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/32,r.Height/32);g.SmoothingMode=SmoothingMode.AntiAlias;using(var p=new Pen(color,2.5f)){g.DrawArc(p,3,3,26,26,35,290);g.DrawLines(p,new[]{new PointF(4,17),new PointF(10,17),new PointF(13,10),new PointF(18,23),new PointF(22,15),new PointF(28,15)});}g.Restore(saved);}
    public static void Bars(Graphics g,RectangleF r,Color color){var saved=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/48,r.Height/32);using(var b=new SolidBrush(color)){g.FillRectangle(b,4,19,8,10);g.FillRectangle(b,17,12,8,17);g.FillRectangle(b,30,5,8,24);g.FillEllipse(b,40,1,6,6);}g.Restore(saved);}
    public static void Hardware(Graphics g,string key,RectangleF r){var saved=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/32,r.Height/32);g.SmoothingMode=SmoothingMode.AntiAlias;Color light=Color.FromArgb(181,208,223),dark=Color.FromArgb(59,88,112);using(var b=new LinearGradientBrush(new Rectangle(2,3,28,26),light,dark,60f))using(var p=new Pen(Color.FromArgb(44,62,81),1)){
        if(key=="cpu"){g.FillPolygon(b,new[]{new Point(4,11),new Point(18,4),new Point(28,13),new Point(14,22)});g.DrawPolygon(p,new[]{new Point(4,11),new Point(18,4),new Point(28,13),new Point(14,22)});using(var chip=new SolidBrush(Color.FromArgb(61,91,128)))g.FillPolygon(chip,new[]{new Point(10,11),new Point(18,7),new Point(23,12),new Point(15,17)});for(int i=0;i<4;i++){g.DrawLine(p,5+i*3,15+i*2,5+i*3,20+i*2);g.DrawLine(p,18+i*3,22-i*2,18+i*3,27-i*2);}}
        else if(key=="gpu"){using(var board=new SolidBrush(Color.FromArgb(62,142,98)))g.FillPolygon(board,new[]{new Point(3,10),new Point(26,5),new Point(29,22),new Point(6,27)});g.FillEllipse(b,10,11,13,12);g.DrawEllipse(p,10,11,13,12);g.DrawLine(p,12,12,21,21);g.DrawLine(p,21,12,12,21);using(var gold=new SolidBrush(Color.FromArgb(218,176,73)))g.FillRectangle(gold,8,25,16,3);}
        else if(key=="ram"){using(var board=new SolidBrush(Color.FromArgb(66,133,86)))g.FillPolygon(board,new[]{new Point(2,13),new Point(27,6),new Point(30,19),new Point(5,26)});for(int i=0;i<3;i++){g.FillRectangle(b,6+i*7,14-i*2,5,7);g.DrawRectangle(p,6+i*7,14-i*2,5,7);}using(var gold=new Pen(Color.FromArgb(224,183,79),2))g.DrawLine(gold,7,25,28,19);}
        else{g.FillPolygon(b,new[]{new Point(3,9),new Point(22,5),new Point(28,23),new Point(9,28)});g.DrawPolygon(p,new[]{new Point(3,9),new Point(22,5),new Point(28,23),new Point(9,28)});using(var disk=new SolidBrush(Color.FromArgb(226,234,240)))g.FillEllipse(disk,8,10,14,13);g.DrawEllipse(p,8,10,14,13);g.DrawLine(p,15,17,24,23);}}
        g.Restore(saved);
    }
}
