using System;
using Stylet;
using StyletIoC;
using AWSV2.ViewModels;
using Common;

namespace AWSV2
{
    public class Bootstrapper : Bootstrapper<LoginViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            BootstrapperExt.ConfigureIoC(ref builder);
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
        }
    }
}
