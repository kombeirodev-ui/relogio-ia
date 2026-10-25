using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public class Theme {
    public static readonly string[] Ids={"winamp","otimizepc","icq","msn","discord","napster","kazaa","aero","xp"};
    public string Id,Name;
    public Color Background,Panel,Inset,Text,Muted,Accent,Border,Header;
    public static Theme Get(string id) {
        if(id=="icq")return new Theme {Id=id,Name="JARDIM",Background=C("#d6d4cb"),Panel=C("#f6f5e9"),Inset=C("#ffffff"),Text=C("#202b30"),Muted=C("#52615b"),Accent=C("#268a2b"),Border=C("#77796e"),Header=C("#13357e")};
        if(id=="otimizepc")return new Theme {Id=id,Name="OTIMIZEPC",Background=C("#0F172A"),Panel=C("#18243b"),Inset=C("#111d32"),Text=C("#f4f7fc"),Muted=C("#b3c0d5"),Accent=C("#FFC907"),Border=C("#344158"),Header=C("#111d32")};
        if(id=="msn")return new Theme {Id=id,Name="CONEXÃO",Background=C("#e7f3fa"),Panel=C("#ffffff"),Inset=C("#f7fbfd"),Text=C("#243f53"),Muted=C("#536f82"),Accent=C("#198bc7"),Border=C("#8cb5cd"),Header=C("#2c66d3")};
        if(id=="discord")return new Theme {Id=id,Name="COMUNIDADE",Background=C("#313338"),Panel=C("#2b2d31"),Inset=C("#232428"),Text=C("#f2f3f5"),Muted=C("#b5bac1"),Accent=C("#949cff"),Border=C("#41434a"),Header=C("#313338")};
        if(id=="napster")return new Theme {Id=id,Name="FREQUÊNCIA",Background=C("#c0c0c0"),Panel=C("#ffffff"),Inset=C("#dfdfdf"),Text=C("#000000"),Muted=C("#454545"),Accent=C("#000080"),Border=C("#808080"),Header=C("#000080")};
        if(id=="kazaa")return new Theme {Id=id,Name="LIMA",Background=C("#ece9d8"),Panel=C("#ffffff"),Inset=C("#f4f4ed"),Text=C("#000000"),Muted=C("#505048"),Accent=C("#a4cb25"),Border=C("#9b9b8f"),Header=C("#b8ccdf")};
        if(id=="xp")return new Theme {Id=id,Name="AZUL CLÁSSICO",Background=C("#ece9d8"),Panel=C("#ffffff"),Inset=C("#ffffff"),Text=C("#222222"),Muted=C("#4c4c4c"),Accent=C("#0054e3"),Border=C("#0055e5"),Header=C("#0055e5")};
        if(id=="aero")return new Theme {Id=id,Name="BRISA",Background=C("#53c9e9"),Panel=C("#e9f9ff"),Inset=C("#ccecf5"),Text=C("#123d58"),Muted=C("#285b75"),Accent=C("#046fa6"),Border=C("#d4f8ff"),Header=C("#4b9dcb")};
        return new Theme {Id="winamp",Name="METAL",Background=C("#29293f"),Panel=C("#414156"),Inset=C("#080d0e"),Text=C("#dfe5e5"),Muted=C("#97ac97"),Accent=C("#00f700"),Border=C("#d7c68b"),Header=C("#29293f")};
    }
    public static Color C(string value){return ColorTranslator.FromHtml(value);}
}
public static class PaintTools {
    public static void RoundBox(Graphics g,Theme t,RectangleF r) {using(var p=new GraphicsPath()){float d=12;p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();using(var b=new SolidBrush(t.Panel))g.FillPath(b,p);using(var pen=new Pen(t.Border))g.DrawPath(pen,p);}}
    public static void Fill(Graphics g,Color c,RectangleF r){using(var b=new SolidBrush(c))g.FillRectangle(b,r);}
    public static void Line(Graphics g,Color c,float x1,float y1,float x2,float y2){using(var p=new Pen(c))g.DrawLine(p,x1,y1,x2,y2);}
    public static void Text(Graphics g,string value,float size,Color color,float x,float y,string family="Segoe UI",FontStyle style=FontStyle.Regular) {using(var font=EmbeddedFonts.Make(family,size,style))using(var b=new SolidBrush(color))g.DrawString(value,font,b,x,y,StringFormat.GenericTypographic);}
    public static void Box(Graphics g,Theme t,RectangleF r) {
        Fill(g,t.Inset,r);using(var p=new Pen(t.Border))g.DrawRectangle(p,r.X,r.Y,r.Width,r.Height);
        if(t.Id!="otimizepc"){Line(g,Color.FromArgb(70,Color.White),r.Left+1,r.Bottom-1,r.Right-1,r.Bottom-1);Line(g,Color.FromArgb(80,Color.Black),r.Left+1,r.Top+1,r.Right-1,r.Top+1);}
    }
    public static void Header(Graphics g,Theme t) {
        Fill(g,t.Header,new RectangleF(0,0,300,26));
        if(t.Id=="winamp"){for(int i=0;i<2;i++){Line(g,t.Border,7,9+i*5,55,9+i*5);Line(g,t.Border,159,9+i*5,202,9+i*5);}Text(g,"RELÓGIO IA",11,t.Text,63,7,"Consolas",FontStyle.Bold);}
        else if(t.Id=="icq"){BrandArt.Flower(g,new RectangleF(5,2,22,22));Text(g,"Relógio IA",11,Color.White,31,7,"Tahoma",FontStyle.Bold);}
        else{Fill(g,t.Accent,new RectangleF(10,8,3,11));Text(g,"OTIMIZEPC",11,t.Text,20,7,"IBM Plex Sans",FontStyle.Bold);Text(g,"/ IA",9,t.Muted,90,9,"IBM Plex Mono");}
    }
    public static Bitmap Icon(string kind,Color color) {
        var image=new Bitmap(20,20);using(var g=Graphics.FromImage(image)){g.SmoothingMode=SmoothingMode.AntiAlias;using(var p=new Pen(color,1.5f)) {
            if(kind=="close"){g.DrawLine(p,6,6,14,14);g.DrawLine(p,14,6,6,14);}
            else if(kind=="expand"){g.DrawRectangle(p,5,5,10,10);g.DrawLine(p,7,10,13,10);g.DrawLine(p,10,7,10,13);}
            else{g.DrawEllipse(p,5,5,10,10);g.DrawEllipse(p,8,8,4,4);for(int i=0;i<8;i++){double a=i*Math.PI/4;g.DrawLine(p,10+(float)Math.Cos(a)*6,10+(float)Math.Sin(a)*6,10+(float)Math.Cos(a)*8,10+(float)Math.Sin(a)*8);}}
        }}return image;
    }
    public static void Digits(Graphics g,string time,RectangleF area,Color color,string style,Image atlas,bool tint,string font) {
        if(style=="atlas"&&atlas!=null){float cell=area.Width/5;using(var a=new ImageAttributes()){
            if(tint)a.SetColorMatrix(new ColorMatrix(new float[][]{new float[]{0,0,0,0,0},new float[]{0,0,0,0,0},new float[]{0,0,0,0,0},new float[]{0,0,0,1,0},new float[]{color.R/255f,color.G/255f,color.B/255f,0,1}}));
            for(int n=0;n<time.Length;n++){int i=time[n]==':'?10:time[n]-'0';g.DrawImage(atlas,Rectangle.Round(new RectangleF(area.X+n*cell,area.Y,cell,area.Height)),i*atlas.Width/11f,0,atlas.Width/11f,atlas.Height,GraphicsUnit.Pixel,a);}}return;}
        if(style=="text"||style=="outline"){using(var path=new GraphicsPath())using(var textFont=EmbeddedFonts.Make(font,area.Height,FontStyle.Regular)){path.AddString(time,textFont.FontFamily,0,area.Height,new PointF(area.X,area.Y-5),StringFormat.GenericTypographic);if(style=="outline"){using(var p=new Pen(color,1.2f))g.DrawPath(p,path);}else using(var b=new SolidBrush(color))g.FillPath(b,path);}return;}
        var active=new[]{"abcdef","bc","abdeg","abcdg","bcfg","acdfg","acdefg","abc","abcdefg","abcdfg"};
        var points=new[]{"5,1 23,1 26,4 23,7 5,7 2,4","24,7 27,4 27,21 24,24 21,21 21,10","24,25 27,28 27,44 24,47 21,44 21,28","5,42 22,42 25,45 22,48 5,48 2,45","3,25 6,28 6,40 3,44 0,41 0,28","3,5 6,8 6,20 3,23 0,20 0,8","5,21 22,21 25,24 22,27 5,27 2,24"};
        var saved=g.Save();g.TranslateTransform(area.X,area.Y);float scale=Math.Min(area.Height/49f,area.Width/131f);g.ScaleTransform(scale,scale);float x=0;
        foreach(char ch in time){if(ch==':'){Fill(g,color,new RectangleF(x+3,14,4,5));Fill(g,color,new RectangleF(x+3,31,4,5));x+=12;continue;}
            for(int i=0;i<7;i++){var polygon=Array.ConvertAll(points[i].Split(' '),s=>{var xy=s.Split(',');return new PointF(x+Int32.Parse(xy[0]),Int32.Parse(xy[1]));});using(var b=new SolidBrush(active[ch-'0'].IndexOf((char)('a'+i))>=0?color:Color.FromArgb(22,color)))g.FillPolygon(b,polygon);}x+=30;
        }g.Restore(saved);
    }
}
