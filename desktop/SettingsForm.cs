using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

public class SettingsForm : Form {
    readonly RelogioIA clock;
    readonly FlowLayoutPanel layout=new FlowLayoutPanel {Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(16)};
    readonly System.Windows.Forms.Timer saveTimer=new System.Windows.Forms.Timer {Interval=450};
    Label reading;bool loading;
    public SettingsForm(RelogioIA owner) {
        clock=owner;Text="Configurações — Relógio IA";AccessibleName=Text;ClientSize=new Size(480,640);MinimumSize=new Size(470,460);Font=new Font("Segoe UI",9);BackColor=Color.FromArgb(244,242,238);AutoScaleMode=AutoScaleMode.None;
        Controls.Add(layout);StartPosition=FormStartPosition.Manual;var work=Screen.FromControl(owner).WorkingArea;Location=new Point(Math.Max(work.Left,Math.Min(work.Right-Width,owner.Left-Width-16)),Math.Max(work.Top,Math.Min(work.Bottom-Height,owner.Top)));
        saveTimer.Tick+=(s,e)=>{saveTimer.Stop();clock.Save();};FormClosed+=(s,e)=>{saveTimer.Stop();saveTimer.Dispose();clock.Save();};Reload();
    }
    void Changed(){if(loading)return;clock.ApplySettings(false);saveTimer.Stop();saveTimer.Start();}
    void Heading(string text){layout.Controls.Add(new Label {Text=text,AutoSize=false,Width=425,Height=26,Margin=new Padding(0,12,0,3),Font=new Font(Font,FontStyle.Bold)});}
    void Note(string text,int height=38){layout.Controls.Add(new Label {Text=text,Width=420,Height=height,ForeColor=Color.FromArgb(85,91,88),Margin=new Padding(0,0,0,5)});}
    Button Action(string text,Action callback){var b=new Button {Text=text,Width=420,Height=31,Margin=new Padding(0,3,0,4),AccessibleName=text};b.Click+=(s,e)=>callback();layout.Controls.Add(b);return b;}
    void Check(string text,bool value,Action<bool> callback){var c=new CheckBox {Text=text,Checked=value,Width=420,Height=25,AccessibleName=text,Margin=new Padding(0,0,0,2)};c.CheckedChanged+=(s,e)=>{if(!loading){callback(c.Checked);Changed();}};layout.Controls.Add(c);}
    void Choice(string text,string[] labels,int index,Action<int> callback){Note(text,20);var c=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList,Width=420,AccessibleName=text,Margin=new Padding(0,0,0,8)};c.Items.AddRange(labels);c.SelectedIndex=Math.Max(0,index);c.SelectedIndexChanged+=(s,e)=>{if(!loading){callback(c.SelectedIndex);Changed();}};layout.Controls.Add(c);}
    public void Reload() {
        loading=true;layout.SuspendLayout();foreach(Control c in layout.Controls.Cast<Control>().ToArray()){layout.Controls.Remove(c);c.Dispose();}
        Heading("Relógio real · mudanças aparecem na hora");Note("Estas configurações controlam a janela de desktop. Fechar esta janela mantém o relógio aberto.");
        string[] themes=Theme.Ids;Choice("Tema completo",themes.Select(id=>Theme.Get(id).Name.ToUpperInvariant()).ToArray(),Array.IndexOf(themes,clock.Settings.Theme),i=>{clock.Settings.Preset(themes[i]);clock.ApplySettings();BeginInvoke(new Action(Reload));});
        Heading("Módulos visíveis");Check("Mostrar relógio",clock.Settings.ShowClock,v=>clock.Settings.ShowClock=v);
        foreach(var item in new[]{new[]{"cpu","Mostrar CPU"},new[]{"gpu","Mostrar GPU"},new[]{"ram","Mostrar memória RAM"},new[]{"disk","Mostrar espaço usado no disco"}}){string key=item[0];Check(item[1],clock.Settings.Metrics.Contains(key),v=>{if(v&&!clock.Settings.Metrics.Contains(key))clock.Settings.Metrics.Add(key);if(!v)clock.Settings.Metrics.Remove(key);});}
        Check("Modo expandido (renovações e detalhes)",clock.Settings.Expanded,v=>clock.Settings.Expanded=v);
        Check("Sempre acima das outras janelas",clock.Settings.TopMost,v=>clock.Settings.TopMost=v);
        Note("GPU: motor mais ocupado informado pelo Windows. Disco: espaço ocupado, não velocidade de leitura/gravação.",37);
        Heading("Spotify");Check("Mostrar player de música",clock.Settings.ShowMusic,v=>clock.Settings.ShowMusic=v);Action("Abrir Spotify",()=>clock.OpenSpotify());Note(clock.MusicDescription(),65);Note("Faixa, artista e controles vêm do Spotify desktop. O Windows não informa o nome da playlist. Navegador e outros players não são controlados.",48);
        Heading("Tamanho e atualização");Note("Largura do relógio (pixels; reduz para caber na tela)",20);var width=new NumericUpDown {Minimum=240,Maximum=500,Value=clock.Settings.Width,Width=120,AccessibleName="Largura do relógio"};width.ValueChanged+=(s,e)=>{clock.Settings.Width=(int)width.Value;Changed();if(width.Value!=clock.Settings.Width)width.Value=clock.Settings.Width;};layout.Controls.Add(width);
        Note("Intervalo de consulta das cotas (segundos)",20);var interval=new NumericUpDown {Minimum=5,Maximum=120,Value=clock.Settings.RefreshSeconds,Width=120,AccessibleName="Intervalo de consulta das cotas"};interval.ValueChanged+=(s,e)=>{clock.Settings.RefreshSeconds=(int)interval.Value;Changed();};layout.Controls.Add(interval);
        Heading("Mostrador e cores");string[] styles={"segments","text","outline","atlas"};Choice("Estilo dos números",new[]{"Rádio-relógio — sete segmentos","Tipográfico","Contorno","Meus números importados"},Array.IndexOf(styles,clock.Settings.ClockStyle),i=>clock.Settings.ClockStyle=styles[i]);
        string[] fonts={"sans","mono","serif","condensed","rounded"};Choice("Letras",new[]{"Clássica","Terminal","Editorial","Condensada","Suave"},Array.IndexOf(fonts,clock.Settings.Font),i=>clock.Settings.Font=fonts[i]);
        Action("Cor dos números e indicadores…",()=>ColorPick(true));Action("Cor dos textos…",()=>ColorPick(false));Check("Recolorir meus números importados",clock.Settings.TintDigits,v=>clock.Settings.TintDigits=v);
        Action("Importar números desenhados (PNG)…",ImportNumbers);Note("Faixa PNG: 11 células iguais em ordem 0123456789:. Exemplo: 1100 × 150 px. Até 1 MB.",36);
        Heading("Foto de fundo");Action("Escolher e recortar uma foto…",Photo);Action("Remover foto",()=>{clock.Settings.Background="";Changed();});
        Note("Fade / escurecimento (0 = foto original; 100 = preto)",22);var fade=new TrackBar {Minimum=0,Maximum=100,TickFrequency=25,Value=(int)(clock.Settings.Fade*100),Width=420,Height=40,AccessibleName="Escurecimento da foto"};fade.ValueChanged+=(s,e)=>{clock.Settings.Fade=fade.Value/100d;Changed();};layout.Controls.Add(fade);
        Heading("Abrir e salvar");Action("Reiniciar relógio",()=>clock.RestartClock());Check("Iniciar com o Windows",StartupEnabled(),v=>{try{using(var run=Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")){if(v)run.SetValue("RelogioIA-Publicacao","\""+Application.ExecutablePath+"\"");else run.DeleteValue("RelogioIA-Publicacao",false);}}catch{MessageBox.Show("Não foi possível alterar o início com o Windows.",Text);BeginInvoke(new Action(Reload));}});
        Action("Importar skin/configurações do editor…",Import);Action("Exportar skin/configurações…",Export);
        Heading("Leitura do consumo");Choice("Conta de IA",new[]{"Codex","Claude Code (login local)"},clock.Settings.Provider=="claude"?1:0,i=>{clock.Settings.Provider=i==1?"claude":"codex";clock.ApplySettings();clock.ReadUsage();});Note("Claude depende do login local do Claude Code. A consulta é limitada a uma por minuto; não altera seu login.",40);Action("Atualizar consumo agora (F5)",()=>clock.ReadUsage());reading=new Label {Width=420,Height=140,Text=clock.ReadingDescription(),AccessibleName="Diagnóstico da leitura",Margin=new Padding(0,6,0,8)};layout.Controls.Add(reading);
        Note("O relógio consulta cotas de conta, não o gasto em reais nem a utilização por aplicativo.",40);
        Heading("Sons e alertas");Check("Avisar com som quando a cota estiver acabando",clock.Settings.AlertsEnabled,v=>clock.Settings.AlertsEnabled=v);Note("Avisar quando restar esta porcentagem da cota",22);var remaining=new NumericUpDown {Minimum=1,Maximum=50,Value=clock.Settings.AlertRemaining,Width=120,AccessibleName="Porcentagem restante para alerta"};remaining.ValueChanged+=(s,e)=>{clock.Settings.AlertRemaining=(int)remaining.Value;Changed();};layout.Controls.Add(remaining);Action("Escolher meu som de alerta (WAV)…",ImportSound);Action("Usar som padrão do Windows",()=>{clock.Settings.AlertSound="";Changed();});Action("Ouvir som de alerta",()=>clock.TestAlertSound());Note("Som opcional, desligado por padrão. Um aviso ao cruzar o limite por cota; abrir o relógio não dispara alertas. WAV PCM, até 2 MB.",48);
        Action("Concluir e voltar ao relógio",()=>Close());loading=false;layout.ResumeLayout();
    }
    public void UpdateReading(string value){if(reading!=null&&!reading.IsDisposed)reading.Text=value;}
    bool StartupEnabled(){try{using(var run=Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))return run!=null&&run.GetValue("RelogioIA-Publicacao")!=null;}catch{return false;}}
    public static void RepairStartupPath(){try{using(var run=Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run",true))if(run!=null&&run.GetValue("RelogioIA-Publicacao")!=null)run.SetValue("RelogioIA-Publicacao","\""+Application.ExecutablePath+"\"");}catch{/* Startup is optional; never prevents the clock opening. */}}
    void ColorPick(bool accent){using(var dialog=new ColorDialog()){var theme=Theme.Get(clock.Settings.Theme);dialog.Color=accent?(clock.Settings.Accent!=""?Theme.C(clock.Settings.Accent):theme.Accent):(clock.Settings.TextColor!=""?Theme.C(clock.Settings.TextColor):theme.Text);if(dialog.ShowDialog(this)==DialogResult.OK){string hex="#"+dialog.Color.R.ToString("X2")+dialog.Color.G.ToString("X2")+dialog.Color.B.ToString("X2");if(accent)clock.Settings.Accent=hex;else clock.Settings.TextColor=hex;Changed();}}}
    void Import(){using(var d=new OpenFileDialog {Filter="Skin/configurações|*.json"})if(d.ShowDialog(this)==DialogResult.OK)try{if(new FileInfo(d.FileName).Length>4000000)throw new Exception();string raw=File.ReadAllText(d.FileName);Data.Json.DeserializeObject(raw);clock.Settings=ClockSettings.Parse(raw);clock.ApplySettings();Reload();}catch{MessageBox.Show("Arquivo inválido ou maior que 4 MB.",Text);}}
    void Export(){using(var d=new SaveFileDialog {Filter="Skin/configurações|*.json",FileName="relogio-ia-skin.json"})if(d.ShowDialog(this)==DialogResult.OK)try{File.WriteAllText(d.FileName,clock.Settings.Serialize());}catch{MessageBox.Show("Não foi possível salvar o arquivo.",Text);}}
    void Photo(){using(var d=new OpenFileDialog {Filter="Foto|*.png;*.jpg;*.jpeg"})if(d.ShowDialog(this)==DialogResult.OK)try{if(new FileInfo(d.FileName).Length>5000000)throw new Exception();using(var image=Image.FromFile(d.FileName)){if(image.Width*1L*image.Height>25000000)throw new Exception();using(var crop=new PhotoCrop(image)){if(crop.ShowDialog(this)==DialogResult.OK){clock.Settings.Background=crop.Result;Changed();}}}}catch{MessageBox.Show("Use PNG/JPG de até 5 MB e 25 megapixels.",Text);}}
    void ImportSound(){using(var dialog=new OpenFileDialog {Filter="Som WAV PCM|*.wav"})if(dialog.ShowDialog(this)==DialogResult.OK)try{if(new FileInfo(dialog.FileName).Length>2000000)throw new Exception();byte[] bytes=File.ReadAllBytes(dialog.FileName);if(!SoundAlerts.ValidWave(bytes))throw new Exception();clock.Settings.AlertSound="data:audio/wav;base64,"+Convert.ToBase64String(bytes);Changed();}catch{MessageBox.Show("Use um WAV PCM de 8 ou 16 bits, mono/estéreo, até 2 MB.",Text);}}
    void ImportNumbers(){using(var d=new OpenFileDialog {Filter="Números em faixa PNG|*.png"})if(d.ShowDialog(this)==DialogResult.OK)try{
        if(new FileInfo(d.FileName).Length>1000000)throw new Exception();using(var image=Image.FromFile(d.FileName)){
            if(image.Width<110||image.Width%11!=0||image.Width>4096||image.Height<16||image.Height>512)throw new Exception();
            using(var output=new Bitmap(704,96))using(var g=Graphics.FromImage(output))using(var stream=new MemoryStream()){
                float cell=image.Width/11f,f=Math.Min(64f/cell,96f/image.Height);for(int i=0;i<11;i++)g.DrawImage(image,new RectangleF(i*64+(64-cell*f)/2,(96-image.Height*f)/2,cell*f,image.Height*f),new RectangleF(i*cell,0,cell,image.Height),GraphicsUnit.Pixel);
                output.Save(stream,ImageFormat.Png);clock.Settings.Atlas="data:image/png;base64,"+Convert.ToBase64String(stream.ToArray());clock.Settings.ClockStyle="atlas";Changed();Reload();
            }
        }
    }catch{MessageBox.Show("PNG até 1 MB; 11 células iguais em ordem 0123456789:. Altura de 16 a 512 px.",Text);}}
}

