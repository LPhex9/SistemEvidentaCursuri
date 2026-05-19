using SistemEvidentaCursuri.Forms;

namespace SistemEvidentaCursuri;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormMain());
    }
}