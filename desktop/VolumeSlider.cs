using System;
using System.Drawing;
using System.Windows.Forms;

public sealed class VolumeSlider : Control {
    public Theme Theme;public int Percent;public bool Dragging;public Action<int> Requested;
    public VolumeSlider(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.Selectable,true);TabStop=true;AccessibleName="Volume do Spotify";AccessibleRole=AccessibleRole.Slider;Cursor=Cursors.Hand;}
    protected override void OnPaint(PaintEventArgs e){if(Theme!=null){e.Graphics.ScaleTransform(Width/158f,Height/22f);MusicPanel.VolumeTrack(e.Graphics,Theme,Enabled?(int?)Percent:null);}}
    void SetFromMouse(int x){int value=SpotifyVolume.Clamp((int)Math.Round(100d*(x-5)/Math.Max(1,Width-10)));Percent=value;Invalidate();if(Requested!=null)Requested(value);}
    protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);if(e.Button==MouseButtons.Left&&Enabled){Focus();Capture=true;Dragging=true;SetFromMouse(e.X);}}
    protected override void OnMouseMove(MouseEventArgs e){base.OnMouseMove(e);if(Dragging&&Capture)SetFromMouse(e.X);}
    protected override void OnMouseUp(MouseEventArgs e){base.OnMouseUp(e);Dragging=false;Capture=false;}
    protected override void OnMouseCaptureChanged(EventArgs e){base.OnMouseCaptureChanged(e);if(!Capture)Dragging=false;}
    protected override bool IsInputKey(Keys key){return key==Keys.Left||key==Keys.Right||key==Keys.Home||key==Keys.End||base.IsInputKey(key);}
    protected override void OnKeyDown(KeyEventArgs e){base.OnKeyDown(e);int value=Percent;if(e.KeyCode==Keys.Left)value-=5;else if(e.KeyCode==Keys.Right)value+=5;else if(e.KeyCode==Keys.Home)value=0;else if(e.KeyCode==Keys.End)value=100;else return;e.Handled=true;Percent=SpotifyVolume.Clamp(value);Invalidate();if(Requested!=null)Requested(Percent);}
}
