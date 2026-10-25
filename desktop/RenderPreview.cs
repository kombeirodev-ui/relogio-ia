using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

// File-only preview utility: never opens a window or reads/writes preferences.
// All quota and hardware values are deliberately illustrative fixtures.
class RenderPreview {
    static int Main(string[] args){
        string theme=args[0];int width=Int32.Parse(args[1]);string output=Path.GetFullPath(args[2]);
        var s=new ClockSettings {Theme=theme,Width=width,Font="sans",Expanded=true,ShowClock=true};s.Metrics.Add("disk");if(theme=="xp")s.ClockStyle="text";
        var t=Theme.Get(theme);int clockHeight=WindowPlacement.ClockHeight(s);int height=clockHeight+MusicPanel.Height(s);int physical=(int)Math.Ceiling(height*width/300f);
        var u=new UsageResult {At=DateTime.UtcNow};long now=(long)(DateTime.UtcNow-new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)).TotalSeconds;
        u.Windows.Add(new Quota {Used=47,Minutes=300,ResetsAt=now+5500});u.Windows.Add(new Quota {Used=73,Minutes=10080,ResetsAt=now+450000});
        var sample=new HardwareSample {DiskName="Disco C:"};sample.Values["cpu"]=23.4;sample.Values["gpu"]=16.8;sample.Values["ram"]=47.2;sample.Values["disk"]=58.1;
        using(var bitmap=new Bitmap(width,physical+30))using(var g=Graphics.FromImage(bitmap)){
            g.Clear(Color.White);g.SmoothingMode=SmoothingMode.AntiAlias;var state=g.Save();g.ScaleTransform(width/300f,width/300f);PaintTools.Fill(g,t.Background,new RectangleF(0,0,300,height));
            if(theme=="xp")XpSkin.Draw(g,s,t,u,sample,null,false,false,"");else if(theme=="aero")AeroSkin.Draw(g,s,t,u,sample,null,false,false,"");else if(theme=="winamp")WinampSkin.Draw(g,s,t,u,sample,null,false,false,"");else if(DesktopSkins.Handles(theme))DesktopSkins.Draw(g,s,t,u,sample,null,false,false,"");else if(RetroSkins.Handles(theme))RetroSkins.Draw(g,s,t,u,sample,null,false,false,"");else BasicSkin.Draw(g,s,t,u,sample,null,false,false,"");
            var music=new MusicState {Available=true,Title="Uma faixa de exemplo",Artist="Artista de exemplo",Playing=true,CanNext=true,CanToggle=true,CanPrevious=true,CanShuffle=true,CanRepeat=true,Shuffle=true,Repeat=2,Volume=new VolumeState {Available=true,Percent=72},DurationSeconds=240,PositionSeconds=83};MusicPanel.Draw(g,s,t,music,clockHeight);for(int i=0;i<5;i++)using(var button=MusicPanel.ButtonImage(i==0?"previous":i==1?"toggle":i==2?"next":i==3?"shuffle":"repeat",t,true))g.DrawImage(button,MusicPanel.ControlBounds(i,clockHeight));for(int i=0;i<3;i++)using(var button=MusicPanel.ButtonImage(i==0?"volume-down":i==1?"volume-up":"mute",t,false))g.DrawImage(button,new RectangleF(i==0?13:i==1?211:244,clockHeight+94,27,22));var volumeState=g.Save();g.TranslateTransform(46,clockHeight+94);MusicPanel.VolumeTrack(g,t,72);g.Restore(volumeState);
            string[] icons={"close","expand","minimize","gear"};for(int i=0;i<icons.Length;i++)using(var icon=theme=="winamp"?WinampSkin.ButtonImage(icons[i]):theme=="xp"?XpSkin.ControlIcon(icons[i]):theme=="aero"?AeroSkin.ControlIcon(icons[i]):DesktopSkins.Handles(theme)?BrandArt.ControlIcon(icons[i],t):RetroSkins.ControlIcon(icons[i],t))g.DrawImage(icon,theme=="winamp"?new Rectangle(i==3?5:288-i*11,3,10,10):theme=="xp"?new Rectangle(276-i*22,4,21,21):new Rectangle(278-i*21,1,20,20));
            g.Restore(state);using(var f=new Font("Segoe UI",11,FontStyle.Regular,GraphicsUnit.Pixel))using(var b=new SolidBrush(Color.FromArgb(55,65,75)))g.DrawString("Prévia · valores ilustrativos",f,b,5,physical+7);
            Directory.CreateDirectory(Path.GetDirectoryName(output));bitmap.Save(output,ImageFormat.Png);
        }return 0;
    }
}
