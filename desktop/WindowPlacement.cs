using System;
using System.Drawing;

public static class WindowPlacement {
    public static int ClockHeight(ClockSettings s){return s.Theme=="xp"?XpSkin.Height(s):s.Theme=="winamp"?WinampSkin.Height(s):s.Theme=="aero"?AeroSkin.Height(s):RetroSkins.Handles(s.Theme)?RetroSkins.Height(s):DesktopSkins.Handles(s.Theme)?DesktopSkins.Height(s):26+(s.ShowClock?66:0)+20+2*(s.Expanded?46:33)+(s.Metrics.Count>0?(s.Expanded||s.Theme=="icq"?s.Metrics.Count*25+24:49):0)+35;}
    public static int WidthToFit(ClockSettings settings,int usableHeight){var s=new ClockSettings {Theme=settings.Theme,Width=settings.Width,Metrics=settings.Metrics,Expanded=settings.Expanded,ShowClock=settings.ShowClock,ShowMusic=settings.ShowMusic};for(int width=s.Width;width>=240;width--){s.Width=width;if(Math.Ceiling((ClockHeight(s)+MusicPanel.Height(s))*width/300f)<=usableHeight)return width;}return 240;}
    public static Point Fit(Rectangle window,Rectangle[] screens){
        if(screens==null||screens.Length==0)return window.Location;
        Rectangle target=screens[0];long maximum=0;
        foreach(var area in screens){var intersection=Rectangle.Intersect(window,area);long size=(long)Math.Max(0,intersection.Width)*Math.Max(0,intersection.Height);if(size>maximum){maximum=size;target=area;}}
        int x=Math.Max(target.Left,Math.Min(window.X,target.Right-window.Width)),y=Math.Max(target.Top,Math.Min(window.Y,target.Bottom-window.Height));return new Point(x,y);
    }
}
