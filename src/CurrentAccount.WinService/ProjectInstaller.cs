using System.ComponentModel;


namespace CurrentAccount.WinService
{

    /// <summary>
    /// Класс-установщик сервиса
    /// </summary>
	[RunInstaller(true)]
	public partial class ProjectInstaller: System.Configuration.Install.Installer
	{
        /// <summary>
        /// Конструктор
        /// </summary>
		public ProjectInstaller()
		{
			InitializeComponent();
		}
	}
}
