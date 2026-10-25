using System;
using System.Drawing;
using System.Drawing.Drawing2D;

public static class MusicPanel {
    public static int Height(ClockSettings s){return s.ShowMusic?126:0;}
    public static RectangleF ControlBounds(int index,int y){return index<3?new RectangleF(13+index*32,y+64,27,22):index==3?new RectangleF(109,y+64,47,22):new RectangleF(161,y+64,29,22);}
    public static void VolumeTrack(Graphics g,Theme t,int? percent){if(t.Id=="winamp"){WinampSkin.DrawVolumeTrack(g,percent);return;}PaintTools.Fill(g,t.Panel,new RectangleF(0,0,158,22));PaintTools.Fill(g,t.Inset,new RectangleF(5,8,148,6));if(percent.HasValue){PaintTools.Fill(g,t.Accent,new RectangleF(5,8,148*percent.Value/100f,6));PaintTools.Fill(g,t.Text,new RectangleF(5+140*percent.Value/100f,4,8,14));}using(var pen=new Pen(t.Border))g.DrawRectangle(pen,5,8,148,6);}
    static void Text(Graphics g,string value,float size,Color color,RectangleF area,string font,bool bold=false){using(var f=EmbeddedFonts.Make(font,size,bold?FontStyle.Bold:FontStyle.Regular))using(var b=new SolidBrush(color))using(var sf=new StringFormat {Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap,LineAlignment=StringAlignment.Center})g.DrawString(value,f,b,area,sf);}
    public static void Draw(Graphics g,ClockSettings s,Theme t,MusicState music,int y){
        if(!s.ShowMusic)return;
        if(t.Id=="winamp"){WinampSkin.DrawMusic(g,music,y);return;}
        bool discord=t.Id=="discord",msn=t.Id=="msn",xp=t.Id=="xp",classic=RetroSkins.Handles(t.Id)||t.Id=="icq";
        Color panel=discord?Theme.C("#232428"):msn?Theme.C("#f6fbff"):xp?Theme.C("#ece9d8"):t.Panel;
        PaintTools.Fill(g,t.Background,new RectangleF(0,y,300,126));PaintTools.Fill(g,panel,new RectangleF(8,y+3,284,118));using(var pen=new Pen(t.Border))g.DrawRectangle(pen,8,y+3,284,118);
        string font=msn||xp||classic?"Tahoma":discord?"Inter":"Segoe UI";
        Color heading=discord?Theme.C("#23a55a"):msn?Theme.C("#447997"):t.Text;
        if(msn)Text(g,"♫ "+(music.Available?music.Description:"O que estou ouvindo"),10,heading,new RectangleF(s.ShowClock?83:42,s.ShowClock?98:55,s.ShowClock?204:245,17),"Tahoma");
        string caption=msn?"♫ O que estou ouvindo":discord?"OUVINDO SPOTIFY":"Spotify";
        Text(g,caption,10,heading,new RectangleF(15,y+7,270,17),font,discord||xp);
        Text(g,music.Available?music.Title:music.Error!=""?music.Error:"Abra o Spotify e escolha uma música",12,t.Text,new RectangleF(15,y+26,270,19),font,true);
        string subtitle=music.Available?(music.Artist!=""?music.Artist:music.Playing?"Reproduzindo":"Pausado"):"Controles locais · sem conectar sua conta";
        if(music.Playlist!="")subtitle+=" · "+music.Playlist;
        Text(g,subtitle,10,t.Muted,new RectangleF(15,y+45,270,16),font);
        string status=music.Available&&music.Error!=""?music.Error:music.Available?(music.Playing?"Reproduzindo":"Pausado"):"Sem sessão";
        Text(g,status,9,t.Muted,new RectangleF(199,y+66,85,18),font);Text(g,music.Volume.Available?music.Volume.Percent.ToString():"—",9,t.Muted,new RectangleF(273,y+97,20,18),font);
    }
    public static Bitmap ButtonImage(string action,Theme t,bool playing){
        if(t.Id=="winamp")return WinampSkin.MusicButton(action,playing);
        int width=action=="shuffle"?47:action.StartsWith("repeat")?29:27;var bitmap=new Bitmap(width,22);using(var g=Graphics.FromImage(bitmap)){
            g.Clear(t.Panel);bool retro=t.Id=="xp"||t.Id=="msn"||RetroSkins.Handles(t.Id)||t.Id=="icq";
            using(var b=new LinearGradientBrush(new Rectangle(0,0,width,22),retro?Color.White:t.Panel,retro?Color.FromArgb(200,211,224):t.Inset,90f))g.FillRectangle(b,0,0,width,22);
            using(var p=new Pen(t.Border))g.DrawRectangle(p,0,0,width-1,21);g.TranslateTransform((width-27)/2f,0);
            Color color=(action=="shuffle"||action.StartsWith("repeat"))&&playing?t.Accent:t.Id=="discord"?Color.White:t.Text;
            using(var brush=new SolidBrush(color)){
                if(action=="volume-down"||action=="volume-up"){g.FillRectangle(brush,7,10,13,2);if(action=="volume-up")g.FillRectangle(brush,13,5,2,12);}
                else if(action=="mute"){g.FillRectangle(brush,6,8,4,6);g.FillPolygon(brush,new[]{new Point(10,8),new Point(15,5),new Point(15,17),new Point(10,14)});using(var pen=new Pen(color,1.5f)){if(playing){g.DrawLine(pen,18,8,23,14);g.DrawLine(pen,23,8,18,14);}else g.DrawArc(pen,14,6,9,10,-60,120);}}
                else if(action=="shuffle"){using(var pen=new Pen(color,1.5f)){g.DrawLines(pen,new[]{new Point(5,6),new Point(10,6),new Point(18,16),new Point(22,16)});g.DrawLines(pen,new[]{new Point(5,16),new Point(10,16),new Point(18,6),new Point(22,6)});g.DrawLine(pen,19,3,22,6);g.DrawLine(pen,19,19,22,16);}}
                else if(action.StartsWith("repeat")){using(var pen=new Pen(color,1.5f)){g.DrawArc(pen,5,5,17,12,35,285);g.DrawLine(pen,21,6,22,11);g.DrawLine(pen,22,11,17,9);}if(action=="repeat-one")Text(g,"1",8,color,new RectangleF(10,6,7,10),"Tahoma");}
                else if(action=="toggle"&&playing){g.FillRectangle(brush,9,6,3,10);g.FillRectangle(brush,15,6,3,10);}
                else if(action=="toggle")g.FillPolygon(brush,new[]{new Point(10,5),new Point(10,17),new Point(19,11)});
                else if(action=="previous"){g.FillRectangle(brush,7,6,2,10);g.FillPolygon(brush,new[]{new Point(18,6),new Point(18,16),new Point(10,11)});}
                else{g.FillRectangle(brush,18,6,2,10);g.FillPolygon(brush,new[]{new Point(9,6),new Point(9,16),new Point(17,11)});}
            }
        }return bitmap;
    }
}
