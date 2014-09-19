using System.Collections.Generic;
using System.ComponentModel;

namespace WinApp.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        protected void SetProperty<T>(ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        ///     + Fast
        ///     - String literal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        ///     + Fastest
        ///     + Without string literal
        ///     We need cache PropertyChangedEventArgs ($PropertyName$Args)
        ///     Use PropertyChangedEventArgs $PropertyName$Args =
        ///     NotifyPropertyChangedHelper.CreateArgs<$ClassName$>(o => o.$PropertyName$);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="e"></param>
        protected void SetProperty<T>(ref T field, T value, PropertyChangedEventArgs e)
        {
            if (field != null && !field.Equals(value))
            {
                field = value;
                OnPropertyChanged(e);
            }
            else if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(e);
            }
        }

        /// <summary>
        ///     Use SetProperty if you can
        /// </summary>
        /// <param name="e"></param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion
    }
}