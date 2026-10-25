using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Drawing.Imaging;

// Original metal player artwork; no third-party skin sprites.
public static class WinampSkin {
    public static readonly Color Green=Color.FromArgb(0,247,0);
    static readonly Color Ink=Color.FromArgb(8,13,14),Gold=Color.FromArgb(215,198,139),Pale=Color.FromArgb(223,229,229);
    static void Fill(Graphics g,Color c,float x,float y,float w,float h){PaintTools.Fill(g,c,new RectangleF(x,y,w,h));}
    static void Line(Graphics g,Color c,float x,float y,float x2,float y2){PaintTools.Line(g,c,x,y,x2,y2);}
    public static Color UsageColor(double value){value=Math.Max(0,Math.Min(100,value));if(value<=50)return Green;if(value<=60)return Blend(Green,Color.FromArgb(255,235,0),(value-50)/10);if(value<=80)return Blend(Color.FromArgb(255,235,0),Color.FromArgb(255,130,0),(value-60)/20);return Blend(Color.FromArgb(255,130,0),Color.FromArgb(255,40,24),(value-80)/20);}
    static Color Blend(Color a,Color b,double f){return Color.FromArgb((int)(a.R+(b.R-a.R)*f),(int)(a.G+(b.G-a.G)*f),(int)(a.B+(b.B-a.B)*f));}
    public static void PixelText(Graphics g,string text,float x,float y,Color color){using(var font=EmbeddedFonts.Make("IBM Plex Mono",8,FontStyle.Regular))using(var brush=new SolidBrush(color))g.DrawString(text.ToUpperInvariant(),font,brush,x,y-2,StringFormat.GenericTypographic);}
    public static int TextWidth(string text){int w=0;foreach(char c in text.Normalize(System.Text.NormalizationForm.FormD))if(CharUnicodeInfo.GetUnicodeCategory(c)!=UnicodeCategory.NonSpacingMark)w+=5;return w;}
    static void Center(Graphics g,string s,float x,float y,float w,Color c){PixelText(g,s,x+(w-TextWidth(s))/2,y,c);}
    static void Bevel(Graphics g,RectangleF r,bool raised){Color light=Color.FromArgb(130,135,155),dark=Color.FromArgb(8,8,19);Line(g,raised?light:dark,r.X,r.Y,r.Right,r.Y);Line(g,raised?light:dark,r.X,r.Y,r.X,r.Bottom);Line(g,raised?dark:light,r.X,r.Bottom,r.Right,r.Bottom);Line(g,raised?dark:light,r.Right,r.Y,r.Right,r.Bottom);}
    static void Metal(Graphics g,float y,float h){using(var brush=new LinearGradientBrush(new RectangleF(0,y,300,h),Color.FromArgb(67,69,84),Color.FromArgb(30,32,45),90f))g.FillRectangle(brush,0,y,300,h);for(float row=y+3;row<y+h;row+=4)Line(g,Color.FromArgb(12,Color.White),2,row,297,row);}
    static void Rail(Graphics g,int x,int y,int width){Fill(g,Color.FromArgb(15,14,22),x,y,width,5);Line(g,Color.FromArgb(250,233,171),x,y,x+width,y);Line(g,Gold,x,y+2,x+width,y+2);Line(g,Color.FromArgb(128,113,70),x,y+4,x+width,y+4);}
    static void Header(Graphics g,string title,int y,bool controls){Metal(g,y,18);string label=controls?(y==0?"RELOGIO IA":"MUSICA"):title=="HARDWARE"?"COMPUTADOR":"COTAS / DETALHES";int width=TextWidth(label);int left=(300-width)/2;Rail(g,17,y+5,Math.Max(5,left-24));Rail(g,left+width+7,y+5,Math.Max(5,278-left-width-7));Center(g,label,0,y+5,300,Pale);Line(g,Color.FromArgb(12,14,23),3,y+16,296,y+16);}
    static void Inset(Graphics g,float x,float y,float w,float h,bool dots){Fill(g,Ink,x,y,w,h);Bevel(g,new RectangleF(x-1,y-1,w+2,h+2),false);if(dots)for(int row=(int)y+1;row<y+h;row+=2)for(int col=(int)x+1;col<x+w;col+=2)Fill(g,Color.FromArgb(22,36,30),col,row,1,1);}
    static void Cap(Graphics g,float x,float y,bool vertical){var r=new RectangleF(x,y,vertical?13:17,10);using(var b=new LinearGradientBrush(r,Color.FromArgb(233,238,238),Color.FromArgb(116,134,148),90f))g.FillRectangle(b,r);Bevel(g,r,true);Line(g,Color.FromArgb(60,72,82),x+6,y+2,x+6,y+7);Line(g,Color.FromArgb(244,249,249),x+8,y+2,x+8,y+7);}
    static void Slider(Graphics g,float x,float y,float width,double? value){Inset(g,x,y,width,7,false);if(value.HasValue){for(int i=0;i<(width-2)*value.Value/100;i++)Fill(g,UsageColor(100.0*i/(width-2)),x+1+i,y+2,1,3);Cap(g,x+(float)((width-15)*value.Value/100),y-2,false);}}
    public static int Height(ClockSettings s){return 16+(s.ShowClock?70:0)+91+(s.Metrics.Count>0?121:0)+(s.Expanded?91:0)+22;}
    static void ClockDigits(Graphics g,string time,int x,int y,Color color){
        string[] numbers={"01110/10001/10001/10001/10001/10001/10001/10001/01110","00010/00110/00010/00010/00010/00010/00010/00010/00010","01110/10001/00001/00001/01110/10000/10000/10000/11111","11110/00001/00001/00001/01110/00001/00001/00001/11110","10001/10001/10001/10001/01111/00001/00001/00001/00001","11111/10000/10000/10000/11110/00001/00001/00001/11110","01110/10000/10000/10000/11110/10001/10001/10001/01110","11111/00001/00001/00010/00100/01000/01000/01000/01000","01110/10001/10001/10001/01110/10001/10001/10001/01110","01110/10001/10001/10001/01111/00001/00001/00001/01110"};
        foreach(char c in time){if(c==':'){Fill(g,color,x+2,y+9,3,3);Fill(g,color,x+2,y+25,3,3);x+=12;continue;}var rows=numbers[c-'0'].Split('/');for(int row=0;row<9;row++)for(int col=0;col<5;col++){bool on=rows[row][col]=='1';Fill(g,on?color:Color.FromArgb(15,35,17),x+col*4,y+row*4,3,3);}x+=28;}
    }
    public static Bitmap ButtonImage(string kind){var b=new Bitmap(10,10);using(var g=Graphics.FromImage(b)){MetalCell(g,0,0,9,9);using(var p=new Pen(Color.FromArgb(28,36,49))){if(kind=="close"){g.DrawLine(p,2,2,7,7);g.DrawLine(p,7,2,2,7);}else if(kind=="minimize")g.DrawLine(p,2,7,7,7);else if(kind=="expand")g.DrawRectangle(p,2,2,5,5);else{g.DrawLine(p,2,3,7,3);g.DrawLine(p,2,6,7,6);g.DrawLine(p,4,2,4,4);g.DrawLine(p,6,5,6,7);}}}return b;}
    public static Bitmap MusicButton(string action,bool playing){int width=action=="shuffle"?47:action.StartsWith("repeat")?29:27;var b=new Bitmap(width,22);using(var g=Graphics.FromImage(b)){MetalCell(g,0,0,width-1,21);Color ink=Color.FromArgb(35,48,63);using(var p=new Pen(ink,1.6f))using(var brush=new SolidBrush(ink)){
 if(action=="shuffle"){g.DrawLine(p,7,7,32,15);g.DrawLine(p,7,15,32,7);g.DrawLine(p,28,5,33,7);g.DrawLine(p,33,7,31,11);}
 else if(action.StartsWith("repeat")){g.DrawArc(p,7,5,14,11,35,295);g.DrawLine(p,21,8,21,4);g.DrawLine(p,21,8,17,8);if(action=="repeat-one")PixelText(g,"1",11,8,ink);}
 else if(action=="volume-down"||action=="volume-up"){g.DrawLine(p,7,11,20,11);if(action=="volume-up")g.DrawLine(p,14,5,14,17);}
 else if(action=="mute"){g.FillPolygon(brush,new[]{new Point(6,9),new Point(11,9),new Point(16,5),new Point(16,17),new Point(11,13),new Point(6,13)});if(playing){g.DrawLine(p,19,8,23,14);g.DrawLine(p,23,8,19,14);}else g.DrawArc(p,15,5,9,12,-70,140);}
 else if(action=="toggle"&&playing){g.FillRectangle(brush,9,6,3,10);g.FillRectangle(brush,16,6,3,10);}
 else{bool previous=action=="previous";g.FillPolygon(brush,previous?new[]{new Point(17,5),new Point(8,11),new Point(17,17)}:new[]{new Point(9,5),new Point(19,11),new Point(9,17)});if(action=="previous"||action=="next")g.DrawLine(p,previous?6:21,5,previous?6:21,17);}
 }if(playing&&(action=="shuffle"||action.StartsWith("repeat")))Fill(g,Green,3,18,width-6,2);}return b;}
    public static void DrawVolumeTrack(Graphics g,int? volume){var state=g.Save();g.SetClip(new Rectangle(0,0,158,22));Metal(g,0,22);Slider(g,1,8,155,volume.HasValue?(double?)SpotifyVolume.Clamp(volume.Value):null);g.Restore(state);}
    public static void DrawMusic(Graphics g,MusicState m,int y){Metal(g,y,126);Header(g,"WINAMP",y,true);Inset(g,10,y+21,280,17,false);var state=g.Save();g.SetClip(new Rectangle(13,y+24,274,11));PixelText(g,m.Available?m.Description:"SPOTIFY - ESCOLHA UMA MUSICA",13,y+26,Green);g.Restore(state);Inset(g,10,y+42,280,15,true);state=g.Save();g.SetClip(new Rectangle(13,y+45,274,9));PixelText(g,m.Error!=""?m.Error:m.Playlist!=""?m.Playlist:m.Available?(m.Playing?"SPOTIFY - REPRODUZINDO":"SPOTIFY - PAUSADO"):"SEM SESSAO DO SPOTIFY",13,y+47,Pale);g.Restore(state);PixelText(g,m.Available?TimeSpan.FromSeconds(m.PositionSeconds).ToString(@"mm\:ss"):"--:--",199,y+72,Green);if(m.DurationSeconds>0){Inset(g,240,y+70,49,7,false);Fill(g,Green,241,y+71,(float)(47*Math.Min(1,m.PositionSeconds/m.DurationSeconds)),5);}PixelText(g,m.Volume.Available?m.Volume.Percent.ToString():"--",273,y+102,Pale);Bevel(g,new RectangleF(0,y,299,125),true);}
    static void Bolt(Graphics g,int x,int y){PublicArt.Wave(g,new RectangleF(x,y,15,17),Gold);}
    public static void Draw(Graphics g,ClockSettings s,Theme t,UsageResult usage,HardwareSample hardware,Image atlas,bool hasPhoto,bool reading,string error){
        var saved=g.Save();g.SmoothingMode=SmoothingMode.None;g.PixelOffsetMode=PixelOffsetMode.None;int h=Height(s);if(!hasPhoto)Metal(g,0,h);Header(g,"WINAMP",0,true);int y=20;
        Color green=s.Accent!=""?t.Accent:Green;
        if(s.ShowClock){Inset(g,9,y,171,55,false);if(hasPhoto)Fill(g,Color.FromArgb(95,0,0,0),9,y,171,55);
            PaintTools.Digits(g,DateTime.Now.ToString("HH:mm"),new RectangleF(32,y+8,142,39),green,s.ClockStyle,atlas,s.TintDigits,"IBM Plex Mono");
            for(int row=y+1;row<y+55;row+=2)for(int col=10;col<180;col+=2)Fill(g,Color.FromArgb(65,0,0,0),col,row,1,1);
            using(var b=new SolidBrush(green))g.FillPolygon(b,new[]{new Point(16,y+13),new Point(16,y+23),new Point(22,y+18)});
            Inset(g,187,y,103,16,false);Center(g,DateTime.Now.ToString("dd/MM/yyyy"),187,y+5,103,green);Center(g,DateTime.Now.ToString("ddd").ToUpperInvariant(),187,y+23,103,Pale);
            Inset(g,187,y+36,103,18,false);Center(g,usage.Provider.ToUpperInvariant(),187,y+42,103,green);y+=66;
        }else y=20;
        Inset(g,10,y,280,16,false);Center(g,error!=""?(usage.At.HasValue?usage.Provider.ToUpperInvariant()+" - LEITURA ANTERIOR":usage.Provider.ToUpperInvariant()+" - SEM LEITURA"):"CONSUMO DO "+usage.Provider.ToUpperInvariant(),10,y+5,280,green);y+=23;
        for(int i=0;i<2;i++){var q=usage.Windows.Count>i?usage.Windows[i]:null;PixelText(g,q==null?(i==0?"5 HORAS":"SEMANAL"):q.Label,12,y,Pale);Inset(g,239,y-3,50,13,false);Center(g,q==null?"--%":q.Used.ToString("0")+"%",239,y,50,q==null?Pale:UsageColor(q.Used));Slider(g,83,y+1,144,q==null?(double?)null:q.Used);y+=27;}
        y+=8;
        if(s.Metrics.Count>0){Header(g,"HARDWARE",y,false);y+=20;Metal(g,y,88);for(int tick=0;tick<3;tick++)PixelText(g,(100-tick*50)+"",275,y+21+tick*21,Pale);Bevel(g,new RectangleF(9,y-1,282,90),false);int cell=276/s.Metrics.Count;
            for(int i=0;i<s.Metrics.Count;i++){string key=s.Metrics[i];double? value;hardware.Values.TryGetValue(key,out value);int x=12+i*cell;MetalCell(g,x,y+3,cell-4,13);Center(g,key=="disk"?"DISCO":key.ToUpperInvariant(),x,y+6,cell-4,Color.FromArgb(47,59,69));
                int cx=x+(cell-4)/2;Fill(g,Color.FromArgb(3,5,6),cx-3,y+22,7,42);Line(g,Color.FromArgb(110,118,123),cx+4,y+22,cx+4,y+64);
                if(value.HasValue){float top=y+22+(float)(42*(1-value.Value/100));for(int row=(int)top;row<y+64;row++)Fill(g,UsageColor(100.0*(y+64-row)/42),cx-1,row,3,1);Cap(g,cx-5,y+21+(float)(35*(1-value.Value/100)),true);}
                Center(g,value.HasValue?value.Value.ToString("0")+"%":"--%",x,y+73,cell-4,value.HasValue?UsageColor(value.Value):Pale);
            }y+=101;
        }
        if(s.Expanded){Header(g,"CONSUMO / DETALHES",y,false);y+=19;Inset(g,10,y,280,63,false);
            for(int i=0;i<2;i++){var q=usage.Windows.Count>i?usage.Windows[i]:null;if(q!=null){PixelText(g,(i+1)+". "+q.Label.ToUpperInvariant()+" - "+q.Used.ToString("0.#")+"% USADO",15,y+6+i*26,green);PixelText(g,q.ResetText(DateTime.UtcNow),15,y+17+i*26,Color.FromArgb(151,172,151));}}
            Inset(g,280,y+2,7,58,false);Cap(g,277,y+8,true);y+=72;
        }
        int footer=h-20;Metal(g,footer,20);Inset(g,10,footer+4,254,11,false);PixelText(g,reading?"CONSULTANDO "+usage.Provider.ToUpperInvariant()+"...":usage.At.HasValue?"LIDO "+usage.At.Value.ToLocalTime().ToString("HH:mm:ss")+" / "+s.RefreshSeconds+"S":"F5 - ATUALIZAR CONSUMO",14,footer+6,green);Bolt(g,274,footer+1);
        for(int grip=0;grip<3;grip++)Line(g,Pale,287+grip*3,h-4,296,h-13+grip*3);Bevel(g,new RectangleF(0,0,299,h-1),true);g.Restore(saved);
    }
    static void MetalCell(Graphics g,int x,int y,int w,int h){using(var b=new LinearGradientBrush(new Rectangle(x,y,w,h),Color.FromArgb(213,222,224),Color.FromArgb(114,126,139),90))g.FillRectangle(b,x,y,w,h);Bevel(g,new RectangleF(x,y,w,h),true);}
}
