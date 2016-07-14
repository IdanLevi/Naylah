﻿using FFImageLoading.Forms;
using Naylah.Xamarin.Common;
using Naylah.Xamarin.Controls.Customizations;
using Naylah.Xamarin.Controls.Pages;
using Naylah.Xamarin.Controls.Style;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using System.IO;
using System.Net.Http;
using PCLStorage;
using Naylah.Xamarin.Converters;
using Naylah.Xamarin.Helpers;

namespace Naylah.Xamarin.Controls.Choosers
{
    public class ImageChooser : ContentPageBase
    {
        public ImageChooserOptions ImageChooserOptionsData { get; set; }

        public IFile CurrentMediaFile { get; private set; }

        Button doneSelectionButton;
        CachedImage image;

        ContentView topContentView;
        ContentView centerContentView;
        ContentView bottomContentView;
        ContentLoader contentLoader;

        public ImageChooser()
        {
            BindingContext = this;

            PropertyChanged += ImageChooser_PropertyChanged;

            IsLoading = true;

            CreateUI();

        }

        private void CreateUI()
        {

            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength (1, GridUnitType.Star)  },
                    new RowDefinition { Height = GridLength.Auto },

                },
                ColumnSpacing = 0,
                RowSpacing = 0
            };

            topContentView = new ContentView();
            centerContentView = new ContentView();
            bottomContentView = new ContentView() { HeightRequest = 60 };

            //centerContentView.SetBinding(IsVisibleProperty, Binding.Create<ImageChooser>(t => t.IsLoading, BindingMode.OneWay, new InversiveBooleanConverter()));
            //bottomContentView.SetBinding(IsVisibleProperty, Binding.Create<ImageChooser>(t => t.IsLoading, BindingMode.OneWay, new InversiveBooleanConverter()));

            if (BootStrapper.CurrentApp.StyleKit != null)
            {
                topContentView.BackgroundColor = BootStrapper.CurrentApp.StyleKit.PrimaryColor;
                bottomContentView.BackgroundColor = BootStrapper.CurrentApp.StyleKit.PrimaryColor;
            }

            grid.Children.Add(topContentView, 0, 0);
            grid.Children.Add(centerContentView, 0, 1);
            grid.Children.Add(bottomContentView, 0, 2);



            var takePictureFromLibraryButton = new Button()
            {
                Image = "ic_photo_size_select_actual_white",
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                BorderWidth = 0,
            };

            takePictureFromLibraryButton.Clicked += TakePictureFromLibraryButton_Clicked;

            var takePictureFromCameraButton = new Button()
            {
                Image = "ic_photo_camera_white",
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                BorderWidth = 0,
            };
            takePictureFromCameraButton.Clicked += TakePictureFromCameraButton_Clicked;

            var topContentViewStackLayout = new StackLayout()
            {
                Padding = new Thickness(12, 6, 12, 6),
                Spacing = 12,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                Children =
                {
                    takePictureFromCameraButton,
                    takePictureFromLibraryButton,
                }
            };

            var topContentViewStackLayoutScrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                Content = topContentViewStackLayout
            };

            topContentView.Content = topContentViewStackLayoutScrollView;

            doneSelectionButton = new Button()
            {
                Image = "ic_done_white",
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                BorderWidth = 0,
            };
            doneSelectionButton.Clicked += DoneSelectionButton_Clicked;

            if (BootStrapper.CurrentApp.StyleKit != null)
            {
                doneSelectionButton.TextColor = BootStrapper.CurrentApp.StyleKit.SecondaryTextColor;
            }

            bottomContentView.Content = doneSelectionButton;

            contentLoader = new ContentLoader(this)
            {
                Content = grid,
                LoadingContent = GetImageChooserLoadingContent(),
                HandlePageBack = true,
                HideNavigationBar = true,
            };

            OnPropertyChanged(nameof(IsLoading));

            this.Content = contentLoader;

        }

        public virtual View GetImageChooserLoadingContent()
        {

            var contentLoadingV = new ContentView();

            var activityIndicator = new ActivityIndicator();
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, Binding.Create<ImageChooser>(vm => vm.IsLoading));
            activityIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, Binding.Create<ImageChooser>(vm => vm.IsLoading));

            var entry = new Label();
            entry.Text = "Loading...";
            entry.HorizontalTextAlignment = TextAlignment.Center;


            var stackLayout = new StackLayout()
            {
                Spacing = 10,
                Padding = 8,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            stackLayout.Children.Add(activityIndicator);
            stackLayout.Children.Add(entry);

            stackLayout.BindingContext = this;

            if (BootStrapper.CurrentApp.StyleKit != null)
            {
                entry.TextColor = BootStrapper.CurrentApp.StyleKit.SecondaryTextColor;
                activityIndicator.Color = BootStrapper.CurrentApp.StyleKit.SecondaryTextColor;
                contentLoadingV.BackgroundColor = BootStrapper.CurrentApp.StyleKit.PrimaryColor;
            }

            contentLoadingV.Content = stackLayout;

            return contentLoadingV;

        }

        private void ImageChooser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsLoading))
            {
                if (contentLoader != null)
                {
                    contentLoader.IsLoading = IsLoading;
                }
            }
        }

        private async void DoneSelectionButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (IsLoading)
                {
                    return;
                }

                IsLoading = true;

                if (image.Source == null)
                {
                    return;
                }

                byte[] imageData = null;

                if (ImageChooserOptionsData.SizeRequested != null)
                {
                    if (ImageChooserOptionsData.MediaExtension == ImageChooserImageExtension.Png)
                    {
                        imageData = await image.GetImageAsPngAsync(Convert.ToInt32(ImageChooserOptionsData.SizeRequested.Value.Width), Convert.ToInt32(ImageChooserOptionsData.SizeRequested.Value.Height));
                    }
                    else
                    {
                        imageData = await image.GetImageAsJpgAsync(Convert.ToInt32(ImageChooserOptionsData.SizeRequested.Value.Width), Convert.ToInt32(ImageChooserOptionsData.SizeRequested.Value.Height));
                    }
                }
                else
                {
                    if (ImageChooserOptionsData.MediaExtension == ImageChooserImageExtension.Png)
                    {
                        imageData = await image.GetImageAsPngAsync();
                    }
                    else
                    {
                        imageData = await image.GetImageAsPngAsync();
                    }

                }

                var file = await GetTempNewFile();

                await file.SaveByteArrayToThisFile(imageData);

                CurrentMediaFile = file;

                TouchAndChangeImageSource(file.Path);

                await ImageChooserOptionsData?.DoneSelectionAction?.Invoke(CurrentMediaFile);

            }
            catch (Exception ex)
            {
                ImageChooserOptionsData?.ExceptionOccurredAction?.Invoke(ex);
            }
            finally
            {
                IsLoading = false;
            }


        }

        private async void TakePictureFromLibraryButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (IsLoading)
                {
                    return;
                }

                IsLoading = true;

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync();

                if (file != null)
                {
                    TouchAndChangeImageSource(file.Path);
                }

            }
            catch (Exception ex)
            {
                ImageChooserOptionsData?.ExceptionOccurredAction?.Invoke(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void TakePictureFromCameraButton_Clicked(object sender, EventArgs e)
        {

            try
            {
                if (IsLoading)
                {
                    return;
                }

                IsLoading = true;

                await CrossMedia.Current.Initialize();

                if (((!CrossMedia.Current.IsCameraAvailable)) || (!CrossMedia.Current.IsTakePhotoSupported))
                {
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions()
                {
                    Directory = "TempSelection",
                    SaveToAlbum = false,
                    Name = Guid.NewGuid().ToString() + GetMediaExtensionAsString()
                });

                if (file != null)
                {
                    TouchAndChangeImageSource(file.Path);
                }
            }
            catch (Exception ex)
            {
                ImageChooserOptionsData?.ExceptionOccurredAction?.Invoke(ex);
            }
            finally
            {
                IsLoading = false;
            }

        }

        private void ApplyOptions(ImageChooserOptions imageChooserOptions)
        {
            ImageChooserOptionsData = imageChooserOptions;

            Title = ImageChooserOptionsData.Title;
            doneSelectionButton.Text = ImageChooserOptionsData.SelectionButtonText;

            LoadImageFromUri(ImageChooserOptionsData.ActualImageUri);

        }

        private async Task LoadImageFromUri(Uri actualImageUri)
        {
            try
            {
                image = new CachedImage();

                centerContentView.Content = image;

                var imagebuffer = await DownloadHelper.DownloadAsByteArray(actualImageUri);
                var file = await GetTempNewFile();
                await file.SaveByteArrayToThisFile(imagebuffer);

                CurrentMediaFile = file;

                TouchAndChangeImageSource(CurrentMediaFile.Path);

            }
            catch (Exception ex)
            {
                ImageChooserOptionsData?.ExceptionOccurredAction?.Invoke(ex);
            }
            finally
            {
                IsLoading = false;
            }


        }

        private void TouchAndChangeImageSource(string filePath)
        {
            image.Source = null;
            image.Source = ImageSource.FromFile(filePath);
        }

        public async Task<IFolder> GetTempWorkFolder()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("TempSelection", CreationCollisionOption.OpenIfExists);
            return folder;
        }

        public async Task<IFile> GetTempNewFile()
        {
            var folder = await GetTempWorkFolder();
            IFile file = await folder.CreateFileAsync(Guid.NewGuid().ToString() + GetMediaExtensionAsString(),
                    CreationCollisionOption.ReplaceExisting);

            return file;
        }

        public string GetMediaExtensionAsString()
        {
            return (ImageChooserOptionsData.MediaExtension == ImageChooserImageExtension.Png) ? ".png" : ".jpeg";
        }


        public static ImageChooser CreateImageChooser(
            ImageChooserOptions imageChooserOptions
            )
        {
            var imageChooser = new ImageChooser();
            imageChooser.ApplyOptions(imageChooserOptions);
            return imageChooser;
        }



        public class ImageChooserOptions
        {
            public string Title { get; set; } = "Image chooser";

            public string SelectionButtonText { get; set; } = "Select";


            public Uri ActualImageUri { get; set; }

            public Size? SizeRequested { get; set; }

            public ImageChooserImageExtension MediaExtension { get; set; }

            public Action<Exception> ExceptionOccurredAction { get; set; }

            public Func<IFile, Task> DoneSelectionAction { get; set; }



        }

        public enum ImageChooserImageExtension
        {
            Png,
            Jpeg
        }
    }


}
