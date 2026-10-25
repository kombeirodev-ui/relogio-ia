using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class RelogioIA : Form {
    public ClockSettings Settings;
    public static readonly string Folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"RelogioIA-Publicacao");
    public static string SettingsPath {get{return Path.Combine(Folder,"settings.json");}}
    readonly System.Windows.Forms.Timer timer=new System.Windows.Forms.Timer {Interval=1000};
    readonly Sensors sensors=new Sensors();readonly ToolTip tips=new ToolTip();
    readonly SpotifyVolume spotifyVolume=new SpotifyVolume();readonly VolumeSlider volumeSlider=new VolumeSlider();readonly System.Windows.Forms.Timer volumeTimer=new System.Windows.Forms.Timer {Interval=120};int? pendingVolume;bool writingVolume;
    readonly SoundAlerts sounds=new SoundAlerts();
    readonly SpotifyMedia spotify=new SpotifyMedia();MusicState music=new MusicState();bool readingMusic,commandingMusic;string musicButtonStyle="",musicActionError="";DateTime musicActionErrorUntil=DateTime.MinValue;DateTime lastMusic=DateTime.MinValue;
    readonly Button musicPrevious=new Button(),musicToggle=new Button(),musicNext=new Button(),musicShuffle=new Button(),musicRepeat=new Button(),volumeDown=new Button(),volumeUp=new Button(),volumeMute=new Button();
    readonly Button gear=new Button(),close=new Button(),expand=new Button(),minimize=new Button();
    UsageResult usage=new UsageResult();HardwareSample hardware=new HardwareSample();
    string readError="",loadedPhoto="",loadedAtlas="";bool reading,measuring,closing,adjusting;
    DateTime attempted=DateTime.MinValue,lastHardware=DateTime.MinValue;int minute=-1;
    Image photo,atlas;SettingsForm editor;Theme theme;string activeProvider="";
    [DllImport("user32.dll")]static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")]static extern bool ReleaseCapture();
    [DllImport("user32.dll")]static extern IntPtr SendMessage(IntPtr window,int message,IntPtr w,IntPtr l);
    [DllImport("user32.dll")]static extern IntPtr FindWindow(string kind,string title);
    [DllImport("user32.dll")]static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")]static extern bool ShowWindow(IntPtr window,int command);
    [STAThread]public static void Main(string[] args) {
        if(args.Length>1&&args[0]=="--restart-wait"){int parent;if(Int32.TryParse(args[1],out parent))try{using(var p=System.Diagnostics.Process.GetProcessById(parent))if(!p.WaitForExit(10000))return;}catch(ArgumentException){} }
        if(args.Length>1&&args[0]=="--music-diagnose"){using(var media=new SpotifyMedia()){var value=media.ReadAsync().GetAwaiter().GetResult();using(var volume=new SpotifyVolume())value.Volume=volume.Read();bool? paused=null;if(args.Contains("--pause")&&value.Available&&!value.Playing)paused=media.CommandAsync("pause").GetAwaiter().GetResult();File.WriteAllText(args[1],Data.Json.Serialize(new {media=value,pauseAccepted=paused}));}return;}
        SetProcessDPIAware();Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
        if(args.Length>1&&args[0]=="--diagnose") {using(var sensor=new Sensors()){sensor.Sample(new[]{"cpu","gpu","ram","disk"});Thread.Sleep(1400);File.WriteAllText(args[1],Data.Json.Serialize(new {hardware=sensor.Sample(new[]{"cpu","gpu","ram","disk"}),quota=CodexClient.Read()}));}return;}
        bool created;using(var mutex=new Mutex(true,"Local\\RelogioIA.Publicacao",out created)) {
            if(!created){var window=FindWindow(null,"Relógio IA · Publicação");if(window!=IntPtr.Zero){ShowWindow(window,9);SetForegroundWindow(window);if(args.Contains("--settings"))SendMessage(window,0x8005,IntPtr.Zero,IntPtr.Zero);}return;}
            var app=new RelogioIA();if(args.Contains("--settings"))app.Shown+=(s,e)=>app.OpenSettings();Application.Run(app);
        }
    }
    public RelogioIA() {
        Text="Relógio IA · Publicação";AccessibleName="Relógio IA";FormBorderStyle=FormBorderStyle.None;AutoScaleMode=AutoScaleMode.None;DoubleBuffered=true;StartPosition=FormStartPosition.Manual;MinimumSize=new Size(240,100);MaximumSize=new Size(500,1400);
        string raw="";try{raw=File.ReadAllText(File.Exists(SettingsPath)?SettingsPath:Path.Combine(Folder,"skin.json"));}catch{}
        Settings=ClockSettings.Parse(raw);SettingsForm.RepairStartupPath();
        ButtonSetup(close,"close","Fechar relógio",()=>Close());ButtonSetup(gear,"gear","Abrir configurações",()=>OpenSettings());ButtonSetup(expand,"expand","Expandir ou recolher",()=>{Settings.Expanded=!Settings.Expanded;ApplySettings();});
        ButtonSetup(minimize,"minimize","Minimizar relógio",()=>WindowState=FormWindowState.Minimized);
        MusicButtonSetup(musicPrevious,"previous","Faixa anterior no Spotify");MusicButtonSetup(musicToggle,"toggle","Reproduzir ou pausar Spotify");MusicButtonSetup(musicNext,"next","Próxima faixa no Spotify");MusicButtonSetup(musicShuffle,"shuffle","Aleatório (shuffle)");MusicButtonSetup(musicRepeat,"repeat","Repetição");MusicButtonSetup(volumeDown,"volume-down","Diminuir volume do Spotify");MusicButtonSetup(volumeUp,"volume-up","Aumentar volume do Spotify");MusicButtonSetup(volumeMute,"mute","Silenciar Spotify");volumeSlider.Requested=QueueVolume;Controls.Add(volumeSlider);volumeTimer.Tick+=(s,e)=>WriteVolume();
        var menu=new ContextMenuStrip();menu.Items.Add("Configurações…",null,(s,e)=>OpenSettings());menu.Items.Add("Atualizar consumo agora",null,(s,e)=>ReadUsage());
        var themes=new ToolStripMenuItem("Tema");foreach(string id in Theme.Ids){string value=id;themes.DropDownItems.Add(Theme.Get(id).Name.ToUpperInvariant(),null,(s,e)=>{Settings.Preset(value);ApplySettings();if(editor!=null&&!editor.IsDisposed)editor.Reload();});}menu.Items.Add(themes);
        var displays=new ToolStripMenuItem("Mover para tela");foreach(var screen in Screen.AllScreens){var display=screen;displays.DropDownItems.Add("Tela "+(Array.IndexOf(Screen.AllScreens,screen)+1)+(screen.Primary?" (principal)":""),null,(s,e)=>{Settings.Width=WindowPlacement.WidthToFit(Settings,display.WorkingArea.Height);Location=new Point(display.WorkingArea.Left+10,display.WorkingArea.Top+10);ApplySettings(false);Location=WindowPlacement.Fit(new Rectangle(display.WorkingArea.Right-Width-20,display.WorkingArea.Bottom-Height-20,Width,Height),new[]{display.WorkingArea});Save();});}menu.Items.Add(displays);
        menu.Items.Add("Abrir Spotify",null,(s,e)=>OpenSpotify());menu.Items.Add("Reiniciar relógio",null,(s,e)=>RestartClock());
        menu.Items.Add("Compacto / expandido",null,(s,e)=>{Settings.Expanded=!Settings.Expanded;ApplySettings();});menu.Items.Add("Fechar relógio",null,(s,e)=>Close());ContextMenuStrip=menu;
        ApplySettings(false);var work=Screen.PrimaryScreen.WorkingArea;Location=new Point(work.Right-Width-30,work.Bottom-Height-30);
        if(Settings.X.HasValue&&Settings.Y.HasValue)Location=WindowPlacement.Fit(new Rectangle(Settings.X.Value,Settings.Y.Value,Width,Height),Screen.AllScreens.Select(screen=>screen.WorkingArea).ToArray());
        MouseDown+=(s,e)=>{if(e.Button==MouseButtons.Left&&e.Y<=28*UiScale){ReleaseCapture();SendMessage(Handle,0xA1,new IntPtr(2),IntPtr.Zero);Save();}};
        timer.Tick+=(s,e)=>Tick();Shown+=(s,e)=>{ApplySettings(false);timer.Start();Tick();};
        FormClosed+=(s,e)=>{closing=true;Save();timer.Stop();timer.Dispose();CodexClient.Cancel();spotify.Dispose();volumeTimer.Stop();volumeTimer.Dispose();spotifyVolume.Dispose();sounds.Dispose();sensors.Dispose();tips.Dispose();if(photo!=null)photo.Dispose();if(atlas!=null)atlas.Dispose();};
        KeyPreview=true;KeyDown+=(s,e)=>{if(e.Control&&e.KeyCode==Keys.O)OpenSettings();if(e.KeyCode==Keys.F5)ReadUsage();};
    }
    float UiScale {get{return Settings.Width/300f;}}
    string FontName {get{return Settings.Font=="mono"?(Settings.Theme=="otimizepc"?"IBM Plex Mono":"Consolas"):Settings.Font=="serif"?"Georgia":Settings.Font=="condensed"?"Impact":Settings.Theme=="icq"?"Tahoma":Settings.Theme=="otimizepc"?"IBM Plex Sans":"Segoe UI";}}
    bool VerticalHardware {get{return Settings.Expanded||Settings.Theme=="icq";}}
    int ClockHeight {get{return WindowPlacement.ClockHeight(Settings);}}
    void ButtonSetup(Button b,string kind,string name,Action action){b.Tag=kind;b.AccessibleName=name;b.FlatStyle=FlatStyle.Flat;b.FlatAppearance.BorderSize=0;b.Click+=(s,e)=>action();Controls.Add(b);tips.SetToolTip(b,name);}
    protected override void WndProc(ref Message m){if(m.Msg==0x8005){OpenSettings();return;}base.WndProc(ref m);if(m.Msg==0x84&&Settings!=null){int x=(short)((long)m.LParam&0xffff)-Left;int y=(short)(((long)m.LParam>>16)&0xffff)-Top;if(x>=Width-6&&y>28*UiScale)m.Result=new IntPtr(11);}if(m.Msg==0x232&&Settings!=null){Settings.Width=Math.Max(240,Math.Min(500,Width));ApplySettings();}}
    protected override void OnResize(EventArgs e){base.OnResize(e);if(Settings!=null&&!adjusting&&Width>=240&&Width<=500){Settings.Width=Width;ApplySettings(false);}}
    public void OpenSettings(){if(editor==null||editor.IsDisposed){editor=new SettingsForm(this);editor.Show(this);}else{editor.Reload();editor.Activate();}}
    int LogicalHeight {get{return ClockHeight+MusicPanel.Height(Settings);}}
    void MusicButtonSetup(Button button,string action,string label){button.Tag=action;button.FlatStyle=FlatStyle.Flat;button.FlatAppearance.BorderSize=0;button.AccessibleName=label;button.TabStop=true;tips.SetToolTip(button,label);button.Click+=(s,e)=>MusicCommand(action);Controls.Add(button);}
    void UpdateMusicButtons(){string style=theme.Id+theme.Text.ToArgb()+":"+theme.Panel.ToArgb()+":"+theme.Inset.ToArgb()+":"+music.Playing+":"+music.Shuffle+":"+music.Repeat+":"+music.Volume.Muted;bool redraw=style!=musicButtonStyle;musicButtonStyle=style;int i=0;foreach(var button in new[]{musicPrevious,musicToggle,musicNext,musicShuffle,musicRepeat,volumeDown,volumeUp,volumeMute}){RectangleF r=i<5?MusicPanel.ControlBounds(i,ClockHeight):new RectangleF(i==5?13:i==6?211:244,ClockHeight+94,27,22);button.SetBounds((int)(r.X*UiScale),(int)(r.Y*UiScale),Math.Max(20,(int)(r.Width*UiScale)),Math.Max(18,(int)(r.Height*UiScale)));button.Visible=Settings.ShowMusic;button.Enabled=!commandingMusic&&(i>=5?music.Volume.Available:music.Available&&(i==3?music.CanShuffle&&music.Shuffle.HasValue:i==4?music.CanRepeat&&music.Repeat.HasValue:true));button.BackColor=theme.Panel;if(redraw){var old=button.BackgroundImage;bool active=i==3?music.Shuffle==true:i==4?music.Repeat>0:i==7?music.Volume.Muted:music.Playing;string action=i==4&&music.Repeat==1?"repeat-one":(string)button.Tag;button.BackgroundImage=MusicPanel.ButtonImage(action,theme,active);button.BackgroundImageLayout=ImageLayout.Stretch;if(old!=null)old.Dispose();}i++;}volumeSlider.SetBounds((int)(46*UiScale),(int)((ClockHeight+94)*UiScale),(int)(158*UiScale),(int)(22*UiScale));volumeSlider.Visible=Settings.ShowMusic;volumeSlider.Enabled=music.Volume.Available;volumeSlider.Theme=theme;if(!volumeSlider.Dragging&&!writingVolume&&!pendingVolume.HasValue)volumeSlider.Percent=music.Volume.Percent;volumeSlider.AccessibleDescription=music.Volume.Available?music.Volume.Percent+"% — somente Spotify":"Volume indisponível; inicie a reprodução no Spotify";tips.SetToolTip(volumeSlider,volumeSlider.AccessibleDescription);tips.SetToolTip(musicShuffle,music.CanShuffle?"Aleatório: "+(music.Shuffle==true?"ligado":"desligado"):"Spotify não disponibilizou shuffle pela sessão local");tips.SetToolTip(musicRepeat,music.CanRepeat?"Repetir: "+(music.Repeat==1?"faixa":music.Repeat==2?"fila":"desligado"):"Spotify não disponibilizou repetição pela sessão local");volumeSlider.Invalidate();}
    void QueueVolume(int value){if(!music.Volume.Available||closing)return;pendingVolume=SpotifyVolume.Clamp(value);music.Volume.Percent=pendingVolume.Value;volumeSlider.Percent=pendingVolume.Value;volumeSlider.Invalidate();volumeTimer.Stop();volumeTimer.Start();Invalidate(new Rectangle(0,(int)(ClockHeight*UiScale),Width,Height-(int)(ClockHeight*UiScale)));}
    async void WriteVolume(){volumeTimer.Stop();if(closing||!pendingVolume.HasValue)return;if(writingVolume){volumeTimer.Start();return;}int value=pendingVolume.Value;pendingVolume=null;writingVolume=true;try{bool ok=await Task.Run(()=>spotifyVolume.SetPercent(value));if(!ok&&!closing){musicActionError="Volume indisponível";musicActionErrorUntil=DateTime.UtcNow.AddSeconds(5);}}finally{writingVolume=false;if(!closing){if(pendingVolume.HasValue)volumeTimer.Start();else RefreshMusic();}}}
    async void RefreshMusic(){if(readingMusic||closing||!Settings.ShowMusic)return;readingMusic=true;lastMusic=DateTime.UtcNow;try{var value=await spotify.ReadAsync();var level=await Task.Run(()=>spotifyVolume.Read());value.Volume=writingVolume||pendingVolume.HasValue?music.Volume:level;if(!closing){if(DateTime.UtcNow<musicActionErrorUntil)value.Error=musicActionError;music=value;UpdateMusicButtons();Invalidate(new Rectangle(0,(int)(ClockHeight*UiScale),Width,Height-(int)(ClockHeight*UiScale)));}}finally{readingMusic=false;}}
    async void MusicCommand(string action){if(action=="volume-down"||action=="volume-up"){QueueVolume(music.Volume.Percent+(action=="volume-down"?-5:5));return;}if(commandingMusic||closing)return;commandingMusic=true;UpdateMusicButtons();try{bool ok=action=="mute"?await Task.Run(()=>spotifyVolume.SetMuted(!music.Volume.Muted)):await spotify.CommandAsync(action);if(!closing){musicActionError=ok?"":"Comando indisponível";musicActionErrorUntil=ok?DateTime.MinValue:DateTime.UtcNow.AddSeconds(5);}}finally{commandingMusic=false;if(!closing){lastMusic=DateTime.MinValue;RefreshMusic();UpdateMusicButtons();}}}
    void DrawMusic(Graphics g){MusicPanel.Draw(g,Settings,theme,music,ClockHeight);}
    public string MusicDescription(){return music.Available?music.Description+"\r\n"+(music.Playing?"Reproduzindo":"Pausado")+" · controles locais do Windows\r\nPlaylist: não informada pela sessão local.":music.Error!=""?music.Error:"Abra o Spotify desktop e escolha uma faixa para ativar os controles.";}
    public void TestAlertSound(){sounds.Play(Settings.AlertSound);}
    public void OpenSpotify(){try{System.Diagnostics.Process.Start("spotify:");}catch{MessageBox.Show("Não foi possível abrir o Spotify. Abra o aplicativo instalado no Windows.",Text);}}
    public void RestartClock(){Save();try{System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath,"--restart-wait "+System.Diagnostics.Process.GetCurrentProcess().Id);Close();}catch{MessageBox.Show("Não foi possível reiniciar. Use o atalho da área de trabalho.",Text);}}
    public void ApplySettings(bool persist=true) {
        if(adjusting)return;adjusting=true;
        try{
            Settings=ClockSettings.Parse(Settings.Serialize());theme=Theme.Get(Settings.Theme);if(Settings.Accent!="")theme.Accent=Theme.C(Settings.Accent);if(Settings.TextColor!="")theme.Text=Theme.C(Settings.TextColor);
            if(activeProvider!=Settings.Provider){CodexClient.Cancel();activeProvider=Settings.Provider;usage=new UsageResult {Provider=Settings.Provider=="claude"?"Claude":"Codex"};readError="";attempted=DateTime.MinValue;}
            BackColor=theme.Background;TopMost=Settings.TopMost;
            if(!Settings.Metrics.Contains("gpu"))sensors.Dispose();
            if(loadedPhoto!=Settings.Background){ReplaceImage(ref photo,Settings.Background);loadedPhoto=Settings.Background;}
            if(loadedAtlas!=Settings.Atlas){ReplaceImage(ref atlas,Settings.Atlas);loadedAtlas=Settings.Atlas;}
            if(IsHandleCreated)Settings.Width=WindowPlacement.WidthToFit(Settings,Screen.FromControl(this).WorkingArea.Height);
            ClientSize=new Size(Settings.Width,(int)Math.Ceiling(LogicalHeight*UiScale));
            var oldRegion=Region;Region=theme.Id=="xp"?XpSkin.WindowRegion(Width,Height):theme.Id=="aero"?AeroSkin.WindowRegion(Width,Height):null;if(oldRegion!=null)oldRegion.Dispose();
            if(IsHandleCreated){var work=Screen.FromControl(this).WorkingArea;Location=new Point(Math.Max(work.Left,Math.Min(Left,work.Right-Width)),Math.Max(work.Top,Math.Min(Top,work.Bottom-Height)));}
            bool classic=theme.Id=="winamp",modern=theme.Id=="xp"||DesktopSkins.Handles(theme.Id)||RetroSkins.Handles(theme.Id)||theme.Id=="aero";minimize.Visible=classic||modern;
            var buttons=classic?new[]{close,expand,minimize,gear}:modern?new[]{close,expand,minimize,gear}:new[]{close,gear,expand};for(int i=0;i<buttons.Length;i++){var b=buttons[i];if(classic)b.SetBounds((int)((i==3?5:288-i*11)*UiScale),(int)(3*UiScale),(int)(10*UiScale),(int)(10*UiScale));else if(theme.Id=="xp")b.SetBounds(Width-(int)((24+i*22)*UiScale),(int)(4*UiScale),(int)(21*UiScale),(int)(21*UiScale));else b.SetBounds(Width-(int)(((modern?22:25)+i*(modern?21:25))*UiScale),(int)((modern?1:3)*UiScale),(int)((modern?20:22)*UiScale),(int)(20*UiScale));b.BackColor=theme.Header;b.FlatAppearance.MouseOverBackColor=theme.Panel;b.FlatAppearance.MouseDownBackColor=theme.Inset;var old=b.Image;b.Image=theme.Id=="xp"?XpSkin.ControlIcon((string)b.Tag):classic?WinampSkin.ButtonImage((string)b.Tag):RetroSkins.Handles(theme.Id)?RetroSkins.ControlIcon((string)b.Tag,theme):theme.Id=="aero"?AeroSkin.ControlIcon((string)b.Tag):modern?BrandArt.ControlIcon((string)b.Tag,theme):PaintTools.Icon((string)b.Tag,Settings.Theme=="icq"?Color.White:theme.Text);b.BackgroundImageLayout=ImageLayout.Stretch;if(old!=null)old.Dispose();}
            UpdateMusicButtons();AccessibleDescription="Tema "+theme.Name+"; "+(Settings.ShowClock?"relógio visível":"relógio oculto")+"; indicadores: "+String.Join(", ",Settings.Metrics);Invalidate();if(persist)Save();
        }finally{adjusting=false;}
    }
    static void ReplaceImage(ref Image target,string data){Image next=null;if(!String.IsNullOrEmpty(data))try{using(var stream=new MemoryStream(Convert.FromBase64String(data.Substring(data.IndexOf(',')+1))))using(var image=Image.FromStream(stream)){if(image.Width*1L*image.Height<=25000000)next=new Bitmap(image);}}catch{}if(target!=null)target.Dispose();target=next;}
    public void Save(){try{Directory.CreateDirectory(Folder);Settings.X=Left;Settings.Y=Top;string temp=SettingsPath+".tmp";File.WriteAllText(temp,Settings.Serialize());if(File.Exists(SettingsPath))File.Replace(temp,SettingsPath,SettingsPath+".bak");else File.Move(temp,SettingsPath);}catch{readError="Preferências aplicadas, mas não foi possível salvar.";}}
    async void Tick() {
        if(closing)return;if(Settings.ShowMusic&&(DateTime.UtcNow-lastMusic).TotalSeconds>=2)RefreshMusic();if(minute!=DateTime.Now.Minute){minute=DateTime.Now.Minute;Invalidate();}
        if(!reading&&(DateTime.UtcNow-attempted).TotalSeconds>=Settings.RefreshSeconds)ReadUsage();
        if(!measuring&&Settings.Metrics.Count>0&&(DateTime.UtcNow-lastHardware).TotalSeconds>=3){measuring=true;lastHardware=DateTime.UtcNow;var selected=Settings.Metrics.ToArray();var result=await Task.Run(()=>sensors.Sample(selected));if(!closing){hardware=result;measuring=false;Invalidate();}}
    }
    public async void ReadUsage(){if(reading||closing)return;reading=true;attempted=DateTime.UtcNow;Invalidate();string provider=Settings.Provider;var result=await Task.Run(()=>provider=="claude"?ClaudeClient.Read():CodexClient.Read());if(closing)return;if(provider!=Settings.Provider){reading=false;return;}reading=false;if(result.At.HasValue){usage=result;readError="";sounds.Observe(Settings,usage);}else readError=result.Error;Invalidate();if(editor!=null&&!editor.IsDisposed)editor.UpdateReading(ReadingDescription());}
    public string ReadingDescription(){return (usage.At.HasValue?"Última leitura: "+usage.At.Value.ToLocalTime().ToString("HH:mm:ss"):"Sem leitura confirmada")+"\r\nFonte: conta "+usage.Provider+" ("+(String.IsNullOrEmpty(usage.Account)?"não identificada":usage.Account)+"). Consulta a cada "+Settings.RefreshSeconds+" s.\r\n"+readError+"\r\n"+String.Join("\r\n",usage.Windows.Select(q=>q.Label+": "+q.Used.ToString("0.#")+"% usado · "+q.ResetText(DateTime.UtcNow)))+"\r\nGPU: "+hardware.GpuNote+". Disco: espaço ocupado na unidade do Windows.";}
    protected override void OnPaint(PaintEventArgs e) {
        base.OnPaint(e);if(theme==null)return;var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.ScaleTransform(UiScale,UiScale);float height=ClockHeight;if(Settings.ShowMusic&&e.ClipRectangle.Top>=Math.Ceiling(ClockHeight*UiScale)){DrawMusic(g);return;}
        PaintTools.Fill(g,theme.Background,new RectangleF(0,0,300,height));
        if(photo!=null){float f=Math.Max(300f/photo.Width,height/photo.Height);g.DrawImage(photo,new RectangleF((300-photo.Width*f)/2,(height-photo.Height*f)/2,photo.Width*f,photo.Height*f));PaintTools.Fill(g,Color.FromArgb((int)(Settings.Fade*255),Color.Black),new RectangleF(0,0,300,height));}
        if(theme.Id=="xp"){XpSkin.Draw(g,Settings,theme,usage,hardware,atlas,photo!=null,reading,readError);DrawMusic(g);return;}
        if(theme.Id=="winamp"){WinampSkin.Draw(g,Settings,theme,usage,hardware,atlas,photo!=null,reading,readError);DrawMusic(g);return;}
        if(DesktopSkins.Handles(theme.Id)){DesktopSkins.Draw(g,Settings,theme,usage,hardware,atlas,photo!=null,reading,readError);DrawMusic(g);return;}
        if(RetroSkins.Handles(theme.Id)){RetroSkins.Draw(g,Settings,theme,usage,hardware,atlas,photo!=null,reading,readError);DrawMusic(g);return;}
        if(theme.Id=="aero"){AeroSkin.Draw(g,Settings,theme,usage,hardware,atlas,photo!=null,reading,readError);DrawMusic(g);return;}
        BasicSkin.Draw(g,Settings,theme,usage,hardware,atlas,photo!=null,reading,readError);
        DrawMusic(g);
    }
    string MetricName(string key){return key=="disk"?hardware.DiskName:key.ToUpperInvariant();}
}
