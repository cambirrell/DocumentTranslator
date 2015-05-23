﻿// // ----------------------------------------------------------------------
// // <copyright file="AccountViewModel.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>AccountViewModel.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;

    using TranslationAssistant.Business;
    using TranslationAssistant.Business.Model;
    using TranslationAssistant.DocumentTranslationInterface.Common;

    /// <summary>
    ///     The account view model.
    /// </summary>
    public class AccountViewModel : Notifyer
    {
        #region Fields

        /// <summary>
        ///     The application id.
        /// </summary>
        private string clientID;

        /// <summary>
        ///     The client secret.
        /// </summary>
        private string clientSecret;

        /// <summary>
        ///     The category identifier.
        /// </summary>
        private string categoryID;

        /// <summary>
        ///     The save account settings click command.
        /// </summary>
        private ICommand saveAccountSettingsClickCommand;

        /// <summary>
        ///     The status text.
        /// </summary>
        private string statusText;

        #endregion

        #region Constructors and Destructors


        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the application id.
        /// </summary>
        public string ClientID
        {
            get
            {
                return this.clientID;
            }

            set
            {
                this.clientID = value;
                this.NotifyPropertyChanged("ClientID");
            }
        }

        /// <summary>
        ///     Gets or sets the client secret.
        /// </summary>
        public string ClientSecret
        {
            get
            {
                return this.clientSecret;
            }

            set
            {
                this.clientSecret = value;
                this.NotifyPropertyChanged("ClientSecret");
            }
        }

        public string CategoryID
        {
            get
            {
                return this.categoryID;
            }

            set
            {
                this.categoryID = value;
                this.NotifyPropertyChanged("CategoryID");
            }
        }

        /// <summary>
        ///     Gets the save account settings click command.
        /// </summary>
        public ICommand SaveAccountSettingsClickCommand
        {
            get
            {
                return this.saveAccountSettingsClickCommand
                       ?? (this.saveAccountSettingsClickCommand = new DelegateCommand(this.SaveAccountClick));
            }
        }

        /// <summary>
        ///     Gets or sets the status text.
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                this.statusText = value;
                this.NotifyPropertyChanged("StatusText");
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Load the account settings from the DocumentTranslator.settings file, which is actually in the user appsettings folder and named user.config.
        /// </summary>
        public AccountViewModel()
        {
            this.clientID = TranslationAssistant.DocumentTranslationInterface.Properties.DocumentTranslator.Default.ClientID;
            this.clientSecret = TranslationAssistant.DocumentTranslationInterface.Properties.DocumentTranslator.Default.ClientSecret;
            this.categoryID = TranslationAssistant.DocumentTranslationInterface.Properties.DocumentTranslator.Default.CategoryID;
        }

        /// <summary>
        ///     Assemblies the browse_ click.
        /// </summary>
        private void SaveAccountClick()
        {
            //Set the Account values and save.
            TranslationAssistant.DocumentTranslationInterface.Properties.DocumentTranslator.Default.ClientID = this.clientID;
            TranslationAssistant.DocumentTranslationInterface.Properties.DocumentTranslator.Default.ClientSecret = this.clientSecret;
            TranslationAssistant.DocumentTranslationInterface.Properties.DocumentTranslator.Default.CategoryID = this.categoryID;
            TranslationAssistant.DocumentTranslationInterface.Properties.DocumentTranslator.Default.Save();

            //Set the public properties for the translation service.
            TranslationServices.Core.TranslationServiceFacade.ClientID = this.clientID;
            TranslationServices.Core.TranslationServiceFacade.ClientSecret = this.clientSecret;
            TranslationServices.Core.TranslationServiceFacade.CategoryID = this.categoryID;

            if (TranslationServices.Core.TranslationServiceFacade.IsTranslationServiceReady()) { 
                this.StatusText = "Settings saved. Ready to translate.";
                NotifyPropertyChanged("SettingsSaved");
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(true);
            }
            else
            {
                this.StatusText = "Client ID or client secret are invalid. Please visit the Azure Marketplace to obtain a subscription.";
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(false);
            }
            
        }

        #endregion
    }
}