public class PhotoCrop : Form {
    readonly Image source;readonly PictureBox preview=new PictureBox {Width=480,Height=240,Left=12,Top=12};
    readonly TrackBar zoom=new TrackBar {Minimum=100,Maximum=300,Value=100},horizontal=new TrackBar {Minimum=0,Maximum=100,Value=50},vertical=new TrackBar {Minimum=0,Maximum=100,Value=50};
    public string Result="";
    public PhotoCrop(Image image){source=image;Text="Recortar foto — Relógio IA";ClientSize=new Size(504,430);FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;StartPosition=FormStartPosition.CenterParent;Controls.Add(preview);int y=260;
        foreach(var pair in new[]{new{Text="Zoom",Slider=zoom},new{Text="Horizontal",Slider=horizontal},new{Text="Vertical",Slider=vertical}}){Controls.Add(new Label {Text=pair.Text,Left=12,Top=y+7,Width=85});pair.Slider.SetBounds(100,y,390,38);pair.Slider.AccessibleName=pair.Text;pair.Slider.ValueChanged+=(s,e)=>preview.Invalidate();Controls.Add(pair.Slider);y+=40;}
        var apply=new Button {Text="Aplicar recorte",Left=335,Top=388,Width=155,Height=30};apply.Click+=(s,e)=>{using(var bitmap=new Bitmap(1200,600))using(var g=Graphics.FromImage(bitmap))using(var stream=new MemoryStream()){Draw(g,1200,600);bitmap.Save(stream,ImageFormat.Jpeg);string data="data:image/jpeg;base64,"+Convert.ToBase64String(stream.ToArray());if(data.Length>2000000){MessageBox.Show("Recorte muito grande. Tente outra imagem.");return;}Result=data;DialogResult=DialogResult.OK;}};Controls.Add(apply);preview.Paint+=(s,e)=>Draw(e.Graphics,480,240);
    }
    void Draw(Graphics g,int w,int h){float cw=Math.Min(source.Width,source.Height*2f)/(zoom.Value/100f),ch=cw/2;float x=(source.Width-cw)*horizontal.Value/100f,y=(source.Height-ch)*vertical.Value/100f;g.Clear(Color.Black);g.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;g.DrawImage(source,new Rectangle(0,0,w,h),new RectangleF(x,y,cw,ch),GraphicsUnit.Pixel);}
}
