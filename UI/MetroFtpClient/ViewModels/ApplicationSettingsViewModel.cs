using MahApps.Metro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MetroFtpClient.Core.Base;
using MetroFtpClient.Core.Interfaces;
using MetroFtpClient.Infrastructure.Constants;
using MetroFtpClient.Infrastructure.Interfaces;
using MetroFtpClient.Infrastructure.Services;
using MetroFtpClient.Model;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MetroFtpClient.ViewModels
{
    public class ApplicationSettingsViewModel : ViewModelBase
    {
        #region Members and Constants

        private IConfigurationFile applicationConfig = null;
        private ILocalizerService localizerService = null;

        private IEnumerable<Swatch> Swatches = null;

        #endregion Members and Constants

        public ApplicationSettingsViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            this.localizerService = Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService);

            // Get the config file
            this.applicationConfig = this.Container.Resolve<IConfigurationFile>(FileAndFolderConstants.ApplicationConfigFile);

            // create metro theme color menu items for the demo
            this.ApplicationThemes = ThemeManager.AppThemes
                                           .Select(a => new ApplicationTheme() { Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();

            // create accent colors list
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColor() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();

            this.Swatches = new SwatchesProvider().Swatches;

            // Language
            var languageTag = applicationConfig.Sections["GeneralSettings"].Settings["Language"].Value;
            var localizerService = this.Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService);
            if (localizerService != null)
            {
                this.SelectedLanguage = localizerService.SupportedLanguages.Where(l => l.IetfLanguageTag.Equals(languageTag)).FirstOrDefault();
            }

            // Theme
            string themeName = applicationConfig.Sections["GeneralSettings"].Settings["Theme"].Value;
            this.SelectedTheme = this.ApplicationThemes.Where(t => t.Name.Equals(themeName)).FirstOrDefault();

            // Accent color
            string accentColor = applicationConfig.Sections["GeneralSettings"].Settings["AccentColor"].Value;
            this.SelectedAccentColor = this.AccentColors.Where(t => t.Name.Equals(accentColor)).FirstOrDefault();
        }

        #region Properties

        private IList<ApplicationTheme> applicationsThemes;

        /// <summary>
        /// List with application themes
        /// </summary>
        public IList<ApplicationTheme> ApplicationThemes
        {
            get { return applicationsThemes; }
            set { this.SetProperty<IList<ApplicationTheme>>(ref this.applicationsThemes, value); }
        }

        private IList<AccentColor> accentColors;

        /// <summary>
        /// List with accent colors
        /// </summary>
        public IList<AccentColor> AccentColors
        {
            get { return accentColors; }
            set { this.SetProperty<IList<AccentColor>>(ref this.accentColors, value); }
        }

        private ApplicationTheme selectedTheme;

        /// <summary>
        /// The selected theme
        /// </summary>
        public ApplicationTheme SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                if (this.SetProperty<ApplicationTheme>(ref this.selectedTheme, value))
                {
                    var theme = ThemeManager.DetectAppStyle(Application.Current);
                    var appTheme = ThemeManager.GetAppTheme(value.Name);
                    ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);

                    this.applicationConfig.Sections["GeneralSettings"].Settings["Theme"].Value = value.Name;
                    this.applicationConfig.Save();
                }
            }
        }

        /// <summary>
        /// Set material design color
        /// </summary>
        private void SetMaterialDesignAccentColor()
        {
            var swatch = this.Swatches.Where(s => s.Name.ToUpper().Equals(this.SelectedAccentColor.Name.ToUpper())).FirstOrDefault();

            if (swatch != null)
            {
                var ph = new PaletteHelper();
                ph.ReplacePrimaryColor(swatch);
                ph.ReplaceAccentColor(swatch);
            }
        }

        private AccentColor selectedAccentColor;

        /// <summary>
        /// Selected accent color
        /// </summary>
        public AccentColor SelectedAccentColor
        {
            get { return selectedAccentColor; }
            set
            {
                if (this.SetProperty<AccentColor>(ref this.selectedAccentColor, value))
                {
                    var theme = ThemeManager.DetectAppStyle(Application.Current);
                    var accent = ThemeManager.GetAccent(value.Name);
                    ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);

                    // Set material design color
                    SetMaterialDesignAccentColor();

                    this.applicationConfig.Sections["GeneralSettings"].Settings["AccentColor"].Value = value.Name;
                    this.applicationConfig.Save();
                }
            }
        }

        /// <summary>
        /// Supported languages
        /// </summary>
        public IList<CultureInfo> SupportedLanguages
        {
            get
            {
                if (localizerService != null)
                {
                    return localizerService.SupportedLanguages;
                }

                return null;
            }
        }

        /// <summary>
        /// The selected language
        /// </summary>
        public CultureInfo SelectedLanguage
        {
            get { return (localizerService != null) ? localizerService.SelectedLanguage : null; }
            set
            {
                if (value != null && value != this.localizerService.SelectedLanguage)
                {
                    if (localizerService != null)
                    {
                        this.localizerService.SelectedLanguage = value;

                        this.applicationConfig.Sections["GeneralSettings"].Settings["Language"].Value = value.IetfLanguageTag;
                        this.applicationConfig.Save();
                    }
                }
            }
        }

        #endregion Properties
    }
}