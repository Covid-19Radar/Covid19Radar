namespace Covid19Radar.Services
{
    public interface ISkipBackup
    {
        // only iOS
        void skipBackup(string fileName);
    }
}
