namespace FilmApp.Model
{
    public class DisqueDur
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string VolumeLabel { get; set; }

        public string Libelle
        {
            get { return this.ToString(); }
        }

        public DisqueDur(bool isSelected, string name, string volumeLabel)
        {
            this.Name = name;
            this.VolumeLabel = volumeLabel;
            this.IsSelected = isSelected;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? VolumeLabel : VolumeLabel + " (" + Name + ")";
        }
    }
}