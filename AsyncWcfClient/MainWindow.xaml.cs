using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using AsyncWcfLib;

namespace AsyncWcfClient
{
    public partial class MainWindow
    {
        #region WCF

        private async Task DeadlockBeHere(bool useAsync, bool cancel, bool capture, bool captureReturn, bool delay) {
            bool success = false;
            try {
                if(useAsync) {
                    await this.DoWorkAsync(cancel, capture).ConfigureAwait(captureReturn);
                } else {
                    this.DoWorkSync(cancel);
                }
                success = true;
            } catch(Exception ex) {
                if(ex is OperationCanceledException) {
                    this.Output("Work canceled.");
                } else {
                    this.Output("An exception occurred: {0}", ex);
                }
            }
            if(!success) {
                // Normally this would be in the catch block above, but then I wouldn't be able to await Task.Delay
                try {
                    if(useAsync && delay) {
                        await Task.Delay(10).ConfigureAwait(capture);
                    }
                    this.Output("Work failed, notifying server...");
                    // This method deadlocks in one specific case:
                    // When an exception is thrown after using ConfigureAwait(false), and there is no context
                    // switch (another await, or using ConfigureAwait(true) when awaiting DoWorkAsync) before
                    // this method is called.
                    mProxy.WorkDone(false);
                    this.Output("Finished.");
                } catch(Exception ex) {
                    this.Output("Failed to notify server: {0}", ex);
                }
            }
        }

        /// <summary>
        /// Call WCF methods asynchronously.
        /// </summary>
        private async Task DoWorkAsync(bool cancel, bool capture) {
            this.Output("Asking for work...");
            bool shouldDoWork = await mProxy.ShouldDoWorkAsync(1).ConfigureAwait(capture);
            if(!shouldDoWork) {
                this.Output("No work to do.");
                return;
            }
            this.Output("Doing work...");
            byte[] data = new byte[20];
            new Random().NextBytes(data);
            await mProxy.DoWorkAsync(data).ConfigureAwait(capture);
            if(cancel) {
                throw new OperationCanceledException("Work canceled by user.");
            }
            this.Output("Work done, notifying server...");
            await mProxy.WorkDoneAsync(true).ConfigureAwait(capture);
            this.Output("Finished.");
        }

        /// <summary>
        /// Call WCF methods synchronously.
        /// </summary>
        private void DoWorkSync(bool cancel) {
            this.Output("Asking for work...");
            bool shouldDoWork = mProxy.ShouldDoWork(1);
            if(!shouldDoWork) {
                this.Output("No work to do.");
                return;
            }
            this.Output("Doing work...");
            byte[] data = new byte[20];
            new Random().NextBytes(data);
            mProxy.DoWork(data);
            if(cancel) {
                throw new OperationCanceledException("Work canceled by user.");
            }
            this.Output("Work done, notifying server...");
            mProxy.WorkDone(true);
            this.Output("Finished.");
        }

        #endregion

        #region Stuff

        private readonly ChannelFactory<IServiceAsync> mFactory;
        private IServiceAsync mProxy;

        public MainWindow() {
            InitializeComponent();
            mFactory = new ChannelFactory<IServiceAsync>(Util.CreateBinding(), new EndpointAddress(Util.CreateAddress(true)));
            mFactory.Open();
        }

        private IClientChannel ClientChannel {
            get {
                return (IClientChannel)mProxy;
            }
        }

        private void Output(string text) {
            Dispatcher.BeginInvoke((Action)delegate {
                this.OutputText.AppendText(string.Format("{0}\n", text));
                this.OutputText.ScrollToEnd();
            });
        }

        private void Output(string format, params object[] args) {
            this.Output(string.Format(format, args));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Connect to the server.
        /// </summary>
        private void Connect_Click(object sender, RoutedEventArgs e) {
            if(mProxy != null) {
                this.Output("Already connected.");
                return;
            }
            try {
                this.Output("Connecting...");
                mProxy = mFactory.CreateChannel();
                this.ClientChannel.Open();
                this.Output("Connected.");
            } catch(Exception ex) {
                this.Output("Failed to connect: {0}", ex);
                if(mProxy != null) {
                    this.ClientChannel.Abort();
                }
                mProxy = null;
            }
        }

        /// <summary>
        /// Close connection and delete proxy.
        /// </summary>
        private void Disconnect_Click(object sender, RoutedEventArgs e) {
            if(mProxy == null) {
                this.Output("Not connected.");
                return;
            }
            try {
                this.Output("Disconnecting...");
                this.ClientChannel.Close();
            } catch {
                this.ClientChannel.Abort();
            } finally {
                this.Output("Disconnected.");
                mProxy = null;
            }
        }

        private async void DoWork_Click(object sender, RoutedEventArgs e) {
            if(mProxy == null) {
                this.Output("Not connected.");
                return;
            }
            bool cancel = (ThrowCheckBox.IsChecked == true);
            bool useAsync = (AsyncCheckBox.IsChecked == true);
            bool capture = (this.ConfigureAwaitCheckBox.IsChecked == false);
            bool captureReturn = (this.CaptureReturnCheckBox.IsChecked == true);
            bool delay = (this.DelayCheckBox.IsChecked == true);
            await this.DeadlockBeHere(useAsync, cancel, capture, captureReturn, delay);
        }

        #endregion
    }
}
