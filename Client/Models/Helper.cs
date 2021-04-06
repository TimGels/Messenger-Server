﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.Models
{
    public static class Helper
    {
        public static ContentDialog AboutDialog()
        {
            ContentDialog aboutDialog = new ContentDialog
            {
                Title = "About Messenger Vision",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary
            };

            aboutDialog.Content += "Application: Messenger Vision\n";
            aboutDialog.Content += "Version: 1.0\n";
            aboutDialog.Content += "Developers: Jochem Brans, Johannes Kauffmann, Sietze Koonstra, Tim Gels, Rik van Rijn, Ruben Kuilder\n";

            return aboutDialog;
        }
    }
}